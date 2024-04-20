using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using BigInteger = System.Numerics.BigInteger;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GeneralManager generalManager;
    [SerializeField] private HeroStatsManager heroStatsManager;
    [SerializeField] private GearsStatsManager gearsStatsManager;
    [SerializeField] private SkillStatsManager skillStatsManager;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private MapManager mapManager;
    [SerializeField] private BoxScreenCollision boxScreenCollision;
    [SerializeField] private BossStage bossStage;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private DarkBoardLoadingUI darkBoardLoading;
    [SerializeField] private Transform tfmHeroPool, tfmEnemyPool, tfmSkillPool, tfmBulletPool, tfmEffectPool;
    [SerializeField] private SkillTable skillTable;

    [SerializeField] private Color colorHero;

    [SerializeField] private int numberOfEnemyMax;
    [SerializeField] private int numberOfEnemyInRow;
    [SerializeField] private string earlyGold, earlyBlueGem, earlyPinkGem;

    private BigInteger gold, blueGem, pinkGem;
    private UnityAction notifyGameOverAction;
    private UserInfo userInfo;
    private UserInfoManager userInfoManager;
    private ObjectPooling objectPooling;
    private List<Character> enemyList = new List<Character>(), heroList = new List<Character>();

    public bool BossDie { get; private set; }

    public UnityAction NotifyGameOverAction { get => notifyGameOverAction; set => notifyGameOverAction = value; }

    public UserInfo UserInfo { get => userInfo; }

    public UIManager UiManager { get => uiManager; }

    private void Start()
    {
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false; 
#endif
        userInfoManager = UserInfoManager.Instance;
        objectPooling = ObjectPooling.Instance;
        mapManager.LoadMapData();
        skillTable.GetAutomaticData();
        LoadAllDatas();
        SetUserInfoData();
        SetDontDestroyOnLoad();
        MapData mapData = mapManager.GetMapData();
        SceneManager.LoadScene($"Map{mapData.map}");
    }

    private void LoadAllDatas()
    {
        // gold, gems
        gold = BigInteger.Parse(PlayerPrefs.GetString("GOLDDATA", earlyGold));
        blueGem = BigInteger.Parse(PlayerPrefs.GetString("BLUEGEMDATA", earlyBlueGem));
        pinkGem = BigInteger.Parse(PlayerPrefs.GetString("PINKGEMDATA", earlyPinkGem));
        // show on ui
        uiManager.SetTextGold(gold, true);
        uiManager.SetTextPinkGem(pinkGem, true);
        uiManager.SetTextBlueGem(blueGem, true);
        // stats
        generalManager.LoadGeneralData();
        EventDispatcher.Push(EventId.CheckGoldToEnhance, gold);

        heroStatsManager.LoadHeroesData();
        gearsStatsManager.LoadGearsData(GearType.Weapon);
        gearsStatsManager.LoadGearsData(GearType.Armor);
        skillStatsManager.CheckUnlockSkillItem();
        skillStatsManager.LoadSkillData();
    }

    private void SetUserInfoData()
    {
        userInfo = userInfoManager.GetUserInfo();
        userInfoManager.SetATK();
        userInfoManager.SetHp();
        userInfoManager.SetATKSpeed();
        userInfoManager.SetHPRecovery();
        userInfoManager.SetCriticalHitChance();
        userInfoManager.SetCriticalHitDamage();
        userInfoManager.SetGoldObtain();
        userInfoManager.SetSkillsDamage();
        userInfoManager.SetBossDamage();
    }

    private void SetDontDestroyOnLoad()
    {
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(uiManager);
        DontDestroyOnLoad(NumberConverter.Instance);
        DontDestroyOnLoad(cameraController);
        DontDestroyOnLoad(tfmHeroPool);
        DontDestroyOnLoad(tfmEnemyPool);
        DontDestroyOnLoad(tfmSkillPool);
        DontDestroyOnLoad(tfmBulletPool);
        DontDestroyOnLoad(tfmEffectPool);
        DontDestroyOnLoad(SoundManager.Instance);
    }

    public void RenewGameState(bool resetWave)
    {
        BossDie = false;
        boxScreenCollision.ClearEnemyInBox();
        // Remove all heroes and enemies are in battle field
        while (enemyList.Count > 0)
        {
            objectPooling.RemoveGOInPool(enemyList[0].gameObject, PoolType.Enemy);
            enemyList.RemoveAt(0);
        }
        while (heroList.Count > 0)
        {
            objectPooling.RemoveGOInPool(heroList[0].gameObject, PoolType.Hero);
            heroList.RemoveAt(0);
        }
        // Load new hero 
        SpawnHeroInGame();

        if (resetWave)
            mapManager.ResetWave();
        else
            mapManager.LoadNextWave();

        SetFirstTarget();
        skillTable.ResetAllSkillTableItem();
    }

    public void RenewInRevengeBossStage()
    {
        boxScreenCollision.ClearEnemyInBox();
        // clear all creeps
        for (int i = 0; i < enemyList.Count;)
        {
            if (!enemyList[i].IsBoss)
            {
                objectPooling.RemoveGOInPool(enemyList[i].gameObject, PoolType.Enemy);
                enemyList.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
        // Let heroes find boss
        for (int i = 0; i < heroList.Count; i++)
        {
            Character target = heroList[i].FindEnemy();
            heroList[i].SetTarget(target);
            heroList[i].SetDirection(target.GetTransform().position);
        }
    }

    private void SetFirstTarget()
    {
        // Find enemy
        Character hero = GetCharacters(CharacterType.Hero)[0];
        Character target = hero.FindEnemy();
        if (target != null)
        {
            hero.SetTarget(target);
            hero.SetDirection(target.GetTransform().position);
        }
    }


    public BigInteger GetGold()
    {
        return gold;
    }

    public void SetGold(BigInteger _gold, bool addtional)
    {
        gold += addtional ? _gold : -_gold;
    }

    public BigInteger GetPinkGem()
    {
        return pinkGem;
    }

    public void SetPinkGem(BigInteger _gem, bool addtional)
    {
        pinkGem += addtional ? _gem : -_gem;
    }

    public BigInteger GetBlueGem()
    {
        return blueGem;
    }

    public void SetBlueGem(BigInteger _gem, bool addtional)
    {
        blueGem += addtional ? _gem : -_gem;
    }

    private void SpawnHeroInGame()
    {
        // get index wave to spawn hero
        int indexSpawningWave = Random.Range(0, mapManager.WaveList.Length);
        bool waveIndexIncreasing = Random.Range(0, 2) == 0 ? false : true;
        int frontWaveIndex; // ++
        int backWaveIndex; // --
        if (indexSpawningWave == mapManager.WaveList.Length - 1)
        {
            frontWaveIndex = 0;
            backWaveIndex = indexSpawningWave - 1;
        }
        else if (indexSpawningWave == 0)
        {
            frontWaveIndex = 1;
            backWaveIndex = mapManager.WaveList.Length - 1;
        }
        else
        {
            frontWaveIndex = indexSpawningWave + 1;
            backWaveIndex = indexSpawningWave - 1;
        }
        int indexWave = waveIndexIncreasing ? frontWaveIndex : backWaveIndex;

        // Get wave index to spawn enemies
        mapManager.SetFirstWaveIndex(indexWave, waveIndexIncreasing);
        // Get wave to spawn hero
        Wave wave = mapManager.WaveList[indexSpawningWave];
        float x = Random.Range(wave.leftRange, wave.rightRange);
        float z = Random.Range(wave.topRange, wave.bottomRange);
        Vector3 posSwnHero = new Vector3(x, wave.swnYPosition, z);
        GameObject prefabHero = GetHeroPrefab();
        Character character = SpawnHero(prefabHero, userInfo, posSwnHero, colorHero);
        character.GetTransform().SetParent(tfmHeroPool);
        cameraController.SetTfmHero(character.transform);
    }

    public Character SpawnHero(GameObject prefab, UserInfo heroInfo, Vector3 position, Color colorHpBar)
    {
        Character hero = objectPooling.SpawnG0InPool(prefab, position, PoolType.Hero).GetComponent<Character>();
        hero.SetAttackSpeed(heroInfo.atkSpeed);
        hero.SetCharacterInfo(heroInfo);
        hero.GetHpBar().SetHpBarColor(colorHpBar);
        hero.Init();
        hero.GetTransform().SetParent(tfmHeroPool);
        heroList.Add(hero);
        return hero;
    }

    public void SpawnEnemyInGame(Character prefab, Vector3 position, BigInteger atk, BigInteger hp)
    {
        Character character = objectPooling.SpawnG0InPool(prefab.gameObject, position, PoolType.Enemy).GetComponent<Character>();
        if (character.IsBoss)
            bossStage.StartBossStage(character);
        character.transform.SetParent(tfmEnemyPool);
        character.SetCharacterInfo(atk, hp);
        character.SetAttackSpeed(1);
        character.Init();
        enemyList.Add(character);
    }

    public List<Character> GetCharacters(CharacterType type) => type == CharacterType.Enemy ? enemyList : heroList;

    public GameObject GetHeroPrefab() => heroStatsManager.GetPrefabHero().gameObject;

    public void RemoveCharacterFromList(Character character, CharacterType type)
    {
        if (type == CharacterType.Enemy)
        {
            if (enemyList.Contains(character))
            {
                enemyList.Remove(character);
                if (enemyList.Count <= 0)
                {
                    //// check next move point
                    boxScreenCollision.ClearEnemyInBox();
                    if (character.IsBoss)
                    {
                        // clear clone hero 
                        while (heroList.Count > 1)
                        {
                            Character cloneHero = heroList[1];
                            heroList.RemoveAt(1);
                            objectPooling.RemoveGOInPool(cloneHero.gameObject, PoolType.Hero);
                            CloneHeroEffectController.Instance.CreateRemovingEffect(cloneHero.GetHeadTransform().position);
                        }
                        notifyGameOverAction();
                        BossDie = true;
                        mapManager.LoopWave(false);
                        mapManager.LoadNextTurn();
                    }
                    else
                    {
                        mapManager.LoadNextWave();
                    }
                }
            }
        }
        else
        {
            if (heroList.Contains(character))
            {
                if (heroList[0] == character)
                {
                    // hero die and then all clone heoes die
                    while (heroList.Count > 1)
                    {
                        Character cloneHero = heroList[1];
                        heroList.RemoveAt(1);
                        objectPooling.RemoveGOInPool(cloneHero.gameObject, PoolType.Hero);
                        CloneHeroEffectController.Instance.CreateRemovingEffect(cloneHero.GetHeadTransform().position);
                    }
                    notifyGameOverAction();
                    if (enemyList[0].IsBoss) // if enemy kill hero is boss
                        mapManager.LoopWave(true);
                    // load dark efect
                    darkBoardLoading.StartFadeBoard(true, () => RenewGameState(true));
                }
                else
                {
                    heroList.Remove(character);
                    objectPooling.RemoveGOInPool(character.gameObject, PoolType.Hero);
                    CloneHeroEffectController.Instance.CreateRemovingEffect(character.GetHeadTransform().position);
                }
            }
        }
    }

    public Character FindNearestCharacter(Vector3 curPosition, CharacterType type)
    {
        if (type == CharacterType.Enemy)
            return GetNearestCharacter(enemyList, curPosition);
        else
            return GetNearestCharacter(heroList, curPosition);
    }

    private Character GetNearestCharacter(List<Character> list, Vector3 curPosition)
    {
        if (list.Count <= 0)
            return null;

        float shortestDistance = Vector3.Distance(list[0].transform.position, curPosition);
        Character nearestCharacter = list[0];
        for (int i = 1; i < list.Count; i++)
        {
            float distance = Vector3.Distance(list[i].transform.position, curPosition);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestCharacter = list[i];
            }
        }
        return nearestCharacter;
    }

    public MapManager GetMapManager() => mapManager;

    public Transform GetSkillPoolTfm() => tfmSkillPool;

    public Transform GetBulletPoolTfm() => tfmBulletPool;
}

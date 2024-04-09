using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
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
    private CharacterPosition[] characterPositions;

    public bool BossDie { get; private set; }

    public bool IsOver { get; private set; }

    public UnityAction NotifyGameOverAction { get => notifyGameOverAction; set => notifyGameOverAction = value; }

    public UserInfo UserInfo { get => userInfo; }
    public UIManager UiManager { get => uiManager; }

    private void Start()
    {
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
        uiManager.SetTextGold(gold,true);
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
        DontDestroyOnLoad(FillData.Instance);
        DontDestroyOnLoad(cameraController);
        DontDestroyOnLoad(tfmHeroPool);
        DontDestroyOnLoad(tfmEnemyPool);
        DontDestroyOnLoad(tfmSkillPool);
        DontDestroyOnLoad(tfmBulletPool);
        DontDestroyOnLoad(tfmEffectPool);
    }

    public void RenewGameState(bool resetWave)
    {
        BossDie = false;
        boxScreenCollision.ClearEnemyInBox();
        // Remove all heroes and enemies are in battle field
        while (enemyList.Count > 0)
        {
            objectPooling.RemoveGOInPool(enemyList[0].gameObject, PoolType.Enemy, enemyList[0].name);
            enemyList.RemoveAt(0);
        }
        while (heroList.Count > 0)
        {
            objectPooling.RemoveGOInPool(heroList[0].gameObject, PoolType.Hero, heroList[0].name);
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

    public void SetHeroSwnPosition(CharacterPosition[] positions)
    {
        characterPositions = positions;
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
        int index = /*Random.Range(0, characterPositions.Length)*/0;
        Vector3 posSwnHero = characterPositions[index].tfmSwnHero.position;
        bool waveIndexIncreasing = Random.Range(0, 1) == 0 ? false : true;
        int indexWave = !waveIndexIncreasing ? characterPositions[index].frontWaveIndex : characterPositions[index].behindWaveIndex;
        // Get wave index to spawn enemies
        mapManager.SetFirstWaveIndex(indexWave, waveIndexIncreasing);

        GameObject prefabHero = GetHeroPrefab();
        Character character = SpawnHero(prefabHero, userInfo, posSwnHero, colorHero);
        character.GetTransform().SetParent(tfmHeroPool);
        cameraController.SetTfmHero(character.transform);
    }

    public Character SpawnHero(GameObject prefab, UserInfo heroInfo, Vector3 position, Color colorHpBar)
    {
        Character hero = objectPooling.SpawnG0InPool(prefab, position, PoolType.Hero).GetComponent<Character>();
        hero.name = prefab.name;
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
        character.transform.rotation = Quaternion.identity;
        character.name = prefab.name;

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
                    //// game win
                    //IsOver = true;
                    boxScreenCollision.ClearEnemyInBox();
                    if (character.IsBoss)
                    {
                        notifyGameOverAction();
                        BossDie = true;
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
                        cloneHero.TakeDamage(cloneHero.GetMaxHp(), DamageTakenType.Normal);
                    }
                    // game lose
                    IsOver = true;
                    notifyGameOverAction();
                    // load dark efect
                    darkBoardLoading.StartFadeBoard(true, () => RenewGameState(true));
                }
                else
                    heroList.Remove(character);
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

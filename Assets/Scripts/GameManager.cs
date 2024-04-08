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
    [SerializeField] private string goldString;
    [SerializeField] private string gemString;

    private BigInteger gold = 100000000000000, blueGem = 100000000000000, pinkGem = 100000000000000;
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
        // gold, gem
        uiManager.SetTextGold(gold);
        uiManager.SetTextPinkGem(pinkGem);
        uiManager.SetTextBlueGem(blueGem);
        // stats
        generalManager.LoadGeneralData();
        heroStatsManager.LoadHeroesData();
        gearsStatsManager.LoadGearsData(GearType.Weapon);
        gearsStatsManager.LoadGearsData(GearType.Armor);
        skillStatsManager.CheckUnlockSkillItem();
        skillStatsManager.LoadSkillData();
    }

    private void SetUserInfoData()
    {
        userInfo = userInfoManager.userInfo;
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

    ///// <summary>
    ///// 
    ///// </summary>
    //public void StartGameInNewMap()
    //{
    //    InitCharacters();
    //    skillTable.ResetAllSkillTableItem();
    //}

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

    ///// <summary>
    ///// Spawn hero end enemy when load new map model (next round and map)
    ///// </summary>
    //private void InitCharacters()
    //{
    //    while (heroList.Count > 0)
    //    {
    //        objectPooling.RemoveGOInPool(heroList[0].gameObject, PoolType.Hero, heroList[0].name);
    //        heroList.RemoveAt(0);
    //    }
    //    SpawnHeroInGame();
    //    mapManager.LoadNextWave();
    //    SetFirstTarget();
    //}

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
        gold = BigInteger.Parse(goldString);
        return gold;
    }

    public void SetGold(BigInteger remainGold)
    {
        gold = remainGold;
        uiManager.SetTextGold(gold);
    }

    public BigInteger GetPinkGem()
    {
        pinkGem = BigInteger.Parse(gemString);
        return pinkGem;
    }

    public void SetPinkGem(BigInteger remainGem)
    {
        pinkGem = remainGem;
        uiManager.SetTextPinkGem(pinkGem);
    }

    public BigInteger GetBlueGem()
    {
        blueGem = BigInteger.Parse(gemString);
        return blueGem;
    }

    public void SetBlueGem(BigInteger remainGem)
    {
        blueGem = remainGem;
        uiManager.SetTextPinkGem(blueGem);
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
                    //notifyGameOverAction();
                    boxScreenCollision.ClearEnemyInBox();
                    if (character.IsBoss)
                    {
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
                    // load dark efect
                    darkBoardLoading.StartFadeBoard(true, () => RenewGameState(true));
                    //notifyGameOverAction();
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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using BigInteger = System.Numerics.BigInteger;

[System.Serializable]
public class CharacterPosition
{
    public Transform tfmSwnHero;
    public int indexBackSwnEnemy, indexNextSwnEnemy;
}

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GeneralManager generalManager;
    [SerializeField] private HeroStatsManager heroStatsManager;
    [SerializeField] private GearsStatsManager gearsStatsManager;
    [SerializeField] private SkillStatsManager skillStatsManager;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private MapManager mapManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private Transform tfmHeroPool, tfmEnemyPool, tfmSkillPool;
    [SerializeField] private SkillTable skillTable;
    [SerializeField] private Color colorHero;

    [SerializeField] private int numberOfEnemyMax;
    [SerializeField] private int numberOfEnemyInRow;
    [SerializeField] private string goldString;
    [SerializeField] private string gemString;

    private int curIndexPosSwnEnemy;
    private bool isIndexPosSwnEIncreasing;

    private BigInteger gold = 100000000000000, gem = 100000000000000;
    private UnityAction notifyGameOverAction;
    private UserInfo userInfo;
    private UserInfoManager userInfoManager;
    private ObjectPooling objectPooling;
    private List<Character> enemyList = new List<Character>(), heroList = new List<Character>();
    private Transform[] tfmSwnEnemyList;
    private CharacterPosition[] characterPositions;

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
        SceneManager.LoadScene($"Map{mapData.map}_Round{mapData.round}");
    }

    private void LoadAllDatas()
    {
        // gold, gem
        uiManager.SetTextGold(gold);
        uiManager.SetTextGem(gem);
        // stats
        generalManager.LoadGeneralData();
        heroStatsManager.LoadHeroData();
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
    }

    public void StartGameState()
    {
        InitCharacters();
        skillTable.ResetAllSkillTableItem();
    }

    public void ResetGameState()
    {
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
        // Reload wave
        mapManager.ReloadWave();
        SetFirstTarget();
    }


    /// <summary>
    /// Spawn hero end enemy when load new map model (next round and map)
    /// </summary>
    private void InitCharacters()
    {
        while (heroList.Count > 0)
        {
            objectPooling.RemoveGOInPool(heroList[0].gameObject, PoolType.Hero, heroList[0].name);
            heroList.RemoveAt(0);
        }
        SpawnHeroInGame();
        mapManager.LoadNextWave();
        SetFirstTarget();
    }

    private void SetFirstTarget()
    {
        // Find enemy
        Character hero = GetCharacters(CharacterType.Hero)[0];
        Character target = hero.FindEnemy();
        hero.SetTarget(target);
        hero.SetDirection(target.GetTransform().position);

    }

    public void SetEnemySwnPosition(Transform[] tfms)
    {
        tfmSwnEnemyList = tfms;
    }

    public void SetHeroSwnPosition(CharacterPosition[] positions)
    {
        characterPositions = positions;
    }

    private void ChangeIndexPosSwnEnemy()
    {
        if (isIndexPosSwnEIncreasing) // increase
        {
            if (curIndexPosSwnEnemy == tfmSwnEnemyList.Length - 1) // turn back 
            {
                curIndexPosSwnEnemy = 0;
            }
            else
            {
                curIndexPosSwnEnemy++;
            }
        }
        else // decrease
        {
            if (curIndexPosSwnEnemy == 0)
            {
                curIndexPosSwnEnemy = tfmSwnEnemyList.Length - 1;
            }
            else
            {
                curIndexPosSwnEnemy--;
            }
        }
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

    public BigInteger GetGem()
    {
        gem = BigInteger.Parse(gemString);
        return gem;
    }

    public void SetGem(BigInteger remainGem)
    {
        gem = remainGem;
        uiManager.SetTextGem(gem);
    }

    private void SpawnHeroInGame()
    {
        int index = Random.Range(0, characterPositions.Length);
        Vector3 posSwnHero = characterPositions[index].tfmSwnHero.position;
        isIndexPosSwnEIncreasing = Random.Range(0, 1) == 0 ? false : true;
        curIndexPosSwnEnemy = !isIndexPosSwnEIncreasing ? characterPositions[index].indexBackSwnEnemy : characterPositions[index].indexNextSwnEnemy;
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

    public void SpawnEnemyInGame(Character prefab, int quantity, int atk, int hp)
    {
        for (int i = 0; i < quantity; i++)
        {
            Vector3 enemyPosition = new Vector3(tfmSwnEnemyList[curIndexPosSwnEnemy].position.x + i % numberOfEnemyInRow,
                 tfmSwnEnemyList[curIndexPosSwnEnemy].position.y, tfmSwnEnemyList[curIndexPosSwnEnemy].position.z + i / numberOfEnemyInRow * 1.5f);
            Character character = objectPooling.SpawnG0InPool(prefab.gameObject, enemyPosition, PoolType.Enemy).GetComponent<Character>();
            character.transform.rotation = Quaternion.identity;
            character.name = prefab.name;
            if (character.IsBoss)
                character.SetHpBar(uiManager.GetTurnBar());
            character.transform.SetParent(tfmEnemyPool);
            character.SetCharacterInfo(atk, hp);
            character.SetAttackSpeed(1);
            character.Init();
            enemyList.Add(character);
        }
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
                    BoxScreenCollision.Instance.ClearEnemyInBox();
                    if (character.IsBoss)
                    {
                        mapManager.LoadNextTurn();
                    }
                    else
                    {
                        ChangeIndexPosSwnEnemy();
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
                        cloneHero.TakeDamage(cloneHero.GetMaxHp());
                    }
                    // game lose
                    IsOver = true;
                    notifyGameOverAction();
                }
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

    public Transform GetSkillPoolTfm() => tfmSkillPool;
}

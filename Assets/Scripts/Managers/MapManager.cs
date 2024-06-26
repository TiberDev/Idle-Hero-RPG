using System.Collections;
using BigInteger = System.Numerics.BigInteger;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : Singleton<MapManager>
{
    [SerializeField] private SObjMapConfig[] mapConfigList;
    [SerializeField] private SceneLoadingUI sceneLoading;
    [SerializeField] private CloudTrasitionLoading cloudTrasitionLoading;
    [SerializeField] private BossKillWaitingEffect bossKillWaitingEffect;
    [SerializeField] private CharacterHpBar turnBar;

    private SObjMapConfig curMap;
    private MapData data;
    private Round curRound;
    private Turn curTurn;
    private Wave curWave;
    private Wave[] waveList;
    private GameManager gameManager;

    private BigInteger bossATK, bossHP;
    private BigInteger creepATK, creepHP;
    private BigInteger goldKillBoss, goldKillCreep;

    private int indexWave;
    private int curNumberOfWaves;
    private bool waveIndexIncreasing;
    private bool loopWave;

    private readonly string DATAKEY = "MAPDATA";

    public Wave[] WaveList { get => waveList; }

    private void SaveData()
    {
        PlayerPrefs.SetString(DATAKEY, JsonUtility.ToJson(data));
    }

    public void LoadMapData()
    {
        // Reference
        gameManager = GameManager.Instance;
        PlayerPrefs.DeleteKey(DATAKEY);
        string json = PlayerPrefs.GetString(DATAKEY, null);
        if (json == null || json == "") // new user
        {
            data = new MapData()
            {
                map = 1,
                round = 1,
                turn = 1,
                bossATK = mapConfigList[0].baseATKBoss.ToString(),
                bossHP = mapConfigList[0].baseHPBoss.ToString(),
                creepATK = mapConfigList[0].baseATKCreep.ToString(),
                creepHP = mapConfigList[0].baseHPCreep.ToString(),
                goldKillBoss = mapConfigList[0].goldKillBoss.ToString(),
                goldKillCreep = mapConfigList[0].goldKillCreep.ToString(),
                loopWave = false
            };
        }
        else
        {
            data = JsonUtility.FromJson<MapData>(json);
        }
        indexWave = -1;

        curMap = mapConfigList[data.map - 1];
        curRound = curMap.roundList[data.round - 1];
        curTurn = curRound.turnList[data.turn - 1];
        loopWave = data.loopWave;
        if (loopWave)
            bossKillWaitingEffect.Init();

        bossATK = BigInteger.Parse(data.bossATK);
        bossHP = BigInteger.Parse(data.bossHP);
        creepATK = BigInteger.Parse(data.creepATK);
        creepHP = BigInteger.Parse(data.creepHP);
        goldKillBoss = BigInteger.Parse(data.goldKillBoss);
        goldKillCreep = BigInteger.Parse(data.goldKillCreep);
    }

    private IEnumerator IEStartNewMap()
    {
        //indexRound = 0; indexTurn = 0;
        var async = SceneManager.LoadSceneAsync($"Map{data.map}");
        yield return new WaitUntil(() => async.isDone);
        sceneLoading.EndLoading();
    }

    private void LoadNextMap()
    {
        if (mapConfigList.Length > data.map)
        {
            data.map += 1;
            curMap = mapConfigList[data.map];

            bossATK = curMap.baseATKBoss;
            bossHP = curMap.baseHPBoss;

            creepATK = curMap.baseATKCreep;
            creepHP = curMap.baseHPCreep;

            goldKillBoss = curMap.goldKillBoss;
            goldKillCreep = curMap.goldKillCreep;
        }
        else // loop current map
        {
            // Get final value in last round and last turn 
            bossATK = CaculateValue(bossATK, curRound.increaseBaseATKPercentage + curTurn.increaseATKPercentage);
            bossHP = CaculateValue(bossHP, curRound.increaseBaseHPPercentage + curTurn.increaseHPPercentage);

            creepATK = CaculateValue(creepATK, curRound.increaseBaseHPPercentage + curTurn.increaseHPPercentage);
            creepHP = CaculateValue(creepHP, curRound.increaseBaseHPPercentage + curTurn.increaseHPPercentage);

            goldKillBoss = CaculateValue(goldKillBoss, curRound.increaseGoldPercentage + curTurn.increaseGoldPercentage);
            goldKillCreep = CaculateValue(goldKillCreep, curRound.increaseGoldPercentage + curTurn.increaseGoldPercentage);
        }
        data.round = 1;
        data.turn = 1;
        data.bossATK = bossATK.ToString();
        data.bossHP = bossHP.ToString();
        data.creepATK = creepATK.ToString();
        data.creepHP = creepHP.ToString();
        data.goldKillBoss = goldKillBoss.ToString();
        data.goldKillCreep = goldKillCreep.ToString();

        curRound = curMap.roundList[data.round - 1];
        curTurn = curRound.turnList[data.turn - 1];
        SaveData();
        // scene trasition effect
        sceneLoading.StartLoading(() => StartCoroutine(IEStartNewMap()));
    }


    private void LoadNextRound()
    {
        if (curMap.roundList.Length <= data.round)
        {
            // next map
            LoadNextMap();
        }
        else
        {
            data.turn = 0;
            data.round += 1;
            SaveData();

            curRound = curMap.roundList[data.round - 1];
            // change time
            //
            LoadNextTurn();
        }
    }

    public void LoadNextTurn()
    {
        if (curRound.turnList.Length <= data.turn)
        {
            // next round
            LoadNextRound();
        }
        else
        {
            data.turn += 1;
            SaveData();

            curTurn = curRound.turnList[data.turn - 1];
            // moving cloud effect
            cloudTrasitionLoading.MoveCloud(() => gameManager.RenewGameState(true));
        }
    }

    private void SpawnBossInTurn()
    {
        BigInteger atk = CaculateValue(bossATK, curRound.increaseBaseATKPercentage + curTurn.increaseATKPercentage);
        BigInteger hp = CaculateValue(bossHP, curRound.increaseBaseHPPercentage + curTurn.increaseHPPercentage);
        float x = (curWave.leftRange + curWave.rightRange) / 2;
        float z = (curWave.topRange + curWave.bottomRange) / 2;
        gameManager.SpawnEnemyInGame(curTurn.boss, new Vector3(x, curWave.swnYPosition, z), atk, hp);
        if (loopWave)
            gameManager.RenewInRevengeBossStage();
    }

    private void SpawnEnemyInWave()
    {
        BigInteger atk = CaculateValue(creepATK, curRound.increaseBaseATKPercentage + curTurn.increaseATKPercentage);
        BigInteger hp = CaculateValue(creepHP, curRound.increaseBaseHPPercentage + curTurn.increaseHPPercentage);
        for (int indexList = 0; indexList < curWave.enemyTypesInWave.Length; indexList++)
        {
            EnemyTypeInWave enemyTypeInWave = curWave.enemyTypesInWave[indexList];
            for (int i = 0; i < enemyTypeInWave.amount; i++)
            {
                float x = Random.Range(curWave.leftRange, curWave.rightRange);
                float z = Random.Range(curWave.topRange, curWave.bottomRange);
                gameManager.SpawnEnemyInGame(enemyTypeInWave.prefab, new Vector3(x, curWave.swnYPosition, z), atk, hp);
            }
        }
    }

    private BigInteger CaculateValue(BigInteger baseValue, BigInteger maxPercent)
    {
        return baseValue + baseValue * maxPercent / 100;
    }

    /// <summary>
    /// Spawn enemies or a boss in wave
    /// </summary>
    public void LoadNextWave(bool inBossStage = false)
    {
        if (curTurn.numberOfWaves <= curNumberOfWaves || inBossStage)
        {
            curNumberOfWaves = 0;
            turnBar.SetFillAmountUI(curTurn.numberOfWaves, curTurn.numberOfWaves, true);
            ChangeWaveIndex();
            // spawn boss when all wave is gone
            SpawnBossInTurn();
        }
        else
        {
            turnBar.SetTextInfo(data.round, data.turn);
            turnBar.SetSize(false);
            turnBar.SetTurnBarColor(false);
            turnBar.SetTextTurnBar(false);
            if (!loopWave)
            {
                curNumberOfWaves++;
                turnBar.SetFillAmountUI(curNumberOfWaves - 1, curTurn.numberOfWaves, curNumberOfWaves > 1 ? true : false);
            }
            else
                turnBar.SetFillAmountUI(curTurn.numberOfWaves, curTurn.numberOfWaves, false);
            ChangeWaveIndex();
            SpawnEnemyInWave();
        }
    }

    private void ChangeWaveIndex()
    {
        if (waveIndexIncreasing) // increase
        {
            if (indexWave == waveList.Length - 1) // turn back 
            {
                indexWave = 0;
            }
            else
            {
                indexWave++;
            }
        }
        else // decrease
        {
            if (indexWave == 0)
            {
                indexWave = waveList.Length - 1;
            }
            else
            {
                indexWave--;
            }
        }
        curWave = waveList[indexWave];
    }

    public void SetFirstWaveIndex(int index, bool increasing)
    {
        indexWave = index;
        waveIndexIncreasing = !increasing;
        ChangeWaveIndex();// method LoadNextWave will call this method one more 
        waveIndexIncreasing = increasing;
    }

    public void SetWaveList(Wave[] waves)
    {
        waveList = waves;
    }

    public void ResetWave()
    {
        if (loopWave)
            bossKillWaitingEffect.Init();

        curNumberOfWaves = 0;
        LoadNextWave();
    }

    /// <summary>
    /// Loop wave until user wants to attack boss again
    /// </summary>
    public void LoopWave(bool loop)
    {
        data.loopWave = loop;
        loopWave = loop;
        SaveData();
    }

    public MapData GetMapData() => data;

    public BigInteger GetGoldKillEnemy(bool boss)
    {
        return boss ? goldKillBoss : goldKillCreep;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    [SerializeField] private SObjMapConfig[] mapConfigList;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private SceneLoadingUI sceneLoading;

    private SObjMapConfig curMap;
    private MapData data;
    private Wave curWave;
    private Wave[] waveList;
    private GameManager gameManager;

    private int indexMap, indexRound, indexTurn, indexWave;
    private int curNumberOfWaves;
    private bool waveIndexIncreasing;

    public void LoadMapData()
    {
        gameManager = GameManager.Instance;
        string json = PlayerPrefs.GetString("MAPDATA", null);
        if (json == null || json == "") // new user
        {
            data = new MapData()
            {
                map = 1,
                round = 1,
                turn = 1,
            };
        }
        else
        {
            data = JsonUtility.FromJson<MapData>(json);
        }
        indexMap = data.map - 1;
        indexRound = data.round - 1;
        indexTurn = data.turn - 1;
        indexWave = -1;
        curMap = mapConfigList[indexMap];
    }


    private IEnumerator IEStartNewMap()
    {
        indexRound = 0; indexTurn = 0;
        var async = SceneManager.LoadSceneAsync($"Map{indexMap + 1}");
        yield return new WaitUntil(() => async.isDone);
        sceneLoading.EndLoading();
    }

    private void LoadNextMap()
    {
        if (mapConfigList.Length > indexMap + 1)
        {
            indexMap++;
            curMap = mapConfigList[indexMap];
        }
        // scene trasition effect
        sceneLoading.StartLoading(() => StartCoroutine(IEStartNewMap()));
    }

    private void LoadNextRound()
    {
        if (curMap.roundList.Length <= indexRound + 1)
        {
            indexRound = -1;
            // next map
            LoadNextMap();
        }
        else
        {
            indexRound++;
            indexTurn = -1;
            // change time
            //
            LoadNextTurn();
            //SceneManager.LoadScene($"Map{indexMap + 1}_Round{indexRound + 1}");
        }
    }

    public void LoadNextTurn()
    {
        if (curMap.roundList[indexRound].turnList.Length <= indexTurn + 1)
        {
            indexTurn = -1;
            // next round
            LoadNextRound();
        }
        else
        {
            indexTurn++;
            gameManager.RenewGameState(true);
            //LoadNextWave();
        }
    }

    private void SpawnBossInTurn()
    {
        int atk = CaculateValue(curMap.baseATKBoss, curMap.roundList[indexRound].increaseBaseATKPercentage + curMap.roundList[indexRound].turnList[indexTurn].increaseATKPercentage);
        int hp = CaculateValue(curMap.baseHPBoss, curMap.roundList[indexRound].increaseBaseHPPercentage + curMap.roundList[indexRound].turnList[indexTurn].increaseHPPercentage);
        gameManager.SpawnEnemyInGame(curMap.roundList[indexRound].turnList[indexTurn].boss, curWave.enemyTypesInWave[0].tfmSwnPositionList[0].position, atk, hp);
        //CharacterHpBar bar = uiManager.GetTurnBar();
        //bar.SetTextInfo();
    }

    private void SpawnEnemyInWave()
    {
        int atk = CaculateValue(curMap.baseATKCreep, curMap.roundList[indexRound].increaseBaseATKPercentage + curMap.roundList[indexRound].turnList[indexTurn].increaseATKPercentage);
        int hp = CaculateValue(curMap.baseHPCreep, curMap.roundList[indexRound].increaseBaseHPPercentage + curMap.roundList[indexRound].turnList[indexTurn].increaseHPPercentage);
        for (int indexList = 0; indexList < curWave.enemyTypesInWave.Length; indexList++)
        {
            EnemyTypeInWave enemyTypeInWave = curWave.enemyTypesInWave[indexList];
            for (int i = 0; i < enemyTypeInWave.tfmSwnPositionList.Length; i++)
            {
                gameManager.SpawnEnemyInGame(enemyTypeInWave.prefab, enemyTypeInWave.tfmSwnPositionList[i].position, atk, hp);
                break;
            }
            break;
        }
        CharacterHpBar bar = uiManager.GetTurnBar();
        bar.SetSize(false);
        bar.SetTurnBarColor(false);
        bar.SetTextTurnBar(false);
        bar.SetTextInfo(indexRound + 1, indexTurn + 1);
        bar.SetHpUI(curNumberOfWaves, curMap.roundList[indexRound].turnList[indexTurn].numberOfWaves, false);
    }

    private int CaculateValue(int baseValue, int maxPercent)
    {
        return baseValue + maxPercent * baseValue;
    }

    /// <summary>
    /// Spawn enemies or a boss in wave
    /// </summary>
    public void LoadNextWave()
    {
        if (curMap.roundList[indexRound].turnList[indexTurn].numberOfWaves <= curNumberOfWaves)
        {
            curNumberOfWaves = 0;
            ChangeWaveIndex();
            // spawn boss when all wave is gone
            SpawnBossInTurn();
            //indexWave = -1;
        }
        else
        {
            curNumberOfWaves++;
            ChangeWaveIndex();
            SpawnEnemyInWave();
        }
        data.map = indexMap + 1;
        data.round = indexRound + 1;
        data.turn = indexTurn + 1;
        PlayerPrefs.SetString("MAPDATA", JsonUtility.ToJson(data));
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
        curNumberOfWaves = 0;
        LoadNextWave();
    }

    public MapData GetMapData() => data;
}

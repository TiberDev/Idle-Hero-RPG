using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    [SerializeField] private SObjMapConfig[] mapConfigList;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private SkillTable skillTable;

    private SObjMapConfig curMap;
    private MapData data;
    private Wave curWave;
    private GameManager gameManager;

    private int indexMap, indexRound, indexTurn, indexWave;

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

    private void LoadNextMap()
    {
        if (mapConfigList.Length > indexMap + 1)
        {
            indexMap++;
        }
        // scene trasition effect
        indexRound = 0; indexTurn = 0;
        SceneManager.LoadScene($"Map{indexMap + 1}_Round{indexRound + 1}");
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
            indexTurn = 0;
            SceneManager.LoadScene($"Map{indexMap + 1}_Round{indexRound + 1}");
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
            LoadNextWave();
        }
    }

    private void SpawnBossInTurn()
    {
        int atk = CaculateValue(curMap.baseATKBoss, curMap.roundList[indexRound].increaseBaseATKPercentage + curMap.roundList[indexRound].turnList[indexTurn].increaseATKPercentage);
        int hp = CaculateValue(curMap.baseHPBoss, curMap.roundList[indexRound].increaseBaseHPPercentage + curMap.roundList[indexRound].turnList[indexTurn].increaseHPPercentage);
        gameManager.SpawnEnemyInGame(curMap.roundList[indexRound].turnList[indexTurn].boss, 1, atk, hp);
        CharacterHpBar bar = uiManager.GetTurnBar();
        bar.SetTextInfo();
    }

    private void SpawnEnemyInWave()
    {

        int atk = CaculateValue(curMap.baseATKCreep, curMap.roundList[indexRound].increaseBaseATKPercentage + curMap.roundList[indexRound].turnList[indexTurn].increaseATKPercentage);
        int hp = CaculateValue(curMap.baseHPCreep, curMap.roundList[indexRound].increaseBaseHPPercentage + curMap.roundList[indexRound].turnList[indexTurn].increaseHPPercentage);
        for (int indexList = 0; indexList < curWave.enemies.Length; indexList++)
        {
            gameManager.SpawnEnemyInGame(curWave.enemies[indexList].prefab, curWave.enemies[indexList].quantity, atk, hp);
        }
        CharacterHpBar bar = uiManager.GetTurnBar();
        bar.SetTextInfo(indexRound + 1, indexTurn + 1);
        bar.SetHpUI(indexWave + 1, curMap.roundList[indexRound].turnList[indexTurn].waveList.Length, false);
    }

    private int CaculateValue(int baseValue, int maxPercent)
    {
        return baseValue + maxPercent * baseValue;
    }

    public void LoadNextWave()
    {
        if (curMap.roundList[indexRound].turnList[indexTurn].waveList.Length <= indexWave + 1)
        {
            indexWave = -1;
            // spawn boss when all wave is gone
            SpawnBossInTurn();
        }
        else
        {
            indexWave++;
            curWave = curMap.roundList[indexRound].turnList[indexTurn].waveList[indexWave];
            SpawnEnemyInWave();
        }
        data.map = indexMap + 1;
        data.round = indexRound + 1;
        data.turn = indexTurn + 1;
        PlayerPrefs.SetString("MAPDATA", JsonUtility.ToJson(data));
    }

    public void ReloadWave()
    {
        indexWave = -1;
        LoadNextWave();
    }

    public MapData GetMapData() => data;
}

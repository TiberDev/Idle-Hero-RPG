#if UNITY_EDITOR
using UnityEngine;

public class HeroStatsTool : MonoBehaviour
{
    [SerializeField] private SObjHeroStatConfig[] heroStatConfigs;

    private readonly string DATAKEY = "HEROSTATSLISTDATA";

    [ContextMenu("Clear Data")]
    public void ClearData()
    {
        PlayerPrefs.DeleteKey(DATAKEY);
    }


    [ContextMenuItem("Set Level For Hero", "SetLevel")]
    public int level;
    public SObjHeroStatConfig heroConfig;
    public void SetLevel()
    {
        var json = PlayerPrefs.GetString(DATAKEY, null);
        HeroStatsList heroStatsList = JsonUtility.FromJson<HeroStatsList>(json);
        if (heroStatsList == null || level < 0 || level > 100)
            return;

        for (int i = 0; i < heroStatsList.list.Count; i++)
        {
            HeroStats heroStats = heroStatsList.list[i];
            if (heroConfig.name == heroStats.name)
            {
                heroStats.level = level;
                heroStats.numberOfPoints = 0;
                heroStats.totalPoint = (heroStatConfigs[numberOfSpawn].pointPerLv + level) / 2;
            }
        }
        PlayerPrefs.SetString(DATAKEY, JsonUtility.ToJson(heroStatsList));
    }

    [ContextMenuItem("Unlock Heroes", "UnlockHeroes")]
    public int numberOfSpawn;
    public void UnlockHeroes()
    {
        if (numberOfSpawn < 1 || numberOfSpawn > heroStatConfigs.Length)
            return;

        var json = PlayerPrefs.GetString(DATAKEY, null);
        HeroStatsList heroStatsList = JsonUtility.FromJson<HeroStatsList>(json);
        if (heroStatsList == null)
        {
            heroStatsList = new HeroStatsList();
            HeroStats heroStats = new HeroStats()
            {
                name = heroStatConfigs[0].heroName,
                level = 1,
                numberOfPoints = 0,
                totalPoint = heroStatConfigs[0].pointPerLv,
                inUse = true,
                unblocked = true,
                position = 1
            };
            heroStatsList.list.Add(heroStats);
        }

        if (numberOfSpawn > heroStatsList.list.Count)
        {
            while (heroStatsList.list.Count < numberOfSpawn)
            {
                int index = heroStatsList.list.Count;
                HeroStats heroStats = new HeroStats()
                {
                    name = heroStatConfigs[index].heroName,
                    level = 1,
                    numberOfPoints = 0,
                    totalPoint = heroStatConfigs[index].pointPerLv,
                    inUse = numberOfSpawn == 1,
                    unblocked = true,
                    position = index + 1
                };
                heroStatsList.list.Add(heroStats);
            }
        }
        else if (numberOfSpawn < heroStatsList.list.Count)
        {
            int index = heroStatsList.list.Count - 1;
            while (heroStatsList.list.Count > numberOfSpawn)
            {
                if (heroStatsList.list[index].inUse)
                {
                    heroStatsList.list[0].inUse = true;
                }
                heroStatsList.list.RemoveAt(index);
            }
        }
        PlayerPrefs.SetString(DATAKEY, JsonUtility.ToJson(heroStatsList));
    }

    [ContextMenu("Unlock All Heroes")]
    public void UnlockAllHeroes()
    {
        var json = PlayerPrefs.GetString(DATAKEY, null);
        HeroStatsList heroStatsList = JsonUtility.FromJson<HeroStatsList>(json);
        if (heroStatsList == null)
            heroStatsList = new HeroStatsList();
        bool inUseExisting = false;
        for (int index = 0; index < heroStatConfigs.Length; index++)
        {
            HeroStats heroStats = null;
            if (index < heroStatsList.list.Count) // heroes that user owned
            {
                heroStats = heroStatsList.list[index];
                if (!inUseExisting)
                    inUseExisting = heroStats.inUse;
            }
            else
            {
                heroStats = new HeroStats()
                {
                    name = heroStatConfigs[index].heroName,
                    level = 1,
                    numberOfPoints = 0,
                    totalPoint = heroStatConfigs[index].pointPerLv,
                    inUse = false,
                    unblocked = true,
                    position = index + 1
                };
                if (!inUseExisting)
                {
                    heroStats.inUse = true;
                    inUseExisting = true;
                }
                heroStatsList.list.Add(heroStats);
            }
        }
        PlayerPrefs.SetString(DATAKEY, JsonUtility.ToJson(heroStatsList));
    }
}
#endif
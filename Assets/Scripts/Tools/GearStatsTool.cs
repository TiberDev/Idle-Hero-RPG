using UnityEditor;
using UnityEngine;

[System.Serializable]
public class GearModeTool
{
    public GearMode gearMode;
    public int maxPercentLevel;
    public int pointPerLv;
    [Space(10)]
    public int fromFirstOwnedEffect; // random
    public int toFirstOwnedEffect;
    [Space(10)]
    public int fromEquippedEffect; // random
    public int toEquippedEffect;
}

public class GearStatsTool : MonoBehaviour
{
    public GearModeTool[] gearModeTools;
    public Sprite[] weaponSpts, armorSpts;
    public GearConfig[] gearConfigs;

    public int levelMax;
    public GearType type;

    private readonly string DATAKEY = "GEARSTATSLISTDATA";

    private GearModeTool GetGearModeTool(int number)
    {
        switch (number)
        {
            case <= 15:
                return gearModeTools[0]; // common
            case <= 25:
                return gearModeTools[1]; // rare
            case <= 32:
                return gearModeTools[2]; // epic
            default:
                return gearModeTools[3]; // legendary
        }
    }

    private string InsertSpace(string name)
    {
        string result = string.Empty;

        for (int i = 0; i < name.Length; i++)
        {
            if (char.IsUpper(name[i]) && i != 0)
            {
                result += " " + name[i];
            }
            else
            {
                result += name[i].ToString();
            }
        }
        return result;
    }

    [ContextMenu("Fill data in SObj")]
    public void FillInSObj()
    {
        Sprite[] sprites = type == GearType.Weapon ? weaponSpts : armorSpts;
        SObjGearsStatsConfig[] configs = gearConfigs[(int)type].gearsStatsConfigs;
        GearModeTool gearModeTool;
        for (int i = 0; i < configs.Length; i++)
        {
#if UNITY_EDITOR
            // rename file
            string assetPath = $"Assets/ScriptableObjects/GearsStatsConfig/{type}s/{configs[i].name}.asset";
            AssetDatabase.RenameAsset(assetPath, sprites[i].name);
#endif
            configs[i].gearName = InsertSpace(sprites[i].name);
            configs[i].levelMax = levelMax;

            gearModeTool = GetGearModeTool(i + 1);
            configs[i].maxPercentLevel = gearModeTool.maxPercentLevel;
            configs[i].pointPerLv = gearModeTool.pointPerLv;
            configs[i].firstOwnedEffect = Random.Range(gearModeTool.fromFirstOwnedEffect, gearModeTool.toFirstOwnedEffect + 1);
            configs[i].firstEquippedEffect = Random.Range(gearModeTool.fromEquippedEffect, gearModeTool.toEquippedEffect + 1);
            configs[i].gearSpt = sprites[i];
            configs[i].type = type;
            configs[i].mode = gearModeTool.gearMode;
            configs[i].name = sprites[i].name;
        }
    }

    [ContextMenuItem("Set Point", "SetPoint")]
    public int numberOfPoints;
    public void SetPoint(GearType type)
    {
        var json = PlayerPrefs.GetString(type + DATAKEY, null);
        GearStatsList gearStatsList = JsonUtility.FromJson<GearStatsList>(json);
        if (numberOfPoints < 0 || gearStatsList == null)
            return;

        for (int i = 0; i < gearStatsList.list.Count; i++)
        {
            GearStats gearStats = gearStatsList.list[i];
            gearStats.numberOfPoints = numberOfPoints;
        }
        PlayerPrefs.SetString(type + DATAKEY, JsonUtility.ToJson(gearStatsList));
    }

    [ContextMenu("Reset Level")]
    public void ResetLevel()
    {
        var json = PlayerPrefs.GetString(type + DATAKEY, null);
        GearStatsList gearStatsList = JsonUtility.FromJson<GearStatsList>(json);

        if (numberOfPoints < 0 || gearStatsList == null)
            return;
        SObjGearsStatsConfig[] configs = gearConfigs[(int)type].gearsStatsConfigs;

        for (int i = 0; i < gearStatsList.list.Count; i++)
        {
            GearStats gearStats = gearStatsList.list[i];
            gearStats.level = 1;
            gearStats.numberOfPoints = 1;

            int index = gearStats.position - 1;
            gearStats.totalPoint = configs[index].pointPerLv + (1 * configs[index].maxPercentLevel / 100);
            gearStats.ownedEffect = configs[index].firstOwnedEffect;
            gearStats.equippedEffect = configs[index].firstEquippedEffect;

        }
        PlayerPrefs.SetString(type + DATAKEY, JsonUtility.ToJson(gearStatsList));
    }

    [ContextMenu("Clear Data")]
    public void ClearData()
    {
        PlayerPrefs.DeleteKey(type + DATAKEY);
    }

    [ContextMenuItem("Unlock Gears", "UnlockGears")]
    public int numberOfSpawn;

    public void UnlockGears()
    {
        int typeInt = (int)type;
        SObjGearsStatsConfig[] configs = gearConfigs[typeInt].gearsStatsConfigs;
        if (numberOfSpawn < 1 || numberOfSpawn > configs.Length)
            return;

        var json = PlayerPrefs.GetString(type + DATAKEY, null);
        GearStatsList gearStatsList = JsonUtility.FromJson<GearStatsList>(json);
        if (gearStatsList == null)
        {
            gearStatsList = new GearStatsList();
            GearStats gearStats = new GearStats()
            {
                name = configs[0].gearName,
                level = 1,
                numberOfPoints = 1,
                totalPoint = configs[0].pointPerLv,
                type = configs[0].type,
                mode = configs[0].mode,
                ownedEffect = configs[0].firstOwnedEffect,
                equippedEffect = configs[0].firstEquippedEffect,
                equipped = true,
                unblocked = true,
                position = 1,
            };
            gearStatsList.list.Add(gearStats);
        }

        if (numberOfSpawn > gearStatsList.list.Count)
        {
            while (gearStatsList.list.Count < numberOfSpawn)
            {
                int index = gearStatsList.list.Count;
                GearStats gearStats = new GearStats()
                {
                    name = configs[index].gearName,
                    level = 1,
                    numberOfPoints = 1,
                    totalPoint = configs[index].pointPerLv,
                    type = configs[index].type,
                    mode = configs[index].mode,
                    ownedEffect = configs[index].firstOwnedEffect,
                    equippedEffect = configs[index].firstEquippedEffect,
                    equipped = numberOfSpawn == 1,
                    unblocked = true,
                    position = index + 1,
                };
                gearStatsList.list.Add(gearStats);
            }
        }
        else if (numberOfSpawn < gearStatsList.list.Count)
        {
            int index = gearStatsList.list.Count - 1;
            while (gearStatsList.list.Count > numberOfSpawn)
            {
                if (gearStatsList.list[index].equipped)
                {
                    gearStatsList.list[0].equipped = true;
                }
                gearStatsList.list.RemoveAt(index);
            }
        }
        PlayerPrefs.SetString(type + DATAKEY, JsonUtility.ToJson(gearStatsList));
    }

    [ContextMenu("Unlock All Gears")]
    public void UnlockAllGears(GearType type)
    {
        int typeInt = (int)type;
        SObjGearsStatsConfig[] configs = gearConfigs[typeInt].gearsStatsConfigs;
        var json = PlayerPrefs.GetString(type + DATAKEY, null);
        GearStatsList gearStatsList = JsonUtility.FromJson<GearStatsList>(json);
        if (gearStatsList == null)
            gearStatsList = new GearStatsList();
        bool equippedExisting = false;
        for (int index = 0; index < configs.Length; index++)
        {
            GearStats gearStats = null;
            if (index < gearStatsList.list.Count) // heroes that user owned
            {
                gearStats = gearStatsList.list[index];
                if (!equippedExisting)
                    equippedExisting = gearStats.equipped;
            }
            else
            {
                gearStats = new GearStats()
                {
                    name = configs[index].gearName,
                    level = 1,
                    numberOfPoints = 1,
                    totalPoint = configs[index].pointPerLv,
                    type = configs[index].type,
                    mode = configs[index].mode,
                    ownedEffect = configs[index].firstOwnedEffect,
                    equippedEffect = configs[index].firstEquippedEffect,
                    equipped = false,
                    unblocked = true,
                    position = index + 1,
                };
                if (!equippedExisting)
                {
                    gearStats.equipped = true;
                    equippedExisting = true;
                }
                gearStatsList.list.Add(gearStats);
            }
        }
        PlayerPrefs.SetString(type + DATAKEY, JsonUtility.ToJson(gearStatsList));
    }
}

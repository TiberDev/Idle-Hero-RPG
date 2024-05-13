using UnityEngine;

public class SkillStatsTool : Singleton<SkillStatsTool>
{
    [SerializeField] private SkillStatsManager skillStatsManager;

    //public SObjSkillStatsConfig[] skillStatsConfigs;

    private readonly string DATAKEY = "SKILLSTATSLISTDATA";

    //[ContextMenuItem("Set Point", "SetPoint")]
    //public int numberOfPoints;
    //public void SetPoint()
    //{
    //    var json = PlayerPrefs.GetString(DATAKEY, null);
    //    SkillStatsList skillStatsList = JsonUtility.FromJson<SkillStatsList>(json);
    //    if (numberOfPoints < 0 || skillStatsList == null)
    //        return;

    //    for (int i = 0; i < skillStatsList.list.Count; i++)
    //    {
    //        SkillStats skillStats = skillStatsList.list[i];
    //        skillStats.numberOfPoints = numberOfPoints;
    //    }
    //    PlayerPrefs.SetString(DATAKEY, JsonUtility.ToJson(skillStatsList));
    //}

    //[ContextMenu("Reset Level")]
    //public void ResetLevel()
    //{
    //    var json = PlayerPrefs.GetString(DATAKEY, null);
    //    SkillStatsList skillStatsList = JsonUtility.FromJson<SkillStatsList>(json);

    //    if (numberOfPoints < 0 || skillStatsList == null)
    //        return;

    //    for (int i = 0; i < skillStatsList.list.Count; i++)
    //    {
    //        SkillStats skillStats = skillStatsList.list[i];
    //        skillStats.level = 1;
    //        skillStats.numberOfPoints = 1;
    //        skillStats.totalPoint = skillStatsConfigs[skillStats.position - 1].pointPerLv + (1 * skillStatsConfigs[skillStats.position - 1].maxPercentLevel / 100);
    //        skillStats.ownedEffect = skillStatsConfigs[skillStats.position - 1].ownedEffect;
    //        skillStats.value = skillStatsConfigs[skillStats.position - 1].damage;
    //    }
    //    PlayerPrefs.SetString(DATAKEY, JsonUtility.ToJson(skillStatsList));
    //}

    //[ContextMenuItem("Unlock Skills", "UnlockSkills")]
    //public int numberOfSpawn;
    //public void UnlockSkills()
    //{
    //    if (numberOfSpawn < 1 || numberOfSpawn > skillStatsConfigs.Length)
    //        return;

    //    var json = PlayerPrefs.GetString(DATAKEY, null);
    //    SkillStatsList skillStatsList = JsonUtility.FromJson<SkillStatsList>(json);
    //    if (skillStatsList == null)
    //    {
    //        skillStatsList = new SkillStatsList();
    //        SkillStats skillStats = new SkillStats()
    //        {
    //            name = skillStatsConfigs[0].skillName,
    //            level = 1,
    //            numberOfPoints = 1,
    //            totalPoint = skillStatsConfigs[0].pointPerLv,
    //            ownedEffect = skillStatsConfigs[0].ownedEffect,
    //            value = skillStatsConfigs[0].damage,
    //            cooldown = skillStatsConfigs[0].cooldown,
    //            equipped = false,
    //            unblocked = false,
    //            equippedPosition = 0,
    //            position = 1
    //        };
    //        skillStatsList.list.Add(skillStats);
    //    }

    //    if (numberOfSpawn > skillStatsList.list.Count)
    //    {
    //        while (skillStatsList.list.Count < numberOfSpawn)
    //        {
    //            int index = skillStatsList.list.Count;
    //            SkillStats skillStats = new SkillStats()
    //            {
    //                name = skillStatsConfigs[index].skillName,
    //                level = 1,
    //                numberOfPoints = 1,
    //                totalPoint = skillStatsConfigs[index].pointPerLv,
    //                ownedEffect = skillStatsConfigs[index].ownedEffect,
    //                value = skillStatsConfigs[index].damage,
    //                cooldown = skillStatsConfigs[index].cooldown,
    //                equipped = false,
    //                unblocked = true,
    //                equippedPosition = 0,
    //                position = index + 1
    //            };
    //            skillStatsList.list.Add(skillStats);
    //        }
    //    }
    //    else if (numberOfSpawn < skillStatsList.list.Count)
    //    {
    //        int index = skillStatsList.list.Count - 1;
    //        while (skillStatsList.list.Count > numberOfSpawn)
    //        {
    //            if (skillStatsList.list[index].equipped)
    //            {
    //                skillStatsList.list[0].equipped = true;
    //            }
    //            skillStatsList.list.RemoveAt(index);
    //        }
    //    }
    //    PlayerPrefs.SetString(DATAKEY, JsonUtility.ToJson(skillStatsList));
    //}

    [ContextMenu("Clear Data")]
    public void ClearData()
    {
        PlayerPrefs.DeleteKey(DATAKEY);
    }

    public void OnClickUnlockAllSkills()
    {
        var json = PlayerPrefs.GetString(DATAKEY, null);
        SkillStatsList skillStatsList = skillStatsManager.GetSkillStatsList();
        SObjSkillStatsConfig[] skillStatsConfigs = skillStatsManager.GetSkillStatsConfigs();

        if (skillStatsList == null)
            skillStatsList = new SkillStatsList();

        for (int index = 0; index < skillStatsConfigs.Length; index++)
        {
            if (index >= skillStatsList.list.Count)
            {
                SkillStats skillStats = new SkillStats()
                {
                    name = skillStatsConfigs[index].skillName,
                    level = 1,
                    numberOfPoints = 1,
                    totalPoint = skillStatsConfigs[index].pointPerLv,
                    ownedEffect = skillStatsConfigs[index].ownedEffect,
                    value = skillStatsConfigs[index].damage,
                    cooldown = skillStatsConfigs[index].cooldown,
                    equipped = false,
                    unblocked = true,
                    equippedPosition = 0,
                    position = index + 1
                };
                skillStatsList.list.Add(skillStats);
            }
        }
        PlayerPrefs.SetString(DATAKEY, JsonUtility.ToJson(skillStatsList));
        skillStatsManager.SetTotalOwnedEffectValue();
        skillStatsManager.SetSkillItems();
    }

    public void SetPoints(SkillItem skillItem, SkillStats skillStats, SObjSkillStatsConfig skillStatsConfig, int point)
    {
        skillStats.numberOfPoints += point;
        SkillStatsList skillStatsList = skillStatsManager.GetSkillStatsList();
        PlayerPrefs.SetString(DATAKEY, JsonUtility.ToJson(skillStatsList));
        skillStatsManager.SetSkillInfoUI(skillStats, skillStatsConfig.skillSpt, skillItem, skillStatsConfig);
    }
}

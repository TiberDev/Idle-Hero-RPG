using System.IO;
using UnityEngine;

public class Db
{
    public static GeneralStat ReadGeneralData(string name)
    {
        var json = Resources.Load<TextAsset>("Database/GeneralStats/" + name);
        return json != null ? JsonUtility.FromJson<GeneralStat>(json.text) : null;
    }

    public static void SaveGeneralData(GeneralStat generalStat,string name)
    {
        using (StreamWriter write = new StreamWriter("Assets/Resources/Database/GeneralStats/" + name +".txt"))
        {
            string json = JsonUtility.ToJson(generalStat);
            write.WriteLine(json);
        }
    }

    public static HeroStats ReadHeroData(string name)
    {
        var json = Resources.Load("Database/Heroes/" + name) as TextAsset;
        return json != null ? JsonUtility.FromJson<HeroStats>(json.text) : null; 
    }

    public static void SaveHeroData(HeroStats heroStats, string name)
    {
        using (StreamWriter write = new StreamWriter("Assets/Resources/Database/Heroes/" + name + ".txt"))
        {
            string json = JsonUtility.ToJson(heroStats);
            write.WriteLine(json);
        }
    }

    public static SkillStats ReadHSkillData(string name)
    {
        var json = Resources.Load("Database/Skills/" + name) as TextAsset;
        return json != null ? JsonUtility.FromJson<SkillStats>(json.text) : null;
    }

    public static void SaveSkillData(SkillStats skillStats, string name)
    {
        using (StreamWriter write = new StreamWriter("Assets/Resources/Database/Skills/" + name + ".txt"))
        {
            string json = JsonUtility.ToJson(skillStats);
            write.WriteLine(json);
        }
    }

    public static GearStats ReadGearData(string name, GearType gearType)
    {
        var json = Resources.Load("Database/Gears/" + gearType + "s/" + name) as TextAsset;
        return json != null ? JsonUtility.FromJson<GearStats>(json.text) : null;
    }

    public static void SaveGearData(GearStats gearStats, string name, GearType gearType)
    {
        using (StreamWriter write = new StreamWriter("Assets/Resources/Database/Gears/" + gearType + "s/" + name + ".txt"))
        {
            string json = JsonUtility.ToJson(gearStats);
            write.WriteLine(json);
        }
    }
}

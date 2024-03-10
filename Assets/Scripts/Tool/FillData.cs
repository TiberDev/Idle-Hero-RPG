using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class FillData : Singleton<FillData>
{
    [SerializeField] private List<string> units = new List<string>();

    [SerializeField] private SObjGearsStatsConfig[] sObjGearsStatsConfig;
    [SerializeField] private SObjSkillStatsConfig[] sObjSkillStatsConfig;

    [ContextMenu("Config")]
    private void Config()
    {
        foreach (var config in sObjGearsStatsConfig)
        {
            config.gearName = config.name;
        }
    }

    [ContextMenu("Config_")]
    private void Config_()
    {
        foreach (var config in sObjSkillStatsConfig)
        {
            config.skillName = config.name;
        }
    }

    public TextAsset[] file;
    public GearType gearType;
    public int numberOfPoints, totalPoint;

    [ContextMenu("Update data")]
    private void UpdateData()
    {
        for (int i = 0; i < file.Length; i++)
        {
            SkillStats skillStats = Db.ReadHSkillData(file[i].name);
            skillStats.numberOfPoints = numberOfPoints;
            skillStats.totalPoint = totalPoint;
            skillStats.level = 1;
            skillStats.unblocked = true;
            Db.SaveSkillData(skillStats, skillStats.name);
        }
    }

    public string FormatNumber(BigInteger value)
    {
        int unitIndex = -1;
        BigInteger quotient, balanceNumber, count = 10, countDivide = 1000;
        while (value / countDivide >= 1)
        {
            unitIndex += 1;
            if (unitIndex >= units.Count)
            {
                unitIndex -= 1;
                break;
            }
            quotient = value / countDivide;
            balanceNumber = value % countDivide;
            if (countDivide >= 1000000)
                count *= 1000;
            countDivide *= 1000;
        }
        if (value < 1000)
        {
            return value.ToString();
        }
        else
        {
            BigInteger roundNumber = balanceNumber / count; // 0 -> 99
            if (roundNumber == 0)
            {
                return $"{quotient}{units[unitIndex]}";
            }
            else
            {
                if (roundNumber % 10 == 0)
                {
                    roundNumber /= 10;
                }
                return $"{quotient}.{roundNumber}{units[unitIndex]}";
            }
        }
    }

    public string s_1, s_2;
    [ContextMenu("TestFloat")]
    public void GetFloatNumberr()
    {
        BigInteger number_1, number_2;
        number_1 = BigInteger.Parse(s_1);
        number_2 = BigInteger.Parse(s_2);
        int exponential_1 = 0, exponential_2 = 0;
        while (number_1 > (BigInteger)float.MaxValue)
        {
            exponential_1++;
            number_1 /= 1000;
        }
        while (number_2 > (BigInteger)float.MaxValue)
        {
            exponential_2++;
            number_2 /= 1000;
        }
        int exponential = exponential_1 - exponential_2;
        BigInteger result = BigInteger.Pow(1000, exponential);
        if (result > (BigInteger)float.MaxValue)
            Debug.Log(0);

        float newNumber_2 = (float)number_2 / Mathf.Pow(1000, exponential);
        float finalResult = newNumber_2 / (float)number_1;
    }

    public void GetFloatNumberr(BigInteger number)
    {
        BigInteger number_1, number_2;
        number_1 = BigInteger.Parse(s_1);
        number_2 = BigInteger.Parse(s_2);
        int exponential_1 = 0, exponential_2 = 0;
        while (number_1 > (BigInteger)float.MaxValue)
        {
            exponential_1++;
            number_1 /= 1000;
        }
        while (number_2 > (BigInteger)float.MaxValue)
        {
            exponential_2++;
            number_2 /= 1000;
        }
        int exponential = exponential_1 - exponential_2;
        BigInteger result = BigInteger.Pow(1000, exponential);
        if (result > (BigInteger)float.MaxValue)
            Debug.Log(0);

        float newNumber_2 = (float)number_2 / Mathf.Pow(1000, exponential);
        float finalResult = newNumber_2 / (float)number_1;
    }


}

using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class FillData : Singleton<FillData>
{
    [SerializeField] private List<string> units = new List<string>();

    [SerializeField] private SObjGearsStatsConfig[] sObjGearsStatsConfig;
    [SerializeField] private SObjSkillStatsConfig[] sObjSkillStatsConfig;

    public GameObject[] gObjs;

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
        for (int i = 0; i < gObjs.Length; i++)
        {
            gObjs[i].name = "EnemySwnPos_" + (i + 1);
        }
    }

    [ContextMenuItem("Test Big Number", "TestBigNumber")]
    public string number;
    public void TestBigNumber()
    {
        Debug.Log(FormatNumber(BigInteger.Parse(number)));
    }

    public string FormatNumber(BigInteger value)
    {
        // 5067     
        int unitIndex = -1;
        BigInteger quotient, balanceNumber, countDivide = 1000;
        while (value / countDivide >= 1) // 1000 or more
        {
            unitIndex += 1;
            if (unitIndex >= units.Count)
            {
                unitIndex -= 1;
                break;
            }
            quotient = value / countDivide;
            balanceNumber = value % countDivide;
            balanceNumber /= countDivide / 1000;
            countDivide *= 1000;
        }
        if (value < 1000)
        {
            return value.ToString();
        }
        else
        {
            // calculate balance number to get tens unit
            if (balanceNumber == 0)
            {
                return $"{quotient}{units[unitIndex]}";
            }
            else if (balanceNumber < 100) // 000 -> 099
            {
                return $"{quotient}.0{balanceNumber / 10}{units[unitIndex]}";
            }
            else // 100 -> 999
            {
                balanceNumber /= 10; // 10 -> 99
                if (balanceNumber % 10 == 0)
                {
                    balanceNumber /= 10;
                }
                return $"{quotient}.{balanceNumber}{units[unitIndex]}";
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

using UnityEngine;
using System.Numerics;

public class NumberConverter : Singleton<NumberConverter>
{
    [SerializeField] private SObjUnitConfig[] units;

    public string FormatNumber(BigInteger value)
    {
        int unitIndex = -1;
        BigInteger quotient, balanceNumber, countDivide = 1000;
        while (value / countDivide >= 1) // 1000 or more
        {
            unitIndex += 1;
            if (unitIndex >= units.Length)
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
                return $"{quotient}{units[unitIndex].unit}";
            }
            else if (balanceNumber < 100) // 000 -> 099
            {
                return $"{quotient}.0{balanceNumber / 10}{units[unitIndex].unit}";
            }
            else // 100 -> 999
            {
                balanceNumber /= 10; // 10 -> 99
                if (balanceNumber % 10 == 0)
                {
                    balanceNumber /= 10;
                }
                return $"{quotient}.{balanceNumber}{units[unitIndex].unit}";
            }
        }
    }

}

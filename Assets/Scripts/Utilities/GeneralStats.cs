using System.Collections.Generic;

[System.Serializable]
public class GeneralStatList
{
    public List<GeneralStat> list = new List<GeneralStat>();
}


[System.Serializable]
public class GeneralStat
{
    public string name, level, stat, gold;
    public int positionIndex; // position in scroll view (only max level)
    public GeneralStatsType type;
}

public enum GeneralStatsType
{
    Attack = 0,
    MaxHp = 1,
    AttackSpeed = 2,
    HpRecovery = 3,
    CriticalHitChance = 4,
    CriticalHitDamage = 5
}


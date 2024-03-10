
[System.Serializable]
public class GeneralStat
{
    public string name, level, stat, gold;
    public GeneralStatsType type;
}

public enum GeneralStatsType
{
    Attack,
    AttackSpeed,
    MaxHp,
    HpRecovery,
    CriticalHitChance,
    CriticalHitDamage
}


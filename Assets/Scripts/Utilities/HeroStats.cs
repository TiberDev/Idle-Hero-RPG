using UnityEngine;

[System.Serializable]
public class HeroStats
{
    public string name;
    public int level;
    public int numberOfPoints, totalPoint;
    public bool inUse;
    public bool unblocked;
    public AddtionalEffect[] addtionalEffects;
}

[System.Serializable]
public class AddtionalEffect
{
    public AddtionalEffectType type;
    public int percent;
    public bool unblock;
}

[System.Serializable]
public class AddtionalEffectImage
{
    public AddtionalEffectType type;
    public Sprite sprite;
}

public enum AddtionalEffectType
{
    IncreaseATK,
    IncreaseHp,
    IncreaseCriticalHitDamage,
    IncreaseGoldObtain,
    IncreaseSkillDamage,
    IncreaseBossDamage
}

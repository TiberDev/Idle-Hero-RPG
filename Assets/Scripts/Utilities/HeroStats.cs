using System.Collections.Generic;
using UnityEngine;

[System.Serializable]   
public class HeroStatsList
{
    public List<HeroStats> list = new List<HeroStats>();
}

[System.Serializable]
public class HeroStats
{
    public string name;
    public int level;
    public int numberOfPoints, totalPoint;
    public bool inUse;
    public bool unblocked;
    public int position; // position is in hero item list
}

[System.Serializable]
public class AddtionalEffect
{
    public AddtionalEffectType type;
    public int percent;
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

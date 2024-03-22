using UnityEngine;

[System.Serializable]
public class GearStats
{
    public string name;
    public int level;
    public int numberOfPoints, totalPoint;
    public GearType type;
    public GearMode mode;
    public string ownedEffect, equippedEffect; // % damage
    public bool equipped;
    public bool unblocked;
}

[System.Serializable]
public enum GearType
{
    Weapon, Armor // increase atk and hp
}

public enum GearMode
{
    Common,
    Rare,
    Epic,
    Legendary
}

[System.Serializable]
public class GearConfig
{
    public GearType type;
    public SObjGearsStatsConfig[] gearsStatsConfigs;
}

[System.Serializable]
public class GearModeConfig
{
    public GearMode mode;
    public Sprite spt;
    public Color color;
}

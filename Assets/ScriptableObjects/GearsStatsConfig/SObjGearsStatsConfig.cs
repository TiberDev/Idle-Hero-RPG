using UnityEngine;

[CreateAssetMenu(fileName = "GearsStatsConfig", menuName = "ScriptableObjects/GearsStatsConfig")]
public class SObjGearsStatsConfig : ScriptableObject
{
    public string gearName;
    public int levelMax = 100;
    public int maxPercentLevel;
    public int pointPerLv; // total point is define by pointPerLv + (level * maxPercentLevel / 100)
    public int firstOwnedEffect, firstEquippedEffect; // next value = current value + level (maybe change later)
    public Sprite gearSpt;
    public GearType type;
    public GearMode mode;
}

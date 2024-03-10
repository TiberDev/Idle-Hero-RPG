using UnityEngine;

[CreateAssetMenu(fileName = "GearsStatsConfig", menuName = "ScriptableObjects/GearsStatsConfig")]
public class SObjGearsStatsConfig : ScriptableObject
{
    public string gearName;
    public int levelMax = 100;
    public int totalPointByXLv = 6; // total point is define by multipe current level
    public string ownedEffect, equippedEffect; // next value = current value + level (maybe change later)
    public Sprite gearSpt;
    public GearType type;
    public GearMode mode;
}

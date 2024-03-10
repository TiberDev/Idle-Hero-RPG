using UnityEngine;

[CreateAssetMenu(fileName = "SkillStatsConfig", menuName = "ScriptableObjects/SkillStatsConfig")]
public class SObjSkillStatsConfig : ScriptableObject
{
    public string skillName;
    public int cooldown;
    public int levelMax = 100;
    public int totalPointByXLv = 6; // total point is define by multipe current level
    public string damage, ownedEffect; // next value = current value + level (maybe change later)
    public Sprite skillSpt;
}

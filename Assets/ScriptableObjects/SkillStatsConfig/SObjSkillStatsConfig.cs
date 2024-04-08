using UnityEngine;

[CreateAssetMenu(fileName = "SkillStatsConfig", menuName = "ScriptableObjects/SkillStatsConfig")]
public class SObjSkillStatsConfig : ScriptableObject
{
    public string skillName;
    public string describe_1;
    public string describe_2;
    public int cooldown;
    public int levelMax = 10;
    public int maxPercentLevel;
    public int pointPerLv; // total point is define by pointPerLv + (level * maxPercentLevel / 100)
    public int damage, ownedEffect; // next value = damage level 1 + level (%)
    public Sprite skillSpt;
    public Skill prefab;
}

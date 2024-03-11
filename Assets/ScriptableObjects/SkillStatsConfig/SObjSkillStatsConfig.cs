using UnityEngine;

[CreateAssetMenu(fileName = "SkillStatsConfig", menuName = "ScriptableObjects/SkillStatsConfig")]
public class SObjSkillStatsConfig : ScriptableObject
{
    public string skillName;
    public string describe_1;
    public string describe_2;
    public int cooldown;
    public int levelMax = 10;
    public int totalPointByXLv = 6; // total point is define by multipe current level
    public int damage, ownedEffect; // next value = current value + level (maybe change later)
    public Sprite skillSpt;
    public Skill prefab;
}

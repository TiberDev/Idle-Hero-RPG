using System.Numerics;

[System.Serializable]
public class SkillStats
{
    public string name;
    public int level;
    public int numberOfPoints, totalPoint;
    public int value, ownedEffect; // % atk damage
    public int cooldown;
    public int position; // position on skillitemequipped panel 
    public bool equipped;
    public bool unblocked;
}

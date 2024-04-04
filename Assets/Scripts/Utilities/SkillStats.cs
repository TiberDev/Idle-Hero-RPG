using System.Collections.Generic;

[System.Serializable]
public class SkillStatsList
{
    public List<SkillStats> list = new List<SkillStats>();
}

[System.Serializable]
public class SkillStats
{
    public string name;
    public int level;
    public int numberOfPoints, totalPoint;
    public int value, ownedEffect; // % atk damage
    public int cooldown;
    public int equippedPosition; // position on skillitemequipped panel 
    public bool equipped;
    public bool unblocked;
    public int position; // position on item list
}

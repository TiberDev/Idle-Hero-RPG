using UnityEngine;

public class BuffSkill : Skill
{
    public override void Execute()
    {
        Debug.Log("Execute Buffskill");
        SetExistingCooldown();
    }
}

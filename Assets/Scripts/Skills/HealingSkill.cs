using UnityEngine;

public class HealingSkill : Skill
{
    public override void Execute()
    {
        Debug.Log("Execute healing skill");
        SetExistingCooldown();
    }
}

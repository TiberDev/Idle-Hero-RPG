using UnityEngine;

public class MagicShieldSkill : Skill
{
    public override void Execute()
    {
        Debug.Log("Execute Magic Shield");
        SetExistingCooldown();
    }
}

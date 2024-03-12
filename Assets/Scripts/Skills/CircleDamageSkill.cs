using UnityEngine;

public class CircleDamageSkill : Skill
{
    Character hero;
    public override void Execute()
    {
        hero = gameManager.GetCharacters(CharacterType.Hero)[0];
        SetParent(gameManager.GetSkillPoolTfm(), true);
        cachedTfm.position = hero.GetTargetPosition();
        SetExistingCooldown();
    }

    public override void EndExistence()
    {
        if (hero != null)
        {
            //hero.SetAddHpRecovery(damage, false);
            gameManager.UserInfo.hpRecovery -= value;
            hero = null;
        }
        base.EndExistence();
    }
}


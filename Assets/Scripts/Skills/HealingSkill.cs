using UnityEngine;

public class HealingSkill : Skill
{
    private Character hero;
    public override void Execute()
    {
        hero = gameManager.GetCharacters(CharacterType.Hero)[0];
        gameManager.UserInfo.hpRecovery += value;
        SetParent(hero.GetTransform(), false);
        cachedTfm.localPosition = Vector3.zero;
        SetExistingCooldown();
    }

    public override void EndExistence()
    {
        if (hero != null)
        {
            gameManager.UserInfo.hpRecovery -= value;
            hero = null;
        }
        base.EndExistence();
    }
}

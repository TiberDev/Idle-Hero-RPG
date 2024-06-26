using UnityEngine;

public class BuffSkill : Skill
{
    private Character hero;
    public override void Execute()
    {
        hero = gameManager.GetCharacters(CharacterType.Hero)[0];
        gameManager.UserInfo.bossDamage += value;
        SetParent(hero.GetTransform(), false);
        cachedTfm.localPosition = Vector3.zero;
        SetExistingCooldown();
    }

    public override void EndExistence()
    {
        if (hero != null)
        {
            gameManager.UserInfo.bossDamage -= value;
            hero = null;
        }
        base.EndExistence();
    }
}

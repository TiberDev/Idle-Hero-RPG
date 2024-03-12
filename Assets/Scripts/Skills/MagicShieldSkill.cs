using BigInteger = System.Numerics.BigInteger;
using UnityEngine;

public class MagicShieldSkill : Skill
{
    private Character hero;

    public override void Execute()
    {
        hero = gameManager.GetCharacters(CharacterType.Hero)[0];
        hero.SetShieldSkill(this);
        SetParent(hero.GetTransform(), false);
        cachedTfm.localPosition = Vector3.zero;
        SetExistingCooldown();
    }

    public override void EndExistence()
    {
        if (hero != null)
        {
            hero.SetShieldSkill(null);
            hero = null;
        }
        base.EndExistence();
    }

    public BigInteger DecreaseDamageTaken(BigInteger damage)
    {
        return damage - damage * value / 100;
    }
}

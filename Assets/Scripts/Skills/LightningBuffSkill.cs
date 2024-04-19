using BigInteger = System.Numerics.BigInteger;
using UnityEngine;

public class LightningBuffSkill : Skill
{
    private Character hero;
    private BigInteger additionalATKTemp;
    public override void Execute()
    {
        hero = gameManager.GetCharacters(CharacterType.Hero)[0];
        UserInfo userInfo = gameManager.UserInfo;
        additionalATKTemp = userInfo.atk * value / 100;
        userInfo.atk += additionalATKTemp;
        SetParent(hero.transform, false);
        cachedTfm.localPosition = Vector3.zero;
        SetExistingCooldown();
    }

    public override void EndExistence()
    {
        if (hero != null)
        {
            gameManager.UserInfo.atk -= additionalATKTemp;
            hero = null;
            additionalATKTemp = 0;
        }
        base.EndExistence();
    }
}

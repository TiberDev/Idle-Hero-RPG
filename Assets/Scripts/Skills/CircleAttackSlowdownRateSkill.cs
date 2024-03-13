using UnityEngine;

public class CircleAttackSlowdownRateSkill : Skill
{
    private Character hero;

    public override void Execute()
    {
        hero = gameManager.GetCharacters(CharacterType.Hero)[0];
        SetParent(gameManager.GetSkillPoolTfm(), true);
        cachedTfm.position = hero.GetTargetPosition();
        SetExistingCooldown();
    }

    public void HandleCollision(Character character)
    {
        character.SetAttackSpeed(value, false);
    }

    public void HandleEndCollision(Character character)
    {
        character.SetAttackSpeed(value, true);
    }
}

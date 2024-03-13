
public class CircleDamageReduction : Skill, ICharacterCollisionHandler
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
        character.SetAttack(value, false);
    }

    public void HandleEndCollision(Character character)
    {
        character.SetAttack(value, true);
    }
}

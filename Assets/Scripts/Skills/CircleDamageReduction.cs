using System.Collections.Generic;
using UnityEngine;

public class CircleDamageReduction : Skill, ICharacterCollisionHandler
{
    private Character hero;
    private List<Character> characters = new List<Character>();

    public override void Execute()
    {
        hero = gameManager.GetCharacters(CharacterType.Hero)[0];
        SetParent(gameManager.GetSkillPoolTfm(), true);
        if (!BoxScreenCollision.Instance.IsEnenmiesEmpty())
        {
            cachedTfm.position = hero.GetTargetPosition();
        }
        else
        {
            Transform tfmHero = hero.GetTransform();
            cachedTfm.position = tfmHero.position + tfmHero.forward * 2;
        }
        SetExistingCooldown();
    }

    public void HandleCollision(Character character)
    {
        characters.Add(character);
        character.SetAttack(value, false);
    }

    public void HandleEndCollision(Character character)
    {
        characters.Remove(character);
        character.SetAttack(value, true);
    }

    private void OnDisable()
    {
        // Set attack to default value when this skill is disabled
        while (characters.Count > 0)
        {
            characters[0].SetAttack(value, true);
            characters.RemoveAt(0);
        }
    }
}

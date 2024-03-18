using System.Collections.Generic;
using UnityEngine;

public class CircleDamageSkill : Skill, ICharacterCollisionHandler
{
    private List<Character> charactersInRange = new List<Character>();
    private Character hero;

    private float curTimeDamage;

    public override void Execute()
    {
        curTimeDamage = 0f;
        charactersInRange.Clear();
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

    private void Update()
    {
        curTimeDamage += Time.deltaTime;
        if (curTimeDamage >= 1)
        {
            curTimeDamage = 0;
            for (int i = 0; i < charactersInRange.Count; i++)
            {
                charactersInRange[i].TakeDamage(hero.GetDamage() * value / 100);
            }
        }
    }

    public void HandleCollision(Character character)
    {
        charactersInRange.Add(character);
    }

    public void HandleEndCollision(Character character)
    {
        charactersInRange.Remove(character);
    }
}


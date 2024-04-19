using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class CircleHighDamageSkill : Skill, ICharacterCollisionHandler
{
    [SerializeField] private float timeExplosion;

    private List<Character> charactersInRange = new List<Character>();
    private Character hero;

    public override void Execute()
    {
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
        StartCoroutine(IECountTimeExplosion());
        SetExistingCooldown();
    }

    private IEnumerator IECountTimeExplosion()
    {
        float curTime = 0;
        while (curTime < timeExplosion)
        {
            curTime += Time.deltaTime;
            yield return null;
        }
        CauseDamage();
    }

    private void CauseDamage()
    {
        BigInteger damage = hero.GetDamage() * value / 100;
        for (int i = 0; i < charactersInRange.Count; i++)
        {
            if (charactersInRange[i].isActiveAndEnabled)
                charactersInRange[i].TakeDamage(damage, DamageTakenType.Skill);
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

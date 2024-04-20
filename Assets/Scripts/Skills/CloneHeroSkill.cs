using System.Collections;
using UnityEngine;

public class CloneHeroSkill : Skill
{
    [SerializeField] private Color colorCloneHero;

    public override void Execute()
    {
        StartCoroutine(IESpawnCloneHero());
    }

    private IEnumerator IESpawnCloneHero()
    {
        UserInfo heroInfo = gameManager.UserInfo;
        UserInfo cloneHeroInfo = new UserInfo()
        {
            atk = heroInfo.atk * value / 100,
            atkSpeed = 1.5f,
            bossDamage = 100,
            criticalHitChance = 0,
            criticalHitDamage = 0,
            goldObtain = heroInfo.goldObtain,
            hp = heroInfo.hp / 2,
            hpRecovery = 0,
            skillDamage = 0,
        };
        // Get postion to spawn clone hero
        GameObject prefab = gameManager.GetHeroPrefab();
        Character hero = gameManager.GetCharacters(CharacterType.Hero)[0];
        cachedTfm.SetParent(gameManager.GetSkillPoolTfm());
        cachedTfm.position = hero.GetTransform().position + hero.GetTransform().forward * -1 + Vector3.up * 0.5f; // spawned 1 unit behind main hero 
        // clone hero spawning effect
        float spawningTimeDelay = 0.3f;
        CloneHeroEffectController.Instance.CreateSpawningEffect(cachedTfm.position - Vector3.up * 0.5f, spawningTimeDelay);
        yield return new WaitForSeconds(spawningTimeDelay);

        // Spawn clone hero
        Character cloneHero = gameManager.SpawnHero(prefab, cloneHeroInfo, cachedTfm.position, colorCloneHero);
        cloneHero.SetTarget(cloneHero.FindEnemy());
        cloneHero.GetTransform().rotation = hero.GetTransform().rotation;
        EndExistence();
        skillTableItem.SetAmountCoolDown();
    }

    public override void EndExistence()
    {
        base.EndExistence();
        StopAllCoroutines();
    }
}

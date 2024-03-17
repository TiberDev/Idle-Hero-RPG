using UnityEngine;

public class CloneHeroSkill : Skill
{
    [SerializeField] private Color colorCloneHero;

    public override void Execute()
    {
        // Get postion to spawn clone hero
        GameObject prefab = gameManager.GetHeroPrefab();
        Character hero = gameManager.GetCharacters(CharacterType.Hero)[0];
        cachedTfm.SetParent(hero.GetTransform(), false);
        cachedTfm.localPosition = Vector3.forward * -2;

        UserInfo heroInfo = gameManager.UserInfo;
        UserInfo cloneHeroInfo = new UserInfo()
        {
            atk = heroInfo.atk * value / 100,
            atkSpeed = 1,
            bossDamage = 100,
            criticalHitChance = 0,
            criticalHitDamage = 0,
            goldObtain = heroInfo.goldObtain,
            hp = heroInfo.hp / 2,
            hpRecovery = 0,
            skillDamage = 0,
        };
        // Spawn clone hero
        Character cloneHero = gameManager.SpawnHero(prefab, cloneHeroInfo, cachedTfm.position, colorCloneHero);
        cloneHero.SetTarget(cloneHero.FindEnemy());
        cloneHero.GetTransform().rotation = hero.GetTransform().rotation;
        EndExistence();
        skillTableItem.SetAmountCoolDown();
    }

}

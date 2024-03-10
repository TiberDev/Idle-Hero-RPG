using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UserInfoManager : Singleton<UserInfoManager>
{
    [SerializeField] private GeneralManager generalManager;
    [SerializeField] private HeroStatsManager heroStatsManager;
    [SerializeField] private GearsStatsManager gearsStatsManager;
    [SerializeField] private SkillStatsManager skillStatsManager;

    public UserInfo userInfo = new UserInfo();

    public void SetATK()
    {
        // base atk + max atk percentage
        BigInteger baseATK = generalManager.GetBigValue(GeneralStatsType.Attack);
        BigInteger maxATKPercent = heroStatsManager.GetEffectDamagePercent(AddtionalEffectType.IncreaseATK)
            + gearsStatsManager.GetAllOwnedEffect(GearType.Weapon) + skillStatsManager.GetAllOwnedEffect();

        Debug.Log($"base ATK: {baseATK}    addition effect: {heroStatsManager.GetEffectDamagePercent(AddtionalEffectType.IncreaseATK)}%   " +
            $"gear:  {gearsStatsManager.GetAllOwnedEffect(GearType.Weapon)}%      skill: {skillStatsManager.GetAllOwnedEffect()}%");

        userInfo.atk = baseATK + baseATK * maxATKPercent;

        List<Character> heroes = GameManager.Instance.GetCharacters(CharacterType.Hero);
        if (heroes != null & heroes.Count > 0)
            heroes[0].SetAttack();
    }

    public void SetHp()
    {
        BigInteger baseHP = generalManager.GetBigValue(GeneralStatsType.MaxHp);
        BigInteger maxHPPercent = heroStatsManager.GetEffectDamagePercent(AddtionalEffectType.IncreaseHp)
            + gearsStatsManager.GetAllOwnedEffect(GearType.Armor);

        Debug.Log($"base HP: {baseHP}    addition effect: {heroStatsManager.GetEffectDamagePercent(AddtionalEffectType.IncreaseHp)}%   " +
           $"gear:  {gearsStatsManager.GetAllOwnedEffect(GearType.Armor)}%");

        BigInteger preHP = userInfo.hp;
        userInfo.hp = baseHP + baseHP * maxHPPercent;

        List<Character> heroes = GameManager.Instance.GetCharacters(CharacterType.Hero);
        if (heroes != null & heroes.Count > 0)
            heroes[0].SetHp(userInfo.hp - preHP);
    }

    public void SetATKSpeed()
    {
        userInfo.atkSpeed = generalManager.GetSmallValue(GeneralStatsType.AttackSpeed);
        List<Character> heroes = GameManager.Instance.GetCharacters(CharacterType.Hero);
        if (heroes != null & heroes.Count > 0)
            heroes[0].SetAttackSpeed(userInfo.atkSpeed);
    }

    public void SetHPRecovery()
    {
        userInfo.hpRecovery = generalManager.GetSmallValue(GeneralStatsType.HpRecovery);
    }

    public void SetCriticalHitChance()
    {
        userInfo.criticalHitChance = generalManager.GetSmallValue(GeneralStatsType.CriticalHitChance);
    }

    public void SetCriticalHitDamage()
    {
        BigInteger baseCHD = generalManager.GetBigValue(GeneralStatsType.CriticalHitDamage);
        BigInteger percent = heroStatsManager.GetEffectDamagePercent(AddtionalEffectType.IncreaseCriticalHitDamage);
        userInfo.criticalHitDamage = baseCHD + baseCHD * percent;
    }

    public void SetBossDamage()
    {
        BigInteger baseBossDamage = 100;
        BigInteger percent = heroStatsManager.GetEffectDamagePercent(AddtionalEffectType.IncreaseBossDamage);
        userInfo.bossDamage = baseBossDamage + baseBossDamage * percent;
    }

    public void SetSkillsDamage()
    {
        BigInteger baseSkillDamage = 100;
        BigInteger percent = heroStatsManager.GetEffectDamagePercent(AddtionalEffectType.IncreaseSkillDamage);
        userInfo.skillDamage = baseSkillDamage + baseSkillDamage * percent;
    }

    public void SetGoldObtain()
    {
        BigInteger baseGoldObtain = 100;
        BigInteger percent = heroStatsManager.GetEffectDamagePercent(AddtionalEffectType.IncreaseGoldObtain);
        userInfo.goldObtain = baseGoldObtain + baseGoldObtain * percent;
    }
}


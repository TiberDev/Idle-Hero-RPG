using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroStatsManager : MonoBehaviour
{
    [SerializeField] private HeroInfoUI heroInfoUI;
    [SerializeField] private HeroItem[] heroItems;
    [SerializeField] private Button[] btnAEffectsList;
    [SerializeField] private Transform rectTfmInUse;
    [SerializeField] private SObjHeroStatConfig[] heroStatConfigs;

    public List<HeroStats> heroStatsList = new List<HeroStats>();
    public HeroStats heroInUse;
    public HeroItem heroItemSelected;
    public SObjHeroStatConfig heroStatConfigSelected, heroStatConfigInUse;
    private UserInfoManager userInfoManager;

    private void OnEnable()
    {
        userInfoManager = UserInfoManager.Instance;
        SetHeroItem();
        SetHeroInfoUI(heroInUse, Array.Find(heroStatConfigs, heroConfig => heroConfig.heroName == heroInUse.name).heroSpt);
    }

    public void LoadHeroData()
    {
        for (int i = 0; i < heroStatConfigs.Length; i++)
        {
            HeroStats heroStats = Db.ReadHeroData(heroStatConfigs[i].heroName);
            if (heroStats == null) // new user or new hero
            {
                heroStats = new HeroStats
                {
                    name = heroStatConfigs[i].heroName,
                    level = 1,
                    numberOfPoints = 0,
                    totalPoint = heroStatConfigs[i].pointPerLv,
                    inUse = i <= 0,
                    unblocked = i <= 0,
                    addtionalEffects = heroStatConfigs[i].addtionalEffects
                };
                Db.SaveHeroData(heroStats, heroStats.name);
            }
            if (heroStats.inUse)
            {
                heroStatConfigInUse = heroStatConfigs[i];
                heroInUse = heroStats;
            }
            heroStatsList.Add(heroStats);
        }
    }

    private void SetHeroItem()
    {
        for (int index = 0; index < heroStatConfigs.Length; index++)
        {
            heroItems[index].Init(this, heroStatsList[index], heroStatConfigs[index]);
            heroItems[index].SetHeroImage(heroStatConfigs[index].heroSpt);
            heroItems[index].SetUnblockedHero();
            if (heroInUse == heroStatsList[index])
            {
                SetHeroItemSelected(heroItems[index], heroStatConfigs[index]);
                SetHeroItemInUse(heroInUse,false);
            }
        }
    }

    public void SetHeroItemSelected(HeroItem item, SObjHeroStatConfig config)
    {
        heroItemSelected = item;
        heroStatConfigSelected = config;
    }

    /// <summary>
    /// Display info of hero on UI
    /// </summary>
    public void SetHeroInfoUI(HeroStats heroStats, Sprite spt)
    {
        heroInfoUI.Init(heroStats, this, heroStatConfigSelected);
        heroInfoUI.SetHeroAvt(spt);
        heroInfoUI.SetTextName(heroStats.name);
        heroInfoUI.SetTextLevel(heroStats.level.ToString());
        if (heroStats.level == heroStatConfigSelected.levelMax)
        {
            heroInfoUI.SetHeroPointUI();
            heroInfoUI.SetEnhanceMaxUI();
        }
        else
        {
            heroInfoUI.SetHeroPointUI(heroStats.numberOfPoints, heroStats.totalPoint);
            heroInfoUI.SetEnhanceUI();
        }
        heroInfoUI.SetInUseUI(heroStats.inUse, heroStats.unblocked);
        heroInfoUI.SetAddtionalEffects();
        heroInfoUI.CheckEffectUnBlock();
    }

    public void SetHeroItemInUse(HeroStats newHeroInUse,bool pressed)
    {
        if (heroInUse != newHeroInUse)
        {
            heroInUse.inUse = false;
            Db.SaveHeroData(heroInUse, heroInUse.name);
            heroStatConfigInUse = heroStatConfigSelected;
            heroInUse = newHeroInUse;
            Db.SaveHeroData(heroInUse, heroInUse.name);
        }
        rectTfmInUse.SetParent(heroItemSelected.transform, false);
        if (pressed)
        {
            SetUserInfo(heroInUse);
            // Reload scene to get new hero
            GameManager.Instance.ResetMap();
        }
    }

    public void SetUserInfo(HeroStats heroStats)
    {
        AddtionalEffectType type = AddtionalEffectType.IncreaseATK;
        for (int i = 0; i < heroStats.addtionalEffects.Length; i++)
        {
            if (heroInUse.addtionalEffects[i].unblock)
            {
                type = heroInUse.addtionalEffects[i].type;
            }
            switch (type)
            {
                case AddtionalEffectType.IncreaseATK:
                    userInfoManager.SetATK();
                    break;
                case AddtionalEffectType.IncreaseHp:
                    userInfoManager.SetHp();
                    break;
                case AddtionalEffectType.IncreaseCriticalHitDamage:
                    userInfoManager.SetCriticalHitDamage();
                    break;
                case AddtionalEffectType.IncreaseGoldObtain:
                    userInfoManager.SetGoldObtain();
                    break;
                case AddtionalEffectType.IncreaseSkillDamage:
                    userInfoManager.SetSkillsDamage();
                    break;
                case AddtionalEffectType.IncreaseBossDamage:
                    userInfoManager.SetBossDamage();
                    break;
            }
        }
    }

    public Character GetPrefabHero()
    {
        return heroStatConfigInUse.prefabHero;
    }

    public int GetEffectDamagePercent(AddtionalEffectType type)
    {
        int damagePercent = 0;
        for (int i = 0; i < heroInUse.addtionalEffects.Length; i++)
        {
            if (heroInUse.addtionalEffects[i].type == type && heroInUse.addtionalEffects[i].unblock)
            {
                damagePercent += heroInUse.addtionalEffects[i].percent;
            }
        }
        return damagePercent;
    }
}
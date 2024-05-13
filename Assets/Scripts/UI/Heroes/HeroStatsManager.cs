using System;
using System.Collections.Generic;
using UnityEngine;

public class HeroStatsManager : MonoBehaviour, IBottomTabHandler
{
    [SerializeField] private RectTransform rectTfm;
    [SerializeField] private GameObject gObj;
    [SerializeField] private HeroInfoUI heroInfoUI;
    [SerializeField] private DarkBoardLoadingUI darkBoardLoading;
    [SerializeField] private BossStage bossStage;
    [SerializeField] private Transform rectTfmInUse;
    [SerializeField] private SObjHeroStatConfig[] heroStatConfigs;
    [SerializeField] private HeroItem heroItemPrefab;
    [SerializeField] private Transform tfmHeroItemParent;

    [SerializeField] private float movingTime;

    private List<HeroItem> heroItems = new List<HeroItem>();
    private HeroStatsList heroStatsList = null;
    private HeroStats heroInUse;
    private HeroItem heroItemSelected;
    private SObjHeroStatConfig heroStatConfigSelected, heroStatConfigInUse;

    private readonly string DATAKEY = "HEROSTATSLISTDATA";

    private void OnEnable()
    {
        SetHeroItems();
        SetHeroInfoUI(heroInUse, Array.Find(heroStatConfigs, heroConfig => heroConfig.heroName == heroInUse.name).heroSpt);
    }

    public void SaveData()
    {
        PlayerPrefs.SetString(DATAKEY, JsonUtility.ToJson(heroStatsList));
    }

    public void LoadHeroesData()
    {
        var json = PlayerPrefs.GetString(DATAKEY, null);
        heroStatsList = JsonUtility.FromJson<HeroStatsList>(json);
        if (heroStatsList == null) // new user 
        {
            heroStatsList = new HeroStatsList();
            HeroStats heroStats = new HeroStats
            {
                name = heroStatConfigs[0].heroName,
                level = 1,
                numberOfPoints = 0,
                totalPoint = (heroStatConfigs[0].pointPerLv + 1) / 2,
                inUse = true,
                unblocked = true,
                position = 1,
            };
            heroStatConfigInUse = heroStatConfigs[0];
            heroInUse = heroStats;
            heroStatsList.list.Add(heroStats);
            SaveData();
        }
        else // heroes that user owned
        {
            for (int i = 0; i < heroStatsList.list.Count; i++)
            {
                HeroStats heroStats = heroStatsList.list[i];
                if (heroStats.inUse)
                {
                    heroStatConfigInUse = heroStatConfigs[i];
                    heroInUse = heroStats;
                }
            }
        }
    }

    public void SetHeroItems()
    {
        int statsListIndex = 0;
        for (int index = 0; index < heroStatConfigs.Length; index++)
        {
            if (heroItems.Count < index + 1) // spawn skill item if not spawned yet
                heroItems.Add(Instantiate(heroItemPrefab, tfmHeroItemParent));

            HeroStats heroStats = null;
            if (statsListIndex < heroStatsList.list.Count)
            {
                if (index + 1 == heroStatsList.list[statsListIndex].position) // hero owned is in hero item list
                {
                    heroStats = heroStatsList.list[statsListIndex];
                    statsListIndex++;
                }
            }

            if (heroStats == null)
            {
                heroStats = new HeroStats()
                {
                    name = heroStatConfigs[index].heroName,
                    level = 1,
                    numberOfPoints = 0,
                    totalPoint = (heroStatConfigs[index].pointPerLv + 1) / 2,
                    inUse = false,
                    unblocked = false,
                    position = index + 1
                };
            }

            heroItems[index].Init(this, heroStats, heroStatConfigs[index]);
            heroItems[index].SetHeroImage(heroStatConfigs[index].heroSpt);
            heroItems[index].SetUnblockedHero();
            if (heroInUse == heroStats)
            {
                SetHeroItemSelected(heroItems[index], heroStatConfigs[index]);
                SetHeroItemInUse(heroInUse, false);
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
    }

    public void SetHeroItemInUse(HeroStats newHeroInUse, bool pressed)
    {
        if (heroInUse != newHeroInUse)
        {
            heroInUse.inUse = false;
            heroStatConfigInUse = heroStatConfigSelected;
            heroInUse = newHeroInUse;
            SaveData();
        }
        rectTfmInUse.gameObject.SetActive(true);
        rectTfmInUse.SetParent(heroItemSelected.transform, false);
        if (pressed)
        {
            SetUserInfo(heroInUse, heroStatConfigInUse);
            // Reset game to get new hero
            if (!GameManager.Instance.BossDie) // can not load game when boss just died
            {
                bossStage.TerminateExcecution();
                bossStage.gameObject.SetActive(false);
                darkBoardLoading.StartFadeBoard(false, () =>
                {
                    bossStage.gameObject.SetActive(true);
                    GameManager.Instance.RenewGameState(true);
                });
            }
        }
    }

    public void SetUserInfo(HeroStats heroStats, SObjHeroStatConfig heroConfig)
    {
        AddtionalEffectType type = AddtionalEffectType.IncreaseATK;
        for (int i = 0; i < heroConfig.addtionalEffects.Length; i++)
        {
            AddtionalEffect addtionalEffect = heroConfig.addtionalEffects[i];
            int effectLv = heroInfoUI.GetEffectLevels()[i];
            if (heroStats.level >= effectLv)
            {
                // unlock
                type = addtionalEffect.type;
            }
            UserInfoManager userInfoManager = UserInfoManager.Instance;
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
        for (int i = 0; i < heroStatConfigInUse.addtionalEffects.Length; i++)
        {
            AddtionalEffect addtionalEffect = heroStatConfigInUse.addtionalEffects[i];
            if (addtionalEffect.type == type && heroInUse.level >= heroInfoUI.GetEffectLevels()[i])
            {
                damagePercent += addtionalEffect.percent;
            }
        }
        return damagePercent;
    }

    public void SetPanelActive(bool active)
    {
        // effect
        if (active)
        {
            gObj.SetActive(true);
            TransformUIPanel();
        }
        else
        {
            StopAllCoroutines();
            gObj.SetActive(false);
        }
    }

    public void TransformUIPanel()
    {
        Vector2 startPos = rectTfm.anchoredPosition;
        startPos.y = -745;
        Vector2 endPos = startPos;
        endPos.y = startPos.y * -1;
        StartCoroutine(UITransformController.Instance.IEMovingRect(rectTfm, startPos, endPos, movingTime, LerpType.EaseOutBack));
    }

    public HeroStatsList GetHeroStatsList() => heroStatsList;

    public SObjHeroStatConfig[] GetHeroStatsConfigs() => heroStatConfigs;

    public SObjHeroStatConfig GetHeroStatsConfigSelected() => heroStatConfigSelected;
}
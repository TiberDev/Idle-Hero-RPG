using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject gObjGemEnhance;
    [SerializeField] private Image imgHeroAvatar, imgPoint;
    [SerializeField] private Image imgInUse, imgEnhance;
    [SerializeField] private Button btnUse, btnEnhance;
    [SerializeField] private TMP_Text txtLv, txtName, txtInUse, txtEnhance, txtGemEnhance, txtPoint, txtDescibleEffect;
    [SerializeField] private AddtionalEffectImage[] addtionalEffectImages;
    [SerializeField] private AddtionalEffectItem[] addtionalEffectItems;
    [SerializeField] private RectTransform rectTfmDescibleEffect, rectTfmTxtEnhance;
    [SerializeField] private int[] effectLevels;
    [SerializeField] private Color colorMax, colorPointNor, colorDisableBtn, colorEnhanceBtn, colorInUseBtn, colorTxtUse, colorTxtUseDisable;

    private HeroStats heroStats;
    private HeroStatsManager heroStatsManager;
    private SObjHeroStatConfig heroStatConfig;
    private RectTransform rectTfmEffectItem;

    private int effectLvIndex = 0;

    public void Init(HeroStats stat, HeroStatsManager statsManager, SObjHeroStatConfig config)
    {
        heroStats = stat;
        heroStatsManager = statsManager;
        heroStatConfig = config;
        // reset describe effect
        SetDescribeEffect();
    }

    public int[] GetEffectLevels() => effectLevels;

    public void SetTextName(string name)
    {
        txtName.text = name;
    }

    public void SetTextLevel(string lv)
    {
        txtLv.text = lv;
    }

    public void SetHeroAvt(Sprite spt)
    {
        imgHeroAvatar.sprite = spt;
    }

    public void SetHeroPointUI(int point, int totalPoint)
    {
        imgPoint.color = colorPointNor;
        txtPoint.text = $"{point}/{totalPoint}";
        imgPoint.fillAmount = (float)point / totalPoint;
    }

    public void SetHeroPointUI()
    {
        txtPoint.text = "MAX";
        imgPoint.color = colorMax;
        imgPoint.fillAmount = 1;
    }

    public void SetInUseUI(bool inUse, bool unblocked)
    {
        if (!unblocked)
        {
            btnUse.interactable = false;
            imgInUse.color = colorDisableBtn;
            txtInUse.text = "Not yet obtained";
            txtInUse.color = colorTxtUseDisable;
            return;
        }
        if (inUse)
        {
            btnUse.interactable = false;
            imgInUse.color = colorDisableBtn;
            txtInUse.color = colorTxtUseDisable;
            txtInUse.text = "In Use";
        }
        else
        {
            btnUse.interactable = true;
            imgInUse.color = colorInUseBtn;
            txtInUse.color = colorTxtUse;
            txtInUse.text = "Use";
        }
    }

    public void SetEnhanceUI()
    {
        int gemEnhance = heroStatConfig.gemToEnhance;
        rectTfmTxtEnhance.anchoredPosition = new Vector2(0, 28);
        btnEnhance.interactable = gemEnhance <= GameManager.Instance.GetPinkGem() && heroStats.unblocked;
        imgEnhance.color = gemEnhance <= GameManager.Instance.GetPinkGem() && heroStats.unblocked ? colorEnhanceBtn : colorDisableBtn;
        txtEnhance.text = "Enhance";
        txtGemEnhance.text = gemEnhance.ToString();
        gObjGemEnhance.SetActive(true);
    }

    public void SetEnhanceMaxUI()
    {
        rectTfmTxtEnhance.anchoredPosition = Vector2.zero;
        imgEnhance.color = colorMax;
        btnEnhance.interactable = false;
        txtEnhance.text = "MAX";
        txtEnhance.color = Color.white;
        gObjGemEnhance.SetActive(false);
    }

    public void SetAddtionalEffects()
    {
        effectLvIndex = 0;
        for (int i = 0; i < heroStatConfig.addtionalEffects.Length; i++)
        {
            AddtionalEffect addtionalEffect = heroStatConfig.addtionalEffects[i];
            addtionalEffectItems[i].Init(addtionalEffect, this);
            AddtionalEffectImage effectImage = Array.Find(addtionalEffectImages,
                effect => effect.type == addtionalEffect.type);
            addtionalEffectItems[i].SetEffectImage(effectImage.sprite);

            // check unlock
            if (heroStats.level >= effectLevels[i])
            {
                // unlock
                addtionalEffectItems[i].SetEffectBlock(false);
                effectLvIndex = i + 1;
            }
            else
            {
                addtionalEffectItems[i].SetEffectBlock(true);
            }
        }
    }

    public void SetDescribeEffect(RectTransform tfmItem = null, string descibe = null)
    {
        if (rectTfmEffectItem == tfmItem || tfmItem == null) // close describe effect
        {
            rectTfmDescibleEffect.gameObject.SetActive(false);
            rectTfmEffectItem = null;
        }
        else
        {
            rectTfmDescibleEffect.gameObject.SetActive(true);
            rectTfmDescibleEffect.anchoredPosition = new Vector2(tfmItem.anchoredPosition.x, tfmItem.anchoredPosition.y + 100);
            txtDescibleEffect.text = descibe;
            rectTfmEffectItem = tfmItem;
        }
    }

    /// <summary>
    /// Check level of hero stat to unlock addtional effects
    /// </summary>
    public void CheckEffectUnBlock()
    {
        if (heroStats.level >= effectLevels[effectLvIndex])
        {
            // unlock new addtional effect
            if (heroStats.inUse)
                heroStatsManager.SetUserInfo(heroStats, heroStatConfig);
            addtionalEffectItems[effectLvIndex].SetEffectBlock(false);
            effectLvIndex++;
        }
    }

    public void OnClickInUseBtn()
    {
        SoundManager.Instance.PlayButtonSound();
        heroStats.inUse = true;
        heroStatsManager.SetHeroItemInUse(heroStats, true);
        SetInUseUI(true, heroStats.unblocked);
    }

    /// <summary>
    /// Event only execute when user has enough gem to enhance
    /// </summary>
    public void OnClickEnhanceBtn()
    {
        SoundManager.Instance.PlayButtonSound();
        heroStats.numberOfPoints += heroStats.totalPoint / heroStatConfig.increasedPoint;
        if (heroStats.numberOfPoints >= heroStats.totalPoint)
        {
            // next level, next point
            heroStats.level += 1;
            heroStats.numberOfPoints -= heroStats.totalPoint;
            heroStats.totalPoint = (heroStatConfig.pointPerLv + heroStats.level) / 2;
            SetTextLevel(heroStats.level.ToString());
            // Check effect to unlock
            CheckEffectUnBlock();
        }
        GameManager gameManager = GameManager.Instance;
        gameManager.SetPinkGem(heroStatConfig.gemToEnhance, false);
        // Save data
        heroStatsManager.SaveData();
        // Show UI
        if (heroStats.level == heroStatConfig.levelMax)
        {
            SetHeroPointUI();
            SetEnhanceMaxUI();
        }
        else
        {
            SetHeroPointUI(heroStats.numberOfPoints, heroStats.totalPoint);
            SetEnhanceUI();
        }
    }
}

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroInfoUI : MonoBehaviour
{
    [SerializeField] private Image imgHeroAvatar, imgPoint;
    [SerializeField] private Image imgInUse, imgEnhance;
    [SerializeField] private Button btnUse, btnEnhance;
    [SerializeField] private TMP_Text txtLv, txtName, txtInUse, txtGemEnhance, txtPoint, txtDescibleEffect;
    [SerializeField] private AddtionalEffectImage[] addtionalEffectImages;
    [SerializeField] private AddtionalEffectItem[] addtionalEffectItems;
    [SerializeField] private RectTransform rectTfmDescibleEffect;
    [SerializeField] private int[] effectLevels;
    [SerializeField] private Color colorMax, colorPointNor, colorDisableBtn, colorEnhanceBtn, colorInUseBtn;

    private HeroStats heroStats;
    private HeroStatsManager heroStatsManager;
    private SObjHeroStatConfig heroStatConfig;
    private RectTransform rectTfmEffectItem;

    public void Init(HeroStats stat, HeroStatsManager statsManager, SObjHeroStatConfig config)
    {
        heroStats = stat;
        heroStatsManager = statsManager;
        heroStatConfig = config;
        // reset describe effect
        SetDescribeEffect();
    }

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
            txtInUse.text = "Waiting...";
            return;
        }
        if (inUse)
        {
            btnUse.interactable = false;
            imgInUse.color = colorDisableBtn;
            txtInUse.text = "In Use";
        }
        else
        {
            btnUse.interactable = true;
            imgInUse.color = colorInUseBtn;
            txtInUse.text = "Use";
        }
    }

    public void SetEnhanceUI()
    {
        int gemEnhance = heroStatConfig.gemToEnhance;
        btnEnhance.interactable = gemEnhance <= GameManager.Instance.GetGem() && heroStats.unblocked;
        imgEnhance.color = gemEnhance <= GameManager.Instance.GetGem() && heroStats.unblocked ? colorEnhanceBtn : colorDisableBtn;
        txtGemEnhance.text = gemEnhance.ToString();
    }

    public void SetEnhanceMaxUI()
    {
        imgEnhance.color = colorMax;
        btnEnhance.interactable = false;
        txtGemEnhance.text = "...";
    }

    public void SetAddtionalEffects()
    {
        for (int i = 0; i < heroStats.addtionalEffects.Length; i++)
        {
            addtionalEffectItems[i].Init(heroStats.addtionalEffects[i], this);
            AddtionalEffectImage effectImage = Array.Find(addtionalEffectImages,
                effect => effect.type == heroStats.addtionalEffects[i].type);
            addtionalEffectItems[i].SetEffectImage(effectImage.sprite);
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
        for (int i = 0; i < heroStats.addtionalEffects.Length; i++)
        {
            if (heroStats.level >= effectLevels[i])
            {
                // unlock
                bool old = heroStats.addtionalEffects[i].unblock;
                heroStats.addtionalEffects[i].unblock = true;
                if (!old) // newly unlock
                    heroStatsManager.SetUserInfo(heroStats);
                addtionalEffectItems[i].SetEffectBlock(false);
            }
            else
            {
                heroStats.addtionalEffects[i].unblock = false;
                addtionalEffectItems[i].SetEffectBlock(true);
            }
        }
    }

    public void OnClickInUseBtn()
    {
        heroStats.inUse = true;
        heroStatsManager.SetHeroItemInUse(heroStats,true);
        SetInUseUI(true, heroStats.unblocked);
    }

    /// <summary>
    /// Event only execute when user has enough gem to enhance
    /// </summary>
    public void OnClickEnhanceBtn()
    {
        heroStats.numberOfPoints += heroStatConfig.increasedPoint;
        if (heroStats.numberOfPoints >= heroStats.totalPoint)
        {
            // next level, next point
            heroStats.level += 1;
            heroStats.totalPoint += heroStatConfig.pointPerLv;
            SetTextLevel(heroStats.level.ToString());
            // Check effect to unlock
            CheckEffectUnBlock();
        }
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
        // Save data
        Db.SaveHeroData(heroStats, heroStats.name);
        GameManager.Instance.SetGem(GameManager.Instance.GetGem() - heroStatConfig.gemToEnhance);
    }
}

using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GearItem : MonoBehaviour
{
    [SerializeField] private TMP_Text txtLv, txtPoint;
    [SerializeField] private Image imgAmountPoint, imgGearBgr, imgGearIcon;
    [SerializeField] private GameObject gObjlock;

    public GearStats gearStats;
    private GearsStatsManager gearsStatsManager;
    private Sprite gearIcon;
    private SObjGearsStatsConfig gearsStatsConfig;

    public void Init(GearStats stats, GearsStatsManager manager, SObjGearsStatsConfig config)
    {
        gearStats = stats;
        gearsStatsManager = manager;
        gearsStatsConfig = config;
    }

    public void SetTextLevel(string lv)
    {
        txtLv.text = lv;
    }

    public void SetGearIcon(Sprite spt)
    {
        gearIcon = spt;
        imgGearIcon.sprite = spt;
    }

    public void SetGearBackGround(Sprite spt)
    {
        imgGearBgr.sprite = spt;
    }

    public void SetBlock(bool unlock)
    {
        gObjlock.SetActive(!unlock);
    }

    public void SetGearPointUI(int point, int totalPoint)
    {
        txtPoint.text = $"{point}/{totalPoint}";
        imgAmountPoint.color = Color.green;
        imgAmountPoint.fillAmount = (float)point / totalPoint;
    }

    public void SetGearPointUI()
    {
        txtPoint.text = "MAX";
        imgAmountPoint.color = Color.blue;
        imgAmountPoint.fillAmount = 1;
    }

    /// <summary>
    /// Enhance gear stats by points
    /// </summary>
    public void SetEnhance(bool isEnhanceAll)
    {
        while (gearStats.numberOfPoints >= gearStats.totalPoint && gearStats.level < gearsStatsConfig.levelMax) // check next point
        {
            gearStats.numberOfPoints -= gearStats.totalPoint;
            gearStats.level += 1;
            gearStats.equippedEffect = (BigInteger.Parse(gearStats.equippedEffect) + gearStats.level).ToString();
            gearStats.ownedEffect = (BigInteger.Parse(gearStats.ownedEffect) + gearStats.level).ToString();
            gearStats.totalPoint = gearsStatsConfig.totalPointByXLv * gearStats.level;
        }
        // Change UI when enhance successfully
        SetTextLevel(gearStats.level.ToString());
        if (gearStats.level == gearsStatsConfig.levelMax)
        {
            SetGearPointUI();
        }
        else
        {
            SetGearPointUI(gearStats.numberOfPoints, gearStats.totalPoint);
        }
        // Save data
        Db.SaveGearData(gearStats, gearStats.name, gearStats.type);
        gearsStatsManager.RemoveGearItemEnhance(this);
        // Change OE value when enhance successfully
        gearsStatsManager.SetTotalOwnedEffectValue();
        if (!isEnhanceAll)
        {
            if (gearStats.type == GearType.Weapon)
                UserInfoManager.Instance.SetATK();
            else
                UserInfoManager.Instance.SetHp();
        }
    }

    public void OnClickItem()
    {
        gearsStatsManager.SetGearInfoUI(gearStats, gearIcon, this, gearsStatsConfig);
    }


}

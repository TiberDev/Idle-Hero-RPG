using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GearItem : MonoBehaviour
{
    [SerializeField] private TMP_Text txtLv, txtPoint;
    [SerializeField] private Image imgAmountPoint, imgGearBgr, imgGearIcon;
    [SerializeField] private GameObject gObjLock;

    public GearStats gearStats;
    private GearsStatsManager gearsStatsManager;
    private Sprite gearIcon;
    private SObjGearsStatsConfig gearsStatsConfig;
    private Transform cachedTfm;

    private void Awake()
    {
        cachedTfm = transform;
    }

    public void Init(GearStats stats, GearsStatsManager manager, SObjGearsStatsConfig config)
    {
        gearStats = stats;
        gearsStatsManager = manager;
        gearsStatsConfig = config;
    }

    public Transform GetTransform() => cachedTfm;

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
        gObjLock.SetActive(!unlock);
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
    public void SetEnhance()
    {
        while (gearStats.numberOfPoints >= gearStats.totalPoint && gearStats.level < gearsStatsConfig.levelMax) // check next point
        {
            gearStats.numberOfPoints -= gearStats.totalPoint;
            gearStats.level += 1;
            gearStats.equippedEffect = (gearsStatsConfig.firstEquippedEffect + gearStats.level);

            gearsStatsManager.SetTotalOwnedEffectValue(gearStats.ownedEffect, false, gearStats.type);
            gearStats.ownedEffect = (gearsStatsConfig.firstOwnedEffect + gearStats.level);
            gearsStatsManager.SetTotalOwnedEffectValue(gearStats.ownedEffect, true, gearStats.type);

            gearStats.totalPoint = gearsStatsConfig.pointPerLv + (gearStats.level * gearsStatsConfig.maxPercentLevel / 100);
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
        gearsStatsManager.SaveData(gearsStatsConfig.type);
        gearsStatsManager.RemoveGearItemEnhance(this);
        if (gearStats.type == GearType.Weapon)
            UserInfoManager.Instance.SetATK();
        else
            UserInfoManager.Instance.SetHp();
    }

    public void OnClickItem()
    {
        if (gearStats.unblocked)
            gearsStatsManager.SetGearInfoUI(gearStats, gearIcon, this, gearsStatsConfig);
    }
}

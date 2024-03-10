using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GearInfoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text txtName, txtLv, txtMode, txtPoint, txtEquip, txtEnhance, txtOwnedEffect, txtEquippedEffect;
    [SerializeField] private Image imgAmountPoint, imgGearBgr, imgGearIcon;
    [SerializeField] private Button btnEquip, btnEnhance;

    public GearStats gearStats;
    private GearsStatsManager gearsStatsManager;
    private GearItem gearItemSelected;
    private SObjGearsStatsConfig gearsStatsConfig;

    public void Init(GearStats stats, GearsStatsManager manager, GearItem gearItem, SObjGearsStatsConfig config)
    {
        gearStats = stats;
        gearsStatsManager = manager;
        gearItemSelected = gearItem;
        gearsStatsConfig = config;
    }

    public void SetTextName(string name)
    {
        txtName.text = name;
    }

    public void SetTextLevel(string lv)
    {
        txtLv.text = lv;
    }

    public void SetGearIcon(Sprite spt)
    {
        imgGearIcon.sprite = spt;
    }

    public void SetGearBackGround(Sprite spt)
    {
        imgGearBgr.sprite = spt;
    }

    public void SetMode(GearMode mode)
    {
        txtMode.text = mode.ToString();
    }

    public void SetGearPointUI(int point, int totalPoint)
    {
        txtPoint.text = $"{point}/{totalPoint}";
        imgAmountPoint.color = Color.yellow;
        imgAmountPoint.fillAmount = (float)point / totalPoint;
    }
    public void SetGearPointUI()
    {
        txtPoint.text = "MAX";
        imgAmountPoint.color = Color.blue;
        imgAmountPoint.fillAmount = 1;
    }

    public void SetTxtOwnedEffect(string value)
    {
        txtOwnedEffect.text = GetTypeDescribeEffect() + FillData.Instance.FormatNumber(BigInteger.Parse(value)) + "%";
    }

    public void SetTextEquippedEffect(string value)
    {
        txtEquippedEffect.text = GetTypeDescribeEffect() + FillData.Instance.FormatNumber(BigInteger.Parse(value)) + "%";
    }

    public string GetTypeDescribeEffect()
    {
        return gearStats.type == GearType.Weapon ? "ATK + " : "HP + ";
    }

    public void SetEquipBtn(bool equipped, bool unlock)
    {
        btnEquip.interactable = unlock && !equipped;

        if (equipped)
        {
            txtEquip.text = "Equipped";
            btnEquip.interactable = false;
            return;
        }
        txtEquip.text = "Equip";
        btnEquip.interactable = unlock;
    }

    public void SetEnhaceBtn(bool isActive, bool isMax)
    {
        btnEnhance.interactable = isActive;
        txtEnhance.text = isMax ? "MAX" : "Enhance";
    }

    public void OnClickEquipBtn()
    {
        gearStats.equipped = true;
        SetEquipBtn(true, gearStats.unblocked);
        gearsStatsManager.SetGearItemEquip(gearStats, gearItemSelected.transform,true);
        gearsStatsManager.OnClickSpace();
    }

    public void OnClickEnhanceBtn()
    {
        gearItemSelected.SetEnhance(false);
        //// Show UI
        SetTextLevel(gearStats.level.ToString());
        SetTextEquippedEffect(gearStats.equippedEffect);
        SetTxtOwnedEffect(gearStats.ownedEffect);
        if (gearStats.level == gearsStatsConfig.levelMax)
        {
            SetEnhaceBtn(false, true);
            SetGearPointUI();
        }
        else
        {
            SetEnhaceBtn(false, false); // button can't interacted because number of points is always > total point
            SetGearPointUI(gearStats.numberOfPoints, gearStats.totalPoint);
        }
    }


}

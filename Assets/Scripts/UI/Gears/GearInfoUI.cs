using BigInteger = System.Numerics.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GearInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject gObj;
    [SerializeField] private RectTransform rectTfm;
    [SerializeField] private TMP_Text txtName, txtLv, txtMode, txtPoint, txtEquip, txtEnhance, txtOwnedEffect, txtEquippedEffect;
    [SerializeField] private Image imgAmountPoint, imgGearBgr, imgGearIcon, imgEquip, imgEnhance;
    [SerializeField] private Button btnEquip, btnEnhance;
    [SerializeField] private float scalingUpTime, scalingDownTime;
    [SerializeField] private Color colorDisableBtn, colorEquip, colorEquipped, colorEnhance, colorMax;

    public GearStats gearStats;
    private GearsStatsManager gearsStatsManager;
    private GearItem gearItemSelected;
    private SObjGearsStatsConfig gearsStatsConfig;
    private Coroutine corouGearInfo;

    public void Init(GearStats stats, GearsStatsManager manager, GearItem gearItem, SObjGearsStatsConfig config)
    {
        gearStats = stats;
        gearsStatsManager = manager;
        gearItemSelected = gearItem;
        gearsStatsConfig = config;
    }

    public void SetActive(bool active)
    {
        // effect
        TransformUIPanel(active);
    }

    private void SetInActive()
    {
        gObj.SetActive(false);
        corouGearInfo = null;
    }

    public void TransformUIPanel(bool open)
    {
        if (open)
        {
            gObj.SetActive(true);
            corouGearInfo = StartCoroutine(UITransformController.Instance.IEScalingRect(rectTfm, Vector2.one * 0.5f, Vector2.one, scalingUpTime, LerpType.EaseOutBack));
        }
        else
        {
            StopCoroutine(corouGearInfo);
            corouGearInfo = null;
            corouGearInfo = StartCoroutine(UITransformController.Instance.IEScalingRect(rectTfm, rectTfm.localScale, Vector2.zero, scalingDownTime, LerpType.EaseInBack, SetInActive));
        }

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

    public void SetMode(GearMode mode, Color color)
    {
        txtMode.text = mode.ToString();
        txtMode.color = color;
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
        if (equipped)
        {
            txtEquip.text = "Equipped";
            btnEquip.interactable = false;
            imgEquip.color = colorEquipped;
            return;
        }
        txtEquip.text = "Equip";
        btnEquip.interactable = unlock;
        imgEquip.color = unlock ? colorEquip : colorDisableBtn;
    }

    public void SetEnhaceBtn(bool isActive, bool isMax)
    {
        btnEnhance.interactable = isActive;
        if (isMax)
        {
            imgEnhance.color = colorMax;
        }
        else
        {
            imgEnhance.color = isActive ? colorEnhance : colorDisableBtn;
        }
        txtEnhance.text = isMax ? "MAX" : "Enhance";
    }

    public void OnClickEquipBtn()
    {
        gearStats.equipped = true;
        SetEquipBtn(true, gearStats.unblocked);
        gearsStatsManager.SetGearItemEquip(gearItemSelected, true, gearStats.type, gearStats);
        gearsStatsManager.OnClickSpace();
    }

    public void OnClickEnhanceBtn()
    {
        gearItemSelected.SetEnhance();
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

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject gObj;
    [SerializeField] private RectTransform rectTfm;
    [SerializeField] private TMP_Text txtName, txtLv, txtDescribe, txtCoolTime, txtPoint, txtEquip, txtEnhance, txtOwnedEffect;
    [SerializeField] private Image imgAmountPoint, imgGearIcon, imgEquip_Remove, imgEnhance;
    [SerializeField] private Button btnEquip, btnEnhance;
    [SerializeField] private Color colorCoolTime, colorDescribe, colorDisableBtn, colorEquip, colorRemove, colorEnhance, colorMax;
    [SerializeField] private float scalingUpTime, scalingDownTime;

    private SkillStats skillStats;
    private SkillStatsManager skillStatsManager;
    private SObjSkillStatsConfig skillStatsConfig;
    private SkillItem skillItem;
    private Coroutine corouSkillInfo;

    public void Init(SkillStats stats, SkillStatsManager manager, SkillItem item, SObjSkillStatsConfig config)
    {
        skillStats = stats;
        skillStatsManager = manager;
        skillItem = item;
        skillStatsConfig = config;
    }

    public void SetActive(bool active)
    {
        // effect
        TransformUIPanel(active);
    }

    private void SetInActive()
    {
        gObj.SetActive(false);
        corouSkillInfo = null;
    }

    public void TransformUIPanel(bool open)
    {
        if (open)
        {
            gObj.SetActive(true);
            corouSkillInfo = StartCoroutine(UITransformController.Instance.IEScalingRect(rectTfm, Vector2.one * 0.5f, Vector2.one, scalingUpTime, LerpType.EaseOutBack));
        }
        else
        {
            StopCoroutine(corouSkillInfo);
            corouSkillInfo = null;
            corouSkillInfo = StartCoroutine(UITransformController.Instance.IEScalingRect(rectTfm, rectTfm.localScale, Vector2.one * 0.5f, scalingDownTime, LerpType.EaseInBack, SetInActive));
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

    public void SetSkillcon(Sprite spt)
    {
        imgGearIcon.sprite = spt;
    }

    public void SetSkillPointUI(int point, int totalPoint)
    {
        txtPoint.text = $"{point}/{totalPoint}";
        imgAmountPoint.color = Color.green;
        imgAmountPoint.fillAmount = (float)point / totalPoint;
    }

    public void SetSkillPointUI()
    {
        txtPoint.text = "MAX";
        imgAmountPoint.color = Color.blue;
        imgAmountPoint.fillAmount = 1;
    }

    public void SetEquipBtn(bool equipped, bool unlock)
    {
        btnEquip.interactable = unlock;
        txtEquip.text = equipped ? "Remove" : "Equip";
        imgEquip_Remove.color = equipped ? colorRemove : colorEquip;
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

    public void SetTextCoolTime(int coolTime)
    {
        txtCoolTime.text = $"Cool Time: <color=#{ColorUtility.ToHtmlStringRGB(colorCoolTime)}>{coolTime}s</color>";
    }

    public void SetTextOwnedEffect(int value)
    {
        txtOwnedEffect.text = "ATK + " + NumberConverter.Instance.FormatNumber(value) + "%";
    }

    public void SetTextDescribe(string describe_1, string describe_2, int damage)
    {
        txtDescribe.text = $"{describe_1} <color=#{ColorUtility.ToHtmlStringRGB(colorDescribe)}>{damage}%</color> {describe_2}";
    }

    public void OnClickEnhanceBtn()
    {
        SoundManager.Instance.PlayButtonSound();
        skillStatsManager.SetSkillItemEnhance(skillStats, skillStatsConfig);
        // Show UI
        SetTextLevel(skillStats.level.ToString());
        SetTextOwnedEffect(skillStats.ownedEffect);
        SetTextDescribe(skillStatsConfig.describe_1, skillStatsConfig.describe_2, skillStats.value);
        if (skillStats.level == skillStatsConfig.levelMax)
        {
            SetEnhaceBtn(false, true);
            SetSkillPointUI();
        }
        else
        {
            SetEnhaceBtn(false, false); // button can't interacted because number of points is always > total point
            SetSkillPointUI(skillStats.numberOfPoints, skillStats.totalPoint);
        }
    }

    public void OnClickEquipBtn()
    {
        SoundManager.Instance.PlayButtonSound();
        skillItem.OnClickEquipped_Remove();
        SetActive(false);
    }

    public void OnClickCover(BaseEventData baseEventData)
    {
        PointerEventData pointerEvent = baseEventData as PointerEventData;
        if (pointerEvent.pointerEnter == gObj)
        {
            SetActive(false);
        }
    }
}

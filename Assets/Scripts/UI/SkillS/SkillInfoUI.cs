using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.ContentSizeFitter;

public class SkillInfoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text txtName, txtLv, txtDescribe, txtCoolTime, txtPoint, txtEquip, txtEnhance, txtOwnedEffect;
    [SerializeField] private Image imgAmountPoint, imgGearIcon;
    [SerializeField] private Button btnEquip, btnEnhance;
    [SerializeField] private Color colorDescribe;

    private SkillStats skillStats;
    private SkillStatsManager skillStatsManager;
    private SObjSkillStatsConfig skillStatsConfig;
    private SkillItem skillItem;

    public void Init(SkillStats stats, SkillStatsManager manager, SkillItem item, SObjSkillStatsConfig config)
    {
        skillStats = stats;
        skillStatsManager = manager;
        skillItem = item;
        skillStatsConfig = config;
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
        imgAmountPoint.color = Color.yellow;
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
    }

    public void SetEnhaceBtn(bool isActive, bool isMax)
    {
        btnEnhance.interactable = isActive;
        txtEnhance.text = isMax ? "MAX" : "Enhance";
    }

    public void SetTextCoolTime(int coolTime)
    {
        txtCoolTime.text = "Cool Time: " + coolTime.ToString();
    }

    public void SetTextOwnedEffect(int value)
    {
        txtOwnedEffect.text = "ATK + " + FillData.Instance.FormatNumber(value) + "%";
    }

    public void SetTextDescribe(string describe_1, string describe_2, int damage)
    {
        txtDescribe.text = $"{describe_1} <color=#{ColorUtility.ToHtmlStringRGB(colorDescribe)}>{damage}%</color> {describe_2}";
    }

    public void OnClickEnhanceBtn()
    {
        skillStatsManager.SetSkillItemEnhance(skillStats, skillStatsConfig);
        // Show UI
        SetTextLevel(skillStats.level.ToString());
        SetTextOwnedEffect(skillStats.ownedEffect);
        SetTextDescribe(skillStatsConfig.describe_1, skillStatsConfig.describe_2, skillStats.damage);
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
        skillItem.OnClickEquipped_Remove();
        skillStatsManager.OnClickSpace();
    }
}

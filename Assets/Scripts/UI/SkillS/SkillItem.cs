using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillItem : MonoBehaviour
{
    [SerializeField] private TMP_Text txtLv, txtPoint;
    [SerializeField] private Image imgAmountPoint, imgSkillIcon, imgEquippedIcon;
    [SerializeField] private Sprite equippedSpt, removedSpt;
    [SerializeField] private GameObject gObjLock, gObjEquipped, gObjCover;

    public SkillStats skillStats;
    private SkillStatsManager skillStatsManager;
    private SObjSkillStatsConfig skillStatsConfig;
    private Sprite skillIcon;
    private bool isOnEquippedItemListPanel;

    public void Init(SkillStats stats, SkillStatsManager manager, SObjSkillStatsConfig config, bool onPanel = false)
    {
        skillStats = stats;
        skillStatsManager = manager;
        skillStatsConfig = config;
        isOnEquippedItemListPanel = onPanel;
    }

    public void SetTextLevel(string lv)
    {
        txtLv.text = lv;
    }

    public void SetSkillIcon(Sprite spt)
    {
        skillIcon = spt;
        imgSkillIcon.sprite = spt;
    }

    public void SetUnlock(bool unlock)
    {
        gObjLock.SetActive(!unlock);
        if (gObjCover != null)
            gObjCover.SetActive(!unlock);
    }

    public void SetEquipped_RemoveIcon(bool equipped)
    {
        imgEquippedIcon.sprite = equipped ? removedSpt : equippedSpt;
    }

    public void SetEquipped_RemoveGOActive(bool isActive)
    {
        imgEquippedIcon.gameObject.SetActive(isActive);
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

    public string GetName()
    {
        return skillStats.name;
    }

    public SkillStats GetSkillStats()
    {
        return skillStats;
    }

    public void ShowEquippedText(bool isActive)
    {
        gObjEquipped.SetActive(isActive);
        if (gObjCover != null && skillStats.unblocked)
            gObjCover.SetActive(isActive);
    }

    public void SetEnhanceUI()
    {
        // Change UI when enhance successfully
        SetTextLevel(skillStats.level.ToString());
        if (skillStats.level == skillStatsConfig.levelMax)
        {
            SetSkillPointUI();
        }
        else
        {
            SetSkillPointUI(skillStats.numberOfPoints, skillStats.totalPoint);
        }
        skillStatsManager.RemoveSkillItemEnhance(this);
    }

    public void OnClickItem()
    {
        skillStatsManager.SetSkillInfoUI(skillStats, skillIcon, this, skillStatsConfig);
    }

    /// <summary>
    /// Handle equip or remove 
    /// </summary>
    public void OnClickEquipped_Remove()
    {
        if (!skillStats.equipped) // skill item is currently in remove state 
        {
            skillStatsManager.HandleEquipSkillItem(skillStats, skillStatsConfig, this);
        }
        else // in equipped state
        {
            skillStatsManager.HandleRemoveSkillItem(this, skillStats, isOnEquippedItemListPanel);
        }
    }

}

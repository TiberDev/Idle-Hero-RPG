using System.Collections.Generic;
using BigInteger = System.Numerics.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillStatsManager : MonoBehaviour
{
    [SerializeField] private SkillInfoUI skillInfoUI;
    [SerializeField] private SObjSkillStatsConfig[] skillStatsConfigs;
    [SerializeField] private Transform tfmSkillItemParent;
    [SerializeField] private SkillItem skillItemPrefab;
    [SerializeField] private Button btnEnhanceAll;
    [SerializeField] private GameObject gObjCoverItemList;
    [SerializeField] private Image imgItemIconover;
    [SerializeField] private TMP_Text txtTotalOwnedEffectValue;
    [SerializeField] private SkillItemEquippedManager skillItemEquippedManager;
    [SerializeField] private SObjMapConditionConfig[] mapConditionConfigs;
    [SerializeField] private MapManager mapManager;
    [SerializeField] private SkillTable skillTable;

    public List<SkillItem> skillItemsEnhance = new List<SkillItem>();
    public List<SkillItem> skillItems = new List<SkillItem>();
    public List<SkillStats> skillStatsList = new List<SkillStats>();

    private void OnEnable()
    {
        SetSkillItem();
    }

    public void LoadSkillData()
    {
        for (int i = 0; i < skillStatsConfigs.Length; i++)
        {
            SkillStats skillStats = Db.ReadHSkillData(skillStatsConfigs[i].skillName);
            if (skillStats == null) // new user or new item
            {
                skillStats = new SkillStats()
                {
                    name = skillStatsConfigs[i].skillName,
                    level = 1,
                    numberOfPoints = 1,
                    totalPoint = skillStatsConfigs[i].totalPointByXLv,
                    ownedEffect = skillStatsConfigs[i].ownedEffect,
                    value = skillStatsConfigs[i].damage,
                    cooldown = skillStatsConfigs[i].cooldown,
                    equipped = false,
                    unblocked = false,
                    position = 0
                };
                Db.SaveSkillData(skillStats, skillStats.name);
            }
            if (skillStats.equipped)
                skillTable.SetSkillTableItem(skillStats.position - 1, skillStats, skillStatsConfigs[i], false);
            skillStatsList.Add(skillStats);
        }
    }

    public void CheckUnlockSkillItem()
    {
        for (int i = 0; i < mapConditionConfigs.Length; i++)
        {
            MapData mapData = mapManager.GetMapData();
            if (mapData.map < mapConditionConfigs[i].map)
            {
                // lock
                skillTable.UnlockSkillTblItem(i, false);
                skillItemEquippedManager.CheckUnLock(i, false);
                break;
            }
            else if (mapData.map == mapConditionConfigs[i].map)
            {
                // check round 
                if (mapData.round < mapConditionConfigs[i].round)
                {
                    // lock
                    skillTable.UnlockSkillTblItem(i, false);
                    skillItemEquippedManager.CheckUnLock(i, false);
                    break;
                }
                else
                {
                    // unlock
                    skillTable.UnlockSkillTblItem(i, true);
                    skillItemEquippedManager.CheckUnLock(i, true);
                }
            }
            else
            {
                // unlock
                skillTable.UnlockSkillTblItem(i, true);
                skillItemEquippedManager.CheckUnLock(i, true);
            }
        }
    }

    private void SetSkillItem()
    {
        // reset 
        btnEnhanceAll.interactable = false;
        skillItemsEnhance.Clear();
        SkillItem skillItem = null;
        for (int index = 0; index < skillStatsList.Count; index++)
        {
            if (skillItems.Count < index + 1) // spawn gear item if not spawned yet
            {
                skillItem = Instantiate(skillItemPrefab, tfmSkillItemParent);
                skillItems.Add(skillItem);
            }
            // Init gear item
            skillItem = skillItems[index];
            skillItem.Init(skillStatsList[index], this, skillStatsConfigs[index]);
            skillItem.SetTextLevel(skillStatsList[index].level.ToString());
            skillItem.SetSkillIcon(skillStatsConfigs[index].skillSpt);
            if (skillStatsList[index].level == skillStatsConfigs[index].levelMax)
            {
                skillItem.SetSkillPointUI();
            }
            else
            {
                skillItem.SetSkillPointUI(skillStatsList[index].numberOfPoints, skillStatsList[index].totalPoint);
            }
            // Check points to enhance later
            if (skillStatsList[index].numberOfPoints >= skillStatsList[index].totalPoint && skillStatsList[index].level < skillStatsConfigs[index].levelMax)
            {
                skillItemsEnhance.Add(skillItem);
                btnEnhanceAll.interactable = true;
            }
            skillItem.SetUnlock(skillStatsList[index].unblocked);
            skillItem.gameObject.SetActive(true);
            skillItem.SetEquipped_RemoveImage(skillStatsList[index].equipped);
            skillItem.ShowEquippedText(skillStatsList[index].equipped);
            // check equipped item to display
            if (skillStatsList[index].equipped)
            {
                skillItemEquippedManager.SetSkillItemEquipped(skillStatsList[index].position - 1, skillStatsList[index], skillStatsConfigs[index], this);
            }
        }
        // Display total OE value UI
        SetTotalOwnedEffectValue();
    }

    public void SetSkillInfoUI(SkillStats skillStats, Sprite icon, SkillItem skillItem, SObjSkillStatsConfig skillStatsConfig)
    {
        skillInfoUI.Init(skillStats, this, skillItem, skillStatsConfig);
        skillInfoUI.SetTextName(skillStats.name);
        skillInfoUI.SetTextLevel(skillStats.level.ToString());
        skillInfoUI.SetSkillcon(icon);
        if (skillStats.level == skillStatsConfig.levelMax)
        {
            skillInfoUI.SetSkillPointUI();
            skillInfoUI.SetEnhaceBtn(false, true);
        }
        else
        {
            skillInfoUI.SetSkillPointUI(skillStats.numberOfPoints, skillStats.totalPoint);
            skillInfoUI.SetEnhaceBtn(skillStats.numberOfPoints >= skillStats.totalPoint, false); // maybe enhance when current point >= total point
        }
        skillInfoUI.SetTextOwnedEffect(skillStats.ownedEffect);
        skillInfoUI.SetTextDescribe(skillStatsConfig.describe_1, skillStatsConfig.describe_2, skillStats.value);
        skillInfoUI.SetTextCoolTime(skillStats.cooldown);
        skillInfoUI.SetEquipBtn(skillStats.equipped, skillStats.unblocked);
        skillInfoUI.SetActive(true);
    }

    public void RemoveGearItemEnhance(SkillItem skillItem)
    {
        if (skillItemsEnhance.Contains(skillItem))
            skillItemsEnhance.Remove(skillItem);
    }

    public void SetTotalOwnedEffectValue()
    {
        int totalOEValue = 0; // total owned effect value
        for (int i = 0; i < skillStatsList.Count; i++)
        {
            if (skillStatsList[i].unblocked)
                totalOEValue += skillStatsList[i].ownedEffect;
        }
        txtTotalOwnedEffectValue.text = "Owned Effects: ATK + " + FillData.Instance.FormatNumber(totalOEValue) + "%";
    }

    private SkillStats skillStatsTemp;
    private SObjSkillStatsConfig configTemp;
    private SkillItem skillItemTemp;

    private void SetCoverGO(bool isActive, Sprite spt = null)
    {
        gObjCoverItemList.SetActive(isActive);
        imgItemIconover.sprite = spt;
    }

    public void HandleEquipSkillItemCallBack(SkillStats removedSkillStats, int index)
    {
        skillItemEquippedManager.SetAllChangeBtn(false);
        HandleRemoveSkillItem(null, removedSkillStats, true);
        skillStatsTemp.equipped = true;
        skillStatsTemp.position = index + 1;
        skillItemEquippedManager.SetSkillItemEquipped(index, skillStatsTemp, configTemp, this);
        skillTable.SetSkillTableItem(index, skillStatsTemp, configTemp, true);
        skillItemTemp.SetEquipped_RemoveImage(true);
        skillItemTemp.ShowEquippedText(true);
        SetCoverGO(false);
        skillItemEquippedManager.SetAllChangeBtn(false);
        Db.SaveSkillData(skillStatsTemp, skillStatsTemp.name);
    }

    public void HandleEquipSkillItem(SkillStats skillStats, SObjSkillStatsConfig skillStatsConfig, SkillItem skillItem)
    {
        int index = skillItemEquippedManager.GetPositionEmpty();
        if (index < 0) // choose one of equipped items to change
        {
            skillStatsTemp = skillStats;
            configTemp = skillStatsConfig;
            skillItemTemp = skillItem;
            skillItemEquippedManager.SetAllChangeBtn(true);
            SetCoverGO(true, skillStatsConfig.skillSpt);
            return;
        }
        skillStats.equipped = true;
        skillStats.position = index + 1;
        skillItemEquippedManager.SetSkillItemEquipped(index, skillStats, skillStatsConfig, this);
        skillTable.SetSkillTableItem(index, skillStats, skillStatsConfig, true);
        skillItem.SetEquipped_RemoveImage(skillStats.equipped);
        skillItem.ShowEquippedText(true);
        Db.SaveSkillData(skillStats, skillStats.name);
    }

    public void HandleRemoveSkillItem(SkillItem skillItem, SkillStats skillStats, bool onPanel)
    {
        if (onPanel)
        {
            for (int i = 0; i < skillItems.Count; i++)
            {
                if (skillItems[i].GetName() == skillStats.name)
                {
                    skillItem = skillItems[i];
                    break;
                }
            }
        }
        skillItem.SetEquipped_RemoveImage(false);
        skillItem.ShowEquippedText(false);
        skillItemEquippedManager.SetSkillItemEmpty(skillStats.position - 1);
        skillTable.SetSkillTblItemEmpty(skillStats.position - 1);
        skillStats.equipped = false;
        skillStats.position = 0;
        Db.SaveSkillData(skillStats, skillStats.name);
    }

    public void SetSkillItemEnhance(SkillStats skillStats, SObjSkillStatsConfig skillStatsConfig)
    {
        IncreasePointSkillItem(skillStats, skillStatsConfig);
        Db.SaveSkillData(skillStats, skillStats.name);
        // Display on UI
        SkillItem skillItem = null;
        for (int i = 0; i < skillItems.Count; i++)
        {
            if (skillItems[i].GetName() == skillStats.name)
            {
                skillItem = skillItems[i];
                break;
            }
        }
        skillItem.SetEnhanceUI();
        if (skillStats.equipped)
            skillItemEquippedManager.GetSkillItem(skillStats.position - 1).SetEnhanceUI();

        UserInfoManager.Instance.SetATK();
    }

    private void IncreasePointSkillItem(SkillStats skillStats, SObjSkillStatsConfig skillStatsConfig)
    {
        while (skillStats.numberOfPoints >= skillStats.totalPoint && skillStats.level < skillStatsConfig.levelMax) // check next point
        {
            skillStats.numberOfPoints -= skillStats.totalPoint;
            skillStats.level += 1;
            skillStats.value = (skillStats.value + skillStats.level);
            skillStats.ownedEffect = skillStats.ownedEffect + skillStats.level;
            skillStats.totalPoint = skillStatsConfig.totalPointByXLv * skillStats.level;
        }
    }

    private void SetAllSkillItemsEnhance()
    {
        for (int i = 0; i < skillStatsList.Count; i++)
        {
            IncreasePointSkillItem(skillStatsList[i], skillStatsConfigs[i]);
            Db.SaveSkillData(skillStatsList[i], skillStatsList[i].name);
            // Display on UI
            skillItems[i].SetEnhanceUI();
            if (skillStatsList[i].equipped)
                skillItemEquippedManager.GetSkillItem(skillStatsList[i].position - 1).SetEnhanceUI();
        }
        UserInfoManager.Instance.SetATK();
    }

    /// <summary>
    /// Close skill info gameobject
    /// </summary>
    public void OnClickSpace()
    {
        skillInfoUI.SetActive(false);
    }

    public void OnClickItemListCoverBtn()
    {
        SetCoverGO(false);
        skillItemEquippedManager.SetAllChangeBtn(false);
    }

    public void OnClickEnhanceAll()
    {
        SetAllSkillItemsEnhance();
    }

    public BigInteger GetAllOwnedEffect()
    {
        int damagePercent = 0;
        for (int i = 0; i < skillStatsList.Count; i++)
        {
            if (skillStatsList[i].unblocked)
            {
                damagePercent += skillStatsList[i].ownedEffect;
            }
        }
        return damagePercent;
    }
}

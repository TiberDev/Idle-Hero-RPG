using System.Collections.Generic;
using BigInteger = System.Numerics.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillStatsManager : MonoBehaviour, IBottomTabHandler
{
    [SerializeField] private RectTransform rectTfm;
    [SerializeField] private SkillInfoUI skillInfoUI;
    [SerializeField] private SObjSkillStatsConfig[] skillStatsConfigs;
    [SerializeField] private Transform tfmSkillItemParent;
    [SerializeField] private SkillItem skillItemPrefab;
    [SerializeField] private Button btnEnhanceAll;
    [SerializeField] private GameObject gObjCoverItemList, gObj;
    [SerializeField] private Image imgItemIconover, imgEnhanceAll;
    [SerializeField] private TMP_Text txtTotalOwnedEffectValue;
    [SerializeField] private SkillItemEquippedManager skillItemEquippedManager;
    [SerializeField] private SObjMapConditionConfig[] mapConditionConfigs;
    [SerializeField] private MapManager mapManager;
    [SerializeField] private SkillTable skillTable;
    [SerializeField] private Color colorDisableBtn, colorEnhanceAll;

    [SerializeField] private float movingTime;

    private SkillStats skillStatsTemp;
    private SObjSkillStatsConfig configTemp;
    private SkillItem skillItemTemp;

    public List<SkillItem> skillItemsEnhance = new List<SkillItem>();
    public List<SkillItem> skillItems = new List<SkillItem>();
    public SkillStatsList skillStatsList = null;

    private int totalOEValue = 0; // total owned effect value
    private bool inHandleEquipSkillItemState; // in state that choose one of equipped skill items to change

    public readonly string DATAKEY = "SKILLSTATSLISTDATA";

    private void OnEnable()
    {
        SetSkillItem();
    }

    public void SaveData()
    {
        PlayerPrefs.SetString(DATAKEY, JsonUtility.ToJson(skillStatsList));
    }

    public void LoadSkillData()
    {
        var json = PlayerPrefs.GetString(DATAKEY, null);
        skillStatsList = JsonUtility.FromJson<SkillStatsList>(json);
        if (skillStatsList == null) // new user 
        {
            skillStatsList = new SkillStatsList();
            SkillStats skillStats = new SkillStats()
            {
                name = skillStatsConfigs[0].skillName,
                level = 1,
                numberOfPoints = 1,
                totalPoint = skillStatsConfigs[0].pointPerLv + (1 * skillStatsConfigs[0].maxPercentLevel / 100),
                ownedEffect = skillStatsConfigs[0].ownedEffect,
                value = skillStatsConfigs[0].damage,
                cooldown = skillStatsConfigs[0].cooldown,
                equipped = false,
                unblocked = true,
                equippedPosition = 0,
                position = 1
            };
            if (skillStats.equipped)
                skillTable.SetSkillTableItem(skillStats.equippedPosition - 1, skillStats, skillStatsConfigs[0], false);
            skillStatsList.list.Add(skillStats);
            SaveData();
        }
        else
        {
            for (int i = 0; i < skillStatsList.list.Count; i++)
            {
                SkillStats skillStats = skillStatsList.list[i];
                if (skillStats.equipped)
                    skillTable.SetSkillTableItem(skillStats.equippedPosition - 1, skillStats, skillStatsConfigs[skillStats.position - 1], false);

            }
        }
        SetTotalOwnedEffectValue();
    }

    private void SetSkillItem()
    {
        // check state
        if (inHandleEquipSkillItemState)
            OnClickItemListCoverBtn();

        // reset 
        btnEnhanceAll.interactable = false;
        imgEnhanceAll.color = colorDisableBtn;
        skillItemsEnhance.Clear();
        int statsListIndex = 0;
        for (int index = 0; index < skillStatsConfigs.Length; index++)
        {
            SkillItem skillItem = null;
            if (skillItems.Count < index + 1) // spawn skill item if not spawned yet
            {
                skillItem = Instantiate(skillItemPrefab, tfmSkillItemParent);
                skillItems.Add(skillItem);
            }
            SkillStats skillStats = null;
            if (statsListIndex < skillStatsList.list.Count)
            {
                if (index + 1 == skillStatsList.list[statsListIndex].position) // skill owned is in skill item list
                {
                    skillStats = skillStatsList.list[statsListIndex];
                    statsListIndex++;
                }
            }

            if (skillStats == null)
            {
                skillStats = new SkillStats()
                {
                    name = skillStatsConfigs[index].skillName,
                    level = 1,
                    numberOfPoints = 0,
                    totalPoint = skillStatsConfigs[index].pointPerLv + (1 * skillStatsConfigs[index].maxPercentLevel / 100),
                    ownedEffect = skillStatsConfigs[index].ownedEffect,
                    value = skillStatsConfigs[index].damage,
                    cooldown = skillStatsConfigs[index].cooldown,
                    equipped = false,
                    unblocked = false,
                    equippedPosition = 0,
                    position = index + 1
                };
            }
            // Init gear item
            skillItem = skillItems[index];
            skillItem.Init(skillStats, this, skillStatsConfigs[index]);
            skillItem.SetTextLevel(skillStats.level.ToString());
            skillItem.SetSkillIcon(skillStatsConfigs[index].skillSpt);
            if (skillStats.level == skillStatsConfigs[index].levelMax)
            {
                skillItem.SetSkillPointUI();
            }
            else
            {
                skillItem.SetSkillPointUI(skillStats.numberOfPoints, skillStats.totalPoint);
            }
            // Check points to enhance later
            if (skillStats.numberOfPoints >= skillStats.totalPoint && skillStats.level < skillStatsConfigs[index].levelMax)
            {
                skillItemsEnhance.Add(skillItem);
                btnEnhanceAll.interactable = true;
                imgEnhanceAll.color = colorEnhanceAll;
            }
            skillItem.SetUnlock(skillStats.unblocked);
            skillItem.gameObject.SetActive(true);
            skillItem.SetEquipped_RemoveGOActive(skillStats.unblocked);
            skillItem.SetEquipped_RemoveIcon(skillStats.equipped);
            skillItem.ShowEquippedText(skillStats.equipped);
            // check equipped item to display
            if (skillStats.equipped)
            {
                skillItemEquippedManager.SetSkillItemEquipped(skillStats.equippedPosition - 1, skillStats, skillStatsConfigs[index], this);
            }
        }
        // Display total OE value UI
        SetOEUI();
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

    public void RemoveSkillItemEnhance(SkillItem skillItem)
    {
        if (skillItemsEnhance.Contains(skillItem))
            skillItemsEnhance.Remove(skillItem);
    }

    private void SetTotalOwnedEffectValue()
    {
        for (int i = 0; i < skillStatsList.list.Count; i++)
        {
            if (skillStatsList.list[i].unblocked)
                totalOEValue += skillStatsList.list[i].ownedEffect;
        }
    }

    public void SetTotalOwnedEffectValue(int ownedEffect, bool addtional)
    {
        if (addtional)
            totalOEValue += ownedEffect;
        else
            totalOEValue -= ownedEffect;
        SetOEUI();
    }

    public BigInteger GetAllOEDamage()
    {
        return totalOEValue;
    }

    public void SetOEUI()
    {
        txtTotalOwnedEffectValue.text = "Owned Effects: ATK + " + FillData.Instance.FormatNumber(totalOEValue) + "%";
    }

    private void SetCoverGO(bool isActive, Sprite spt = null)
    {
        gObjCoverItemList.SetActive(isActive);
        imgItemIconover.sprite = spt;
    }

    public void HandleEquipSkillItemCallBack(SkillStats removedSkillStats, int index)
    {
        inHandleEquipSkillItemState = false;
        skillItemEquippedManager.SetAllChangeBtn(false);
        HandleRemoveSkillItem(null, removedSkillStats, true);
        skillStatsTemp.equipped = true;
        skillStatsTemp.equippedPosition = index + 1;
        skillItemEquippedManager.SetSkillItemEquipped(index, skillStatsTemp, configTemp, this);
        skillTable.SetSkillTableItem(index, skillStatsTemp, configTemp, true);
        skillItemTemp.SetEquipped_RemoveIcon(true);
        skillItemTemp.ShowEquippedText(true);
        SetCoverGO(false);
        skillItemEquippedManager.SetAllChangeBtn(false);
        SaveData();
    }

    public void HandleEquipSkillItem(SkillStats skillStats, SObjSkillStatsConfig skillStatsConfig, SkillItem skillItem)
    {
        int index = skillItemEquippedManager.GetPositionEmpty();
        if (index < 0) // choose one of equipped items to change
        {
            inHandleEquipSkillItemState = true;
            skillStatsTemp = skillStats;
            configTemp = skillStatsConfig;
            skillItemTemp = skillItem;
            skillItemEquippedManager.SetAllChangeBtn(true);
            SetCoverGO(true, skillStatsConfig.skillSpt);
            return;
        }
        skillStats.equipped = true;
        skillStats.equippedPosition = index + 1;
        skillItemEquippedManager.SetSkillItemEquipped(index, skillStats, skillStatsConfig, this);
        skillTable.SetSkillTableItem(index, skillStats, skillStatsConfig, true);
        skillItem.SetEquipped_RemoveIcon(skillStats.equipped);
        skillItem.ShowEquippedText(true);
        SaveData();
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
        skillItem.SetEquipped_RemoveIcon(false);
        skillItem.ShowEquippedText(false);
        skillItemEquippedManager.SetSkillItemEmpty(skillStats.equippedPosition - 1);
        skillTable.SetSkillTblItemEmpty(skillStats.equippedPosition - 1);
        skillStats.equipped = false;
        skillStats.equippedPosition = 0;
        SaveData();
    }

    private void IncreasePointSkillItem(SkillStats skillStats, SObjSkillStatsConfig skillStatsConfig)
    {
        while (skillStats.numberOfPoints >= skillStats.totalPoint && skillStats.level < skillStatsConfig.levelMax) // check next point
        {
            skillStats.numberOfPoints -= skillStats.totalPoint;
            skillStats.level += 1;
            skillStats.value = skillStatsConfig.damage + skillStats.level;
            SetTotalOwnedEffectValue(skillStats.ownedEffect, false);
            skillStats.ownedEffect = skillStatsConfig.ownedEffect + skillStats.level;
            SetTotalOwnedEffectValue(skillStats.ownedEffect, true);
            skillStats.totalPoint = skillStatsConfig.pointPerLv + (skillStats.level * skillStats.level / 100);
        }
    }

    public void SetSkillItemEnhance(SkillStats skillStats, SObjSkillStatsConfig skillStatsConfig)
    {
        IncreasePointSkillItem(skillStats, skillStatsConfig);
        SaveData();
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
        if (skillStats.equipped) // update on Skill Item Equipped List
            skillItemEquippedManager.GetSkillItem(skillStats.equippedPosition - 1).SetEnhanceUI();

        UserInfoManager.Instance.SetATK();
    }

    private void SetAllSkillItemsEnhance()
    {
        // Disable button
        btnEnhanceAll.interactable = false;
        imgEnhanceAll.color = colorDisableBtn;

        for (int i = 0; i < skillStatsList.list.Count; i++)
        {
            IncreasePointSkillItem(skillStatsList.list[i], skillStatsConfigs[i]);
            SaveData();
            // Display on UI
            skillItems[i].SetEnhanceUI();
            if (skillStatsList.list[i].equipped)
                skillItemEquippedManager.GetSkillItem(skillStatsList.list[i].equippedPosition - 1).SetEnhanceUI();
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
        inHandleEquipSkillItemState = false;
        SetCoverGO(false);
        skillItemEquippedManager.SetAllChangeBtn(false);
    }

    public void OnClickEnhanceAll()
    {
        SetAllSkillItemsEnhance();
    }

    public void OnClickBuy()
    {
        // Go to shop
        BottomTab.Instance.OnClickTabBtn(3);
    }

    public void SetPanelActive(bool active)
    {
        // effect
        if (active)
        {
            gameObject.SetActive(true);
            TransformUIPanel();
        }
        else
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
        }
    }

    public void TransformUIPanel()
    {
        Vector2 startPos = rectTfm.anchoredPosition;
        startPos.y = -703;
        Vector2 endPos = startPos;
        endPos.y = startPos.y * -1;
        StartCoroutine(UITransformController.Instance.IEMovingRect(rectTfm, startPos, endPos, movingTime, LerpType.EaseOutBack));
    }

}

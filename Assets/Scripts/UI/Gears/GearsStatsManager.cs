using System;
using System.Collections.Generic;
using BigInteger = System.Numerics.BigInteger;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GearsStatsManager : MonoBehaviour, IBottomTabHandler
{
    [SerializeField] private RectTransform rectTfm;
    [SerializeField] private GameObject gObj;
    [SerializeField] private Transform tfmGearItemParent;
    [SerializeField] private GearItem gearItemPrefab;
    [SerializeField] private GearInfoUI gearInfoUI;
    [SerializeField] private GearConfig[] gearConfigs;
    [SerializeField] private GearModeConfig[] gearModeConfigs;
    [SerializeField] private Button btnWeapon, btnArmor, btnEnhanceAll;
    [SerializeField] private RectTransform rectTfmEquipped;
    [SerializeField] private TMP_Text txtTotalOwnedEffectValue, txtGearName;
    [SerializeField] private Color colorSelectedGearType, colorUnselectedGearType, colorDisableBtn, colorEnhanceAllBtn, colorBuyBtn;
    [SerializeField] private Image imgWeapon, imgArmor, imgEnhanceAll;

    [SerializeField] private float movingTime;

    private List<GearItem> gearItems = new List<GearItem>();
    private List<GearItem> gearItemsEnhance = new List<GearItem>();
    private Dictionary<GearType, List<GearStats>> gearStatsDic = new Dictionary<GearType, List<GearStats>>();
    private GearStats gearStatsEquipped;

    private void OnEnable()
    {
        SetGearItem(GearType.Weapon);
    }

    public void LoadGearsData(GearType gearType)
    {
        List<GearStats> gearStatsList = new List<GearStats>();
        GearConfig gearConfig = Array.Find(gearConfigs, config => config.type == gearType);
        for (int i = 0; i < gearConfig.gearsStatsConfigs.Length; i++)
        {
            GearStats gearStats = Db.ReadGearData(gearConfig.gearsStatsConfigs[i].gearName, gearConfig.gearsStatsConfigs[i].type);
            if (gearStats == null) // new user or new item
            {
                gearStats = new GearStats()
                {
                    name = gearConfig.gearsStatsConfigs[i].gearName,
                    level = 1,
                    numberOfPoints = 1,
                    totalPoint = gearConfig.gearsStatsConfigs[i].totalPointByXLv,
                    type = gearConfig.gearsStatsConfigs[i].type,
                    mode = gearConfig.gearsStatsConfigs[i].mode,
                    ownedEffect = gearConfig.gearsStatsConfigs[i].ownedEffect,
                    equippedEffect = gearConfig.gearsStatsConfigs[i].equippedEffect,
                    equipped = i <= 0, // when i = 0, it means the first item is newly initialized
                    unblocked = i <= 0, // as above
                };
                Db.SaveGearData(gearStats, gearStats.name, gearStats.type);
            }
            gearStatsList.Add(gearStats);
        }
        gearStatsDic.Add(gearType, gearStatsList);
    }

    private void SetGearItem(GearType gearType)
    {
        // set color
        imgWeapon.color = gearType == GearType.Weapon ? colorSelectedGearType : colorUnselectedGearType;
        imgArmor.color = gearType == GearType.Armor ? colorSelectedGearType : colorUnselectedGearType;
        // check button 
        btnWeapon.interactable = gearType == GearType.Armor;
        btnArmor.interactable = gearType == GearType.Weapon;
        // reset 
        btnEnhanceAll.interactable = false;
        imgEnhanceAll.color = colorDisableBtn;
        gearItemsEnhance.Clear();

        GearConfig gearConfig = Array.Find(gearConfigs, config => config.type == gearType);
        List<GearStats> gearStatsList = gearStatsDic[gearType];
        GearItem gearItem = null;
        for (int index = 0; index < gearConfig.gearsStatsConfigs.Length; index++)
        {
            if (gearItems.Count < index + 1) // spawn gear item if not spawned yet
            {
                gearItem = Instantiate(gearItemPrefab, tfmGearItemParent);
                gearItems.Add(gearItem);
            }
            // Init gear item
            gearItem = gearItems[index];
            gearItem.Init(gearStatsList[index], this, gearConfig.gearsStatsConfigs[index]);
            gearItem.SetTextLevel(gearStatsList[index].level.ToString());
            gearItem.SetGearBackGround(Array.Find(gearModeConfigs, config => config.mode == gearStatsList[index].mode).spt);
            gearItem.SetGearIcon(gearConfig.gearsStatsConfigs[index].gearSpt);
            if (gearStatsList[index].level == gearConfig.gearsStatsConfigs[index].levelMax)
            {
                gearItem.SetGearPointUI();
            }
            else
            {
                gearItem.SetGearPointUI(gearStatsList[index].numberOfPoints, gearStatsList[index].totalPoint);
            }
            // Check points to enhance later
            if (gearStatsList[index].numberOfPoints >= gearStatsList[index].totalPoint && gearStatsList[index].level < gearConfig.gearsStatsConfigs[index].levelMax)
            {
                gearItemsEnhance.Add(gearItem);
                btnEnhanceAll.interactable = true;
                imgEnhanceAll.color = colorEnhanceAllBtn;
            }
            gearItem.SetBlock(gearStatsList[index].unblocked);
            gearItems[index].gameObject.SetActive(true);
            if (gearStatsList[index].equipped)
            {
                gearStatsEquipped = gearStatsList[index];
                SetGearItemEquip(gearStatsEquipped, gearItem.transform, false);
            }
        }
        for (int i = gearConfig.gearsStatsConfigs.Length; i < gearItems.Count; i++)// if enable gearitem gameobject that is within range gearStatsConfigs
        {
            gearItems[i].gameObject.SetActive(false);
        }
        // Display total OE value UI
        SetTotalOwnedEffectValue();
    }

    public void SetGearInfoUI(GearStats gearStats, Sprite icon, GearItem gearItem, SObjGearsStatsConfig gearsStatsConfig)
    {
        GearConfig gearConfig = Array.Find(gearConfigs, config => config.type == gearStats.type);
        gearInfoUI.Init(gearStats, this, gearItem, gearsStatsConfig);
        gearInfoUI.SetTextName(gearStats.name);
        gearInfoUI.SetTextLevel(gearStats.level.ToString());
        gearInfoUI.SetGearIcon(icon);
        GearModeConfig modeConfig = Array.Find(gearModeConfigs, config => config.mode == gearStats.mode);
        gearInfoUI.SetGearBackGround(modeConfig.spt);
        gearInfoUI.SetMode(gearStats.mode, modeConfig.color);
        if (gearStats.level == gearsStatsConfig.levelMax)
        {
            gearInfoUI.SetGearPointUI();
            gearInfoUI.SetEnhaceBtn(false, true);
        }
        else
        {
            gearInfoUI.SetGearPointUI(gearStats.numberOfPoints, gearStats.totalPoint);
            gearInfoUI.SetEnhaceBtn(gearStats.numberOfPoints >= gearStats.totalPoint, false); // maybe enhance when current point >= total point
        }
        gearInfoUI.SetTxtOwnedEffect(gearStats.ownedEffect);
        gearInfoUI.SetTextEquippedEffect(gearStats.equippedEffect);
        gearInfoUI.SetEquipBtn(gearStats.equipped, gearStats.unblocked);
        gearInfoUI.SetActive(true);
    }

    public void SetGearItemEquip(GearStats newGearStatsEquipped, Transform tfmGearItem, bool pressed)
    {
        if (gearStatsEquipped != newGearStatsEquipped)
        {
            gearStatsEquipped.equipped = false;
            Db.SaveGearData(gearStatsEquipped, gearStatsEquipped.name, gearStatsEquipped.type);
            gearStatsEquipped = newGearStatsEquipped;
            Db.SaveGearData(gearStatsEquipped, gearStatsEquipped.name, gearStatsEquipped.type);
        }
        txtGearName.text = gearStatsEquipped.name;
        rectTfmEquipped.SetParent(tfmGearItem.transform);
        rectTfmEquipped.anchoredPosition = Vector2.zero;
        rectTfmEquipped.sizeDelta = Vector2.one * -32;
        if (!pressed)
            return;
        if (gearStatsEquipped.type == GearType.Weapon)
            UserInfoManager.Instance.SetATK();
        else
            UserInfoManager.Instance.SetHp();
    }

    /// <summary>
    /// When a gear item enhance successfully, remove it from list
    /// </summary>
    /// <param name="item"></param>
    public void RemoveGearItemEnhance(GearItem item)
    {
        if (gearItemsEnhance.Contains(item))
            gearItemsEnhance.Remove(item);
    }

    public void SetTotalOwnedEffectValue()
    {
        BigInteger totalOEValue = 0; // total owned effect value
        for (int i = 0; i < gearStatsDic[gearStatsEquipped.type].Count; i++)
        {
            if (gearStatsDic[gearStatsEquipped.type][i].unblocked)
                totalOEValue += BigInteger.Parse(gearStatsDic[gearStatsEquipped.type][i].ownedEffect);
        }
        string type = gearStatsEquipped.type == GearType.Weapon ? "ATK + " : "HP + ";
        txtTotalOwnedEffectValue.text = "Owned Effects: " + type + FillData.Instance.FormatNumber(totalOEValue) + "%";
    }

    public void OnClickEnhanceAll()
    {
        // Enhance all gear items in list then remove them
        while (gearItemsEnhance.Count > 0)
        {
            gearItemsEnhance[0].SetEnhance(true);
        }
    }

    public void OnClickGearItemDisplayBtn(int type)
    {
        SetGearItem((GearType)type);
    }

    /// <summary>
    /// Close gear info gameobject
    /// </summary>
    public void OnClickSpace()
    {
        gearInfoUI.SetActive(false);
    }

    public void OnClickBuy()
    {
        // Go to shop
        BottomTab.Instance.OnClickTabBtn(3);
    }

    public BigInteger GetAllOwnedEffect(GearType type)
    {
        BigInteger damagePercent = 0;
        for (int index = 0; index < gearStatsDic[type].Count; index++)
        {
            GearStats stats = gearStatsDic[type][index];
            if (stats.unblocked)
            {
                damagePercent += BigInteger.Parse(stats.ownedEffect);
                damagePercent += stats.equipped ? BigInteger.Parse(stats.equippedEffect) : 0;
            }
        }
        return damagePercent;
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
        startPos.y = -752;
        Vector2 endPos = startPos;
        endPos.y = startPos.y * -1;
        StartCoroutine(UITransformController.Instance.IEMovingRect(rectTfm, startPos, endPos, movingTime, LerpType.EaseOutBack));
    }

}

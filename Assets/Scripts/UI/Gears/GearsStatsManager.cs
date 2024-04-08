using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GearsStatsManager : MonoBehaviour, IBottomTabHandler
{
    private class IndividualGearStats
    {
        public GearStats gearStatsEquipped;
        public int totalOEDamage; // owned effects damage
    }

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

    private IndividualGearStats[] individualGearStats = new IndividualGearStats[2] { new IndividualGearStats(), new IndividualGearStats() }; // index 0 is Weapon, 1 is Armor 
    private List<GearItem> gearItems = new List<GearItem>();
    private List<GearItem> gearItemsEnhance = new List<GearItem>();
    private Dictionary<GearType, GearStatsList> gearStatsDic = new Dictionary<GearType, GearStatsList>();

    private readonly string DATAKEY = "GEARSTATSLISTDATA";

    private void OnEnable()
    {
        SetGearItem(GearType.Weapon);
    }

    public void SaveData(GearType gearType)
    {
        PlayerPrefs.SetString(gearType + DATAKEY, JsonUtility.ToJson(gearStatsDic[gearType]));
    }

    public void LoadGearsData(GearType gearType)
    {
        GearStatsList gearStatsList = new GearStatsList();
        GearConfig gearConfig = Array.Find(gearConfigs, config => config.type == gearType);
        SObjGearsStatsConfig gearStatsConfig = gearConfig.gearsStatsConfigs[0];
        var json = PlayerPrefs.GetString(gearStatsConfig.type + DATAKEY, null);
        gearStatsList = JsonUtility.FromJson<GearStatsList>(json);
        if (gearStatsList == null) // new user
        {
            GearStats gearStats = new GearStats()
            {
                name = gearStatsConfig.gearName,
                level = 1,
                numberOfPoints = 1,
                totalPoint = gearStatsConfig.pointPerLv + (1 * gearStatsConfig.maxPercentLevel / 100),
                type = gearStatsConfig.type,
                mode = gearStatsConfig.mode,
                ownedEffect = gearStatsConfig.firstOwnedEffect,
                equippedEffect = gearStatsConfig.firstEquippedEffect,
                equipped = true,
                unblocked = true,
                position = 1,
            };
            gearStatsList = new GearStatsList();
            gearStatsList.list.Add(gearStats);
            individualGearStats[(int)gearType].gearStatsEquipped = gearStats;
        }
        else
        {
            individualGearStats[(int)gearType].gearStatsEquipped = gearStatsList.list.Find(stats => stats.equipped);
        }
        gearStatsDic.Add(gearType, gearStatsList);
        SaveData(gearType);
        // Display total OE value UI
        SetTotalOwnedEffectValue(gearType);
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
        GearStatsList gearStatsList = gearStatsDic[gearType];
        int statsListIndex = 0;
        for (int index = 0; index < gearConfig.gearsStatsConfigs.Length; index++)
        {
            GearItem gearItem = null;
            if (gearItems.Count < index + 1) // spawn gear item if not spawned yet
            {
                gearItem = Instantiate(gearItemPrefab, tfmGearItemParent);
                gearItems.Add(gearItem);
            }
            GearStats gearStats = null;
            SObjGearsStatsConfig gearStatsConfig = gearConfig.gearsStatsConfigs[index];

            if (statsListIndex < gearStatsList.list.Count)
            {
                if (index + 1 == gearStatsList.list[statsListIndex].position) // gears owned is in gear item list
                {
                    gearStats = gearStatsList.list[statsListIndex];
                    statsListIndex++;
                }
            }
            if (gearStats == null)
            {
                gearStats = new GearStats()
                {
                    name = gearStatsConfig.gearName,
                    level = 1,
                    numberOfPoints = 0,
                    totalPoint = gearStatsConfig.pointPerLv + (1 * gearStatsConfig.maxPercentLevel / 100),
                    type = gearStatsConfig.type,
                    mode = gearStatsConfig.mode,
                    ownedEffect = gearStatsConfig.firstOwnedEffect,
                    equippedEffect = gearStatsConfig.firstEquippedEffect,   
                    equipped = false,
                    unblocked = false,
                    position = index + 1
                };
            }
            // Init gear item
            gearItem = gearItems[index];
            gearItem.Init(gearStats, this, gearConfig.gearsStatsConfigs[index]);
            gearItem.SetTextLevel(gearStats.level.ToString());
            gearItem.SetGearBackGround(Array.Find(gearModeConfigs, config => config.mode == gearStats.mode).spt);
            gearItem.SetGearIcon(gearStatsConfig.gearSpt);
            if (gearStats.level == gearStatsConfig.levelMax)
            {
                gearItem.SetGearPointUI();
            }
            else
            {
                gearItem.SetGearPointUI(gearStats.numberOfPoints, gearStats.totalPoint);
            }
            // Check points to enhance later
            if (gearStats.numberOfPoints >= gearStats.totalPoint && gearStats.level < gearStatsConfig.levelMax)
            {
                gearItemsEnhance.Add(gearItem);
                btnEnhanceAll.interactable = true;
                imgEnhanceAll.color = colorEnhanceAllBtn;
            }
            gearItem.SetBlock(gearStats.unblocked);
            gearItems[index].gameObject.SetActive(true);

            if (gearStats.equipped)
                SetGearItemEquip(gearItem, false, gearType);
        }
        for (int i = gearConfig.gearsStatsConfigs.Length; i < gearItems.Count; i++)// if enable gearitem gameobject that is within range gearStatsConfigs
        {
            gearItems[i].gameObject.SetActive(false);
        }
        // Show Owned Effect on UI
        SetOEUI(gearType);
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

    public void SetGearItemEquip(GearItem gearItem, bool pressed, GearType gearType, GearStats newGearStatsEquipped = null)
    {
        if (newGearStatsEquipped != null)
        {
            individualGearStats[(int)gearType].gearStatsEquipped.equipped = false;
            individualGearStats[(int)gearType].gearStatsEquipped = newGearStatsEquipped;
            SaveData(individualGearStats[(int)gearType].gearStatsEquipped.type);
        }
        txtGearName.text = individualGearStats[(int)gearType].gearStatsEquipped.name;
        rectTfmEquipped.gameObject.SetActive(true);
        rectTfmEquipped.SetParent(gearItem.GetTransform());
        rectTfmEquipped.anchoredPosition = Vector2.zero;
        rectTfmEquipped.sizeDelta = Vector2.zero;
        if (!pressed)
            return;
        if (gearType == GearType.Weapon)
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

    public void SetTotalOwnedEffectValue(GearType gearType)
    {
        for (int i = 0; i < gearStatsDic[gearType].list.Count; i++)
        {
            if (gearStatsDic[gearType].list[i].unblocked)
                individualGearStats[(int)gearType].totalOEDamage += gearStatsDic[gearType].list[i].ownedEffect;
        }
    }

    /// <summary>
    /// Set total with one gear item is enhanced
    /// </summary>
    /// <param name="ownedEffect"></param>
    /// <param name="additional"></param>
    public void SetTotalOwnedEffectValue(int ownedEffect, bool additional, GearType type)
    {
        if (additional)
            individualGearStats[(int)type].totalOEDamage += ownedEffect;
        else
            individualGearStats[(int)type].totalOEDamage -= ownedEffect;

        SetOEUI(type);
    }

    public int GetAllEffectDamage(GearType type)
    {
        return individualGearStats[(int)type].gearStatsEquipped.equippedEffect + individualGearStats[(int)type].totalOEDamage;
    }

    public void SetOEUI(GearType gearType)
    {
        string type = gearType == GearType.Weapon ? "ATK + " : "HP + ";
        txtTotalOwnedEffectValue.text = "Owned Effects: " + type + FillData.Instance.FormatNumber(individualGearStats[(int)gearType].totalOEDamage) + "%";
    }

    public void OnClickEnhanceAll()
    {
        // Disable button
        btnEnhanceAll.interactable = false;
        imgEnhanceAll.color = colorDisableBtn;
        // Enhance all gear items in list
        while (gearItemsEnhance.Count > 0)
        {
            gearItemsEnhance[0].SetEnhance();
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

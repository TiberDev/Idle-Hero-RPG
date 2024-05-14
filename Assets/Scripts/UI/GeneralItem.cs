using System;
using BigInteger = System.Numerics.BigInteger;
using TMPro;
using UnityEngine;

public class GeneralItem : MonoBehaviour
{
    [SerializeField] private TMP_Text txtLv, txtStat, txtGold;
    [SerializeField] private HoldGStatsButton holdGStatsButton;
    [SerializeField] private GeneralManager generalManager;
    [SerializeField] private string unit;

    private Transform cachedTfm;
    private GeneralStat generalStat;
    private SObjGeneralStatsConfig generalStatsConfig;

    private BigInteger level, bigValue, gold;

    private float smallValue;

    private void Awake()
    {
        cachedTfm = transform;
    }

    private void OnEnable()
    {
        EventDispatcher.Register(EventId.CheckGoldToEnhance, CheckGoldEnough);
    }

    private void OnDisable()
    {
        EventDispatcher.RemoveCallback(EventId.CheckGoldToEnhance, CheckGoldEnough);
    }

    private void Start()
    {
        holdGStatsButton.EnhanceAction = EnhanceItem;
    }

    private void CheckMaxLevel(bool enhance)
    {
        if (generalStatsConfig.isValueSmall && generalStatsConfig.levelMax <= level)
        {
            // block button
            holdGStatsButton.SetMaxLv(true);
            if (enhance)
                generalManager.ChangeGeneralItemPostion(this, generalStat);
        }
    }

    private void SetTxtLevel(BigInteger lv)
    {
        txtLv.text = "Level " + lv.ToString();
    }


    private void SetTextGold(BigInteger gold)
    {
        txtGold.text = NumberConverter.Instance.FormatNumber(gold);
    }

    private void SetTxtStat(BigInteger stat)
    {
        txtStat.text = NumberConverter.Instance.FormatNumber(stat) + unit;
    }

    private void SetTxtStat(float stat)
    {
        txtStat.text = stat.ToString() + unit;
    }

    private void CheckGoldEnough(object _gold)
    {
        BigInteger ownedGold = (BigInteger)_gold;
        if (ownedGold < gold) // can't enhance
        {
            holdGStatsButton.SetInteractive(false);
        }
        else
        {
            holdGStatsButton.SetInteractive(true);
        }
    }

    public void SetStat(GeneralStat _stat, SObjGeneralStatsConfig config)
    {
        generalStat = _stat;
        generalStatsConfig = config;
        level = BigInteger.Parse(generalStat.level);
        CheckMaxLevel(false);
        gold = BigInteger.Parse(generalStat.gold);
        if (generalStatsConfig.isValueSmall)
        {
            smallValue += float.Parse(generalStat.stat);
            smallValue = (float)Math.Round(smallValue, 2);
            SetTxtStat(smallValue);
        }
        else
        {
            bigValue += BigInteger.Parse(generalStat.stat);
            SetTxtStat(bigValue);
        }
        SetTxtLevel(level);
        SetTextGold(gold);
    }

    public void EnhanceItem()
    {
        // increase lv, stat, gold
        level += 1;
        // check max level
        CheckMaxLevel(true);
        BigInteger enhanceGold = gold;
        gold += Mathf.RoundToInt(generalStatsConfig.goldPerLv);
        if (generalStatsConfig.isValueSmall)
        {
            smallValue += generalStatsConfig.statPerLv;
            smallValue = (float)Math.Round(smallValue, 2);
            generalStat.stat = smallValue.ToString();
            SetTxtStat(smallValue);
        }
        else
        {
            bigValue += Mathf.RoundToInt(generalStatsConfig.statPerLv);
            generalStat.stat = bigValue.ToString();
            SetTxtStat(bigValue);
        }
        generalStat.level = level.ToString();
        generalStat.gold = gold.ToString();
        SetTxtLevel(level);
        SetTextGold(gold);
        // save db
        generalManager.ChangeGeneralData(enhanceGold, generalStat);
    }

    public Transform GetTransform() => cachedTfm;
}

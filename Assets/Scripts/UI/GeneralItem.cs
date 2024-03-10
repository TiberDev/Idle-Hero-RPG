using System;
using System.Numerics;
using TMPro;
using UnityEngine;

public class GeneralItem : MonoBehaviour
{
    [SerializeField] private TMP_Text txtLv, txtStat, txtGold;
    [SerializeField] private HoldGStatsButton holdGStatsButton;
    [SerializeField] private GeneralManager GeneralManager;
    [SerializeField] private string unit;

    private BigInteger level, bigValue, gold;
    private GeneralStat generalStat;
    private SObjGeneralStatsConfig generalStatsConfig;

    private float smallValue;

    private void Awake()
    {
        EventDispatcher.Register(EventId.CheckGoldToEnhance, CheckGoldEnough);
    }

    private void OnDestroy()
    {
        EventDispatcher.RemoveCallback(EventId.CheckGoldToEnhance, CheckGoldEnough);
    }

    private void CheckMaxLevel()
    {
        if (generalStatsConfig.levelMax > 0 && generalStatsConfig.levelMax <= level) // levelMax <= 0 means item doesn't need check max level
        {
            // block button
            holdGStatsButton.SetMaxLv();
        }
    }

    public void SetStat(GeneralStat _stat, SObjGeneralStatsConfig config)
    {
        generalStat = _stat;
        generalStatsConfig = config;
        level = BigInteger.Parse(generalStat.level);
        CheckMaxLevel();
        gold = BigInteger.Parse(generalStat.gold);
        if (generalStatsConfig.isValueSmall)
        {
            smallValue += float.Parse(generalStat.stat);
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

    public void SetTxtLevel(BigInteger lv)
    {
        txtLv.text = "Level " + lv.ToString();
    }

    public void SetTextGold(BigInteger gold)
    {
        txtGold.text = FillData.Instance.FormatNumber(gold);
    }

    public void SetTxtStat(BigInteger stat)
    {
        txtStat.text = FillData.Instance.FormatNumber(stat) + unit;
    }

    public void SetTxtStat(float stat)
    {
        txtStat.text = stat.ToString() + unit;
    }

    public void CheckGoldEnough(object _gold)
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

    public void EnhanceItem()
    {
        // increase lv, stat, gold
        level += Mathf.RoundToInt(generalStatsConfig.levelEnhance);
        // check max level
        CheckMaxLevel();
        BigInteger enhanceGold = gold;
        gold += Mathf.RoundToInt(generalStatsConfig.goldPerLv);
        if (generalStatsConfig.isValueSmall)
        {
            smallValue += generalStatsConfig.statPerLv;
            generalStat.stat = smallValue.ToString();
            smallValue = (float)Math.Round(smallValue, 2);
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
        GeneralManager.ChangeGeneralData(enhanceGold,generalStat);
    }
}

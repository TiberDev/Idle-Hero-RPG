using System.Collections.Generic;
using BigInteger = System.Numerics.BigInteger;
using UnityEngine;

public class GeneralManager : MonoBehaviour
{
    [SerializeField] private GeneralItem[] generalItems;
    [SerializeField] private SObjGeneralStatsConfig[] generalStatsConfigs;

    private List<int> maximumItemIndexList = new List<int>();
    private GeneralStatList generalStatList = null;

    private int numberOfItemsMax;
    private readonly string DATAKEY = "GENERALSTATLISTDATA";

    private void Start()
    {
        EventDispatcher.Push(EventId.CheckGoldToEnhance, GameManager.Instance.GetGold());
    }

    private void SaveData()
    {
        PlayerPrefs.SetString(DATAKEY, JsonUtility.ToJson(generalStatList));
    }

    public void LoadGeneralData()
    {
        var json = PlayerPrefs.GetString(DATAKEY, null);
        generalStatList = JsonUtility.FromJson<GeneralStatList>(json);
        if (generalStatList == null)
            generalStatList = new GeneralStatList();

        for (int i = 0; i < generalStatsConfigs.Length; i++)
        {
            GeneralStat generalStat = null;
            if (json == null || json == "") // new user
            {
                generalStat = new GeneralStat()
                {
                    name = generalStatsConfigs[i].statName,
                    level = "1",
                    gold = generalStatsConfigs[i].goldLv1,
                    stat = generalStatsConfigs[i].statLv1,
                    type = generalStatsConfigs[i].type,
                };
                generalStatList.list.Add(generalStat);
            }
            else
            {
                generalStat = generalStatList.list[i];
            }
            // Check level max
            if (generalStatsConfigs[i].isValueSmall && int.Parse(generalStat.level) >= generalStatsConfigs[i].levelMax) // some stats have a maximum level
            {
                generalItems[i].GetTransform().SetSiblingIndex(generalStat.positionIndex);
                maximumItemIndexList.Add(i);
                numberOfItemsMax++;
            }
            // Set item
            generalItems[i].SetStat(generalStat, generalStatsConfigs[i]);
        }
    }

    public void ChangeGeneralData(BigInteger enhanceGold, GeneralStat generalStat)
    {
        SaveData();
        GameManager.Instance.SetGold(GameManager.Instance.GetGold() - enhanceGold);
        EventDispatcher.Push(EventId.CheckGoldToEnhance, GameManager.Instance.GetGold());
        SetUserInfo(generalStat.type);
    }

    public void ChangeGeneralItemPostion(GeneralItem generalItem, GeneralStat generalStat)
    {
        int changingPositionIndex = generalStatsConfigs.Length - numberOfItemsMax - 1;
        generalStat.positionIndex = changingPositionIndex;
        generalItem.GetTransform().SetSiblingIndex(changingPositionIndex);
        numberOfItemsMax++;
    }

    public void SetUserInfo(GeneralStatsType type)
    {
        UserInfoManager userInfoManager = UserInfoManager.Instance;
        switch (type)
        {
            case GeneralStatsType.Attack:
                userInfoManager.SetATK();
                break;
            case GeneralStatsType.AttackSpeed:
                userInfoManager.SetATKSpeed();
                break;
            case GeneralStatsType.MaxHp:
                userInfoManager.SetHp();
                break;
            case GeneralStatsType.HpRecovery:
                userInfoManager.SetHPRecovery();
                break;
            case GeneralStatsType.CriticalHitChance:
                userInfoManager.SetCriticalHitChance();
                break;
            case GeneralStatsType.CriticalHitDamage:
                userInfoManager.SetCriticalHitDamage();
                break;
        }
    }

    /// <summary>
    /// Get values: atk, hp, critical hit damage
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public BigInteger GetBigValue(GeneralStatsType type)
    {
        GeneralStat generalStat = generalStatList.list[(int)type]/*.Find(stat => stat.type == type)*/;
        return BigInteger.Parse(generalStat.stat);
    }

    /// <summary>
    /// Get values: atk speed, hp recovery, critical hit chance
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public float GetSmallValue(GeneralStatsType type)
    {
        GeneralStat generalStat = generalStatList.list[(int)type];
        return float.Parse(generalStat.stat);
    }

}

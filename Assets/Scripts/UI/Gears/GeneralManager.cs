using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class GeneralManager : MonoBehaviour
{
    [SerializeField] private GeneralItem[] generalItems;
    [SerializeField] private SObjGeneralStatsConfig[] generalStatsConfigs;

    private UserInfoManager userInfoManager;
    private List<GeneralStat> generalStats = new List<GeneralStat>();

    private void Start()
    {
        userInfoManager = UserInfoManager.Instance;
        EventDispatcher.Push(EventId.CheckGoldToEnhance, GameManager.Instance.GetGold());
    }

    public void LoadGeneralData()
    {
        for (int i = 0; i < generalStatsConfigs.Length; i++)
        {
            GeneralStat generalStat = Db.ReadGeneralData(generalStatsConfigs[i].statName);
            if (generalStat == null) // new user
            {
                generalStat = new GeneralStat()
                {
                    name = generalStatsConfigs[i].statName,
                    level = "1",
                    gold = generalStatsConfigs[i].goldLv1,
                    stat = generalStatsConfigs[i].statLv1,
                    type = generalStatsConfigs[i].type,
                };
                Db.SaveGeneralData(generalStat, generalStat.name);
            }
            generalStats.Add(generalStat);
            // Set item
            generalItems[i].SetStat(generalStat, generalStatsConfigs[i]);
        }
    }

    public void ChangeGeneralData(BigInteger enhanceGold, GeneralStat generalStat)
    {
        Db.SaveGeneralData(generalStat, generalStat.name);
        GameManager.Instance.SetGold(GameManager.Instance.GetGold() - enhanceGold);
        EventDispatcher.Push(EventId.CheckGoldToEnhance, GameManager.Instance.GetGold());
        SetUserInfo(generalStat.type);
    }

    public void SetUserInfo(GeneralStatsType type)
    {
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
        GeneralStat generalStat = generalStats.Find(stat => stat.type == type);
        return BigInteger.Parse(generalStat.stat);
    }
    
    /// <summary>
    /// Get values: atk speed, hp recovery, critical hit chance
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public float GetSmallValue(GeneralStatsType type)
    {
        GeneralStat generalStat = generalStats.Find(stat => stat.type == type);
        return float.Parse(generalStat.stat);
    }

}

using System.Collections;
using BigInteger = System.Numerics.BigInteger;
using TMPro;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private HeroStatsManager heroStatsManager;
    [SerializeField] private GearsStatsManager gearsStatsManager;
    [SerializeField] private SkillStatsManager skillStatsManager;
    [SerializeField] private CharacterHpBar turnBar;
    [SerializeField] private TMP_Text txtGold, txtGem;

    public CharacterHpBar GetTurnBar()
    {
        return turnBar;
    }

    public void SetTextGold(BigInteger gold)
    {
        txtGold.text = FillData.Instance.FormatNumber(gold);
    }

    public void SetTextGem(BigInteger gem)
    {
        txtGem.text = FillData.Instance.FormatNumber(gem);
    }

}

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
    [SerializeField] private TMP_Text txtGold, txtPinkGem, txtBlueGem;

    public CharacterHpBar GetTurnBar()
    {
        return turnBar;
    }

    public void SetTextGold(BigInteger gold)
    {
        txtGold.text = FillData.Instance.FormatNumber(gold);
    }

    public void SetTextBlueGem(BigInteger gem)
    {
        txtBlueGem.text = FillData.Instance.FormatNumber(gem);
    }

    public void SetTextPinkGem(BigInteger gem)
    {
        txtPinkGem.text = FillData.Instance.FormatNumber(gem);
    }

}

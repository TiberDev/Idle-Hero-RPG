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

    private BigInteger goldUI, blueGemUI, pinkGemUI;

    public CharacterHpBar GetTurnBar()
    {
        return turnBar;
    }

    public void SetTextGold(BigInteger gold, bool addtional)
    {
        goldUI += addtional ? gold : -gold;
        txtGold.text = NumberConverter.Instance.FormatNumber(goldUI);
        EventDispatcher.Push(EventId.CheckGoldToEnhance, goldUI);
    }

    public void SetTextBlueGem(BigInteger gem, bool addtional)
    {
        blueGemUI += addtional ? gem : -gem;
        txtBlueGem.text = NumberConverter.Instance.FormatNumber(blueGemUI);
    }

    public void SetTextPinkGem(BigInteger gem, bool additional)
    {
        pinkGemUI += additional ? gem : -gem;
        txtPinkGem.text = NumberConverter.Instance.FormatNumber(pinkGemUI);
    }

}

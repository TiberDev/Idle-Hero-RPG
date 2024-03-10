using System.Numerics;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
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

    public void PressHeroStatsBtn()
    {
        heroStatsManager.gameObject.SetActive(!heroStatsManager.gameObject.activeInHierarchy);
        gearsStatsManager.gameObject.SetActive(false);
        skillStatsManager.gameObject.SetActive(false);
    }

    public void PressGearsStatBtn()
    {
        gearsStatsManager.gameObject.SetActive(!gearsStatsManager.gameObject.activeInHierarchy);
        heroStatsManager.gameObject.SetActive(false);
        skillStatsManager.gameObject.SetActive(false);
    }

    public void PressSkillStatsBtn()
    {
        skillStatsManager.gameObject.SetActive(!skillStatsManager.gameObject.activeInHierarchy);
        heroStatsManager.gameObject.SetActive(false);
        gearsStatsManager .gameObject.SetActive(false);
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

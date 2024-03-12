using TMPro;
using UnityEngine;

public class SkillTable : MonoBehaviour
{
    [SerializeField] private SkillTableItem[] skillTableItemList;
    [SerializeField] private Sprite sptLock;
    [SerializeField] private SkillStatsManager skillStatsManager;
    [SerializeField] private TMP_Text txtAuto;

    private bool automatic;

    public bool Automatic { get => automatic; }

    public void GetAutomaticData()
    {
        // 0 : false
        // 1 : true
        //automatic = PlayerPrefs.GetInt("AUTOMATIC", 0) == 0 ? false : true;
    }

    public void UnlockSkillTblItem(int index, bool unlock)
    {
        skillTableItemList[index].SetImageIcon(unlock, sptLock);
        skillTableItemList[index].SetInteractButton(unlock);
        skillTableItemList[index].ResetChildComponents();
        skillTableItemList[index].SetSkillTable(this);
    }

    public void SetSkillTableItem(int index, SkillStats skillStats, SObjSkillStatsConfig config)
    {
        skillTableItemList[index].SetInteractButton(true);
        skillTableItemList[index].InitSkillTblItem(skillStats, config);
        skillTableItemList[index].SetImageIcon(false, config.skillSpt);
        skillTableItemList[index].ResetChildComponents();
        if (automatic)
            skillTableItemList[index].ExecuteSkill();
    }

    public void SetSkillTblItemEmpty(int index)
    {
        skillTableItemList[index].InitSkillTblItem(null, null);
        skillTableItemList[index].SetImageIcon(true);
        skillTableItemList[index].SetInteractButton(false);
        skillTableItemList[index].ResetChildComponents();
    }

    public SkillStatsManager GetSkillStatsManager() => skillStatsManager;

    public void ResetAllSkillTableItem()
    {
        for (int i = 0; i < skillTableItemList.Length; i++)
        {
            skillTableItemList[i].ResetExecutingSkill();
        }
    }

    public void HandleAutomatic()
    {
        txtAuto.text = automatic ? "STOP" : "AUTO";
        if (automatic)
        {
            for (int i = 0; i < skillTableItemList.Length; i++)
            {
                skillTableItemList[i].ExecuteSkill();
            }
        }
    }

    public void PressAutoBtn()
    {
        automatic = !automatic;
        PlayerPrefs.SetInt("AUTOMATIC", automatic ? 1 : 0);
        HandleAutomatic();
    }

}

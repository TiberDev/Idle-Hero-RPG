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
        automatic = PlayerPrefs.GetInt("AUTOMATIC", 0) == 0 ? false : true;
        txtAuto.text = automatic ? "STOP" : "AUTO";
    }

    public void UnlockSkillTblItem(int index, bool unlock)
    {
        skillTableItemList[index].SetImageIcon(unlock, sptLock);
        skillTableItemList[index].ResetExecutingSkill();
        skillTableItemList[index].SetSkillTable(this);
    }

    public void SetSkillTableItem(int index, SkillStats skillStats, SObjSkillStatsConfig config, bool cooldown)
    {
        skillTableItemList[index].Init(skillStats, config);
        skillTableItemList[index].SetImageIcon(false, config.skillSpt);
        skillTableItemList[index].ResetExecutingSkill();
        if (cooldown)
            skillTableItemList[index].SetAmountCoolDown();
    }

    public void SetSkillTblItemEmpty(int index)
    {
        skillTableItemList[index].Init(null, null);
        skillTableItemList[index].SetImageIcon(true);
        skillTableItemList[index].ResetExecutingSkill();
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
        txtAuto.text = automatic ? "STOP" : "AUTO";
        PlayerPrefs.SetInt("AUTOMATIC", automatic ? 1 : 0);
        if (automatic && !BoxScreenCollision.Instance.IsEnenmiesEmpty())
        {
            HandleAutomatic();
        }
    }
}

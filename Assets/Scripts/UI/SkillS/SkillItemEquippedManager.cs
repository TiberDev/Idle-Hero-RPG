using UnityEngine;

public class SkillItemEquippedManager : MonoBehaviour
{
    [SerializeField] private SkillStatsManager skillStatsManager;
    [SerializeField] private SkillItem[] skillItems;
    [SerializeField] private GameObject[] gObjLocks;
    [SerializeField] private GameObject[] gObjChangeSkillItems;
    [SerializeField] private GameObject[] gObjPointer;
    [SerializeField] private int[] conditions;
    [SerializeField] private int map; // test


    /// <summary>
    /// Init skill item with index is position on equipped item panel
    /// </summary>
    public void SetSkillItemEquipped(int index, SkillStats skillStats, SObjSkillStatsConfig skillStatsConfig, SkillStatsManager statsManager)
    {
        // Init skill item 
        SkillItem skillItem = skillItems[index];
        skillItem.Init(skillStats, statsManager, skillStatsConfig, true);
        skillItem.SetTextLevel(skillStats.level.ToString());
        skillItem.SetSkillIcon(skillStatsConfig.skillSpt);
        skillItem.SetUnlock(true);
        if (skillStats.level == skillStatsConfig.levelMax)
        {
            skillItem.SetSkillPointUI();
        }
        else
        {
            skillItem.SetSkillPointUI(skillStats.numberOfPoints, skillStats.totalPoint);
        }
        skillItem.SetEquipped_RemoveImage(skillStats.equipped);
        skillItems[index].gameObject.SetActive(true);
    }

    public void SetSkillItemEmpty(int index)
    {
        skillItems[index].gameObject.SetActive(false);
    }

    public void CheckLock()
    {
        // Lock all equipped item
        for (int i = 0; i < skillItems.Length; i++)
        {
            gObjLocks[i].SetActive(conditions[i] >= map);
            skillItems[i].gameObject.SetActive(false);
        }
    }

    public int GetPositionEmpty()
    {
        for (int i = 0; i < skillItems.Length; i++)
        {
            if (!skillItems[i].gameObject.activeInHierarchy && !gObjLocks[i].activeInHierarchy)
            {
                return i;
            }
        }
        return -1;
    }

    public void SetAllChangeBtn(bool isActive)
    {
        for (int i = 0; i < gObjChangeSkillItems.Length; i++)
        {
            gObjChangeSkillItems[i].SetActive(isActive);
            gObjPointer[i].SetActive(isActive);
            skillItems[i].SetEquipped_RemoveGOActive(!isActive);
        }
    }

    public SkillItem GetSkillItem(int index)
    {
        return skillItems[index];
    }

    public void OnClickChangeBtn(int index)
    {
        skillStatsManager.HandleEquipSkillItemCallBack(skillItems[index].skillStats, index);
    }
}

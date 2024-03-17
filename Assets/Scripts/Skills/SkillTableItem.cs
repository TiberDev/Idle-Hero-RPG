using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTableItem : MonoBehaviour
{
    [SerializeField] private Image imgIcon;
    [SerializeField] private Image amountCoolDown, amountCoolDownExisting;
    [SerializeField] private Button btn;
    [SerializeField] private TMP_Text txtCounter;

    private SkillStats skillStats;
    private SObjSkillStatsConfig skillStatsConfig;
    private SkillTable skillTable;
    private Skill skill;

    public void SetSkillTable(SkillTable table)
    {
        skillTable = table;
    }

    public void Init(SkillStats stats, SObjSkillStatsConfig config)
    {
        skillStats = stats;
        skillStatsConfig = config;
    }
    public void SetTextCounter(float time)
    {
        txtCounter.text = ((int)time + 1).ToString();
    }

    public void SetTextCounter()
    {
        txtCounter.text = "";
    }

    public void SetAmountCoolDown()
    {
        SetInteractButton(false);
        StartCoroutine(IECoolDown());
    }

    public void SetAmountExistingCoolDown(float totalTime)
    {
        StartCoroutine(IEExistingCounter(totalTime));
    }

    private IEnumerator IEExistingCounter(float totalTime)
    {
        amountCoolDown.fillAmount = 1;
        float curTime = totalTime;
        while (curTime > 0)
        {
            curTime -= Time.deltaTime;
            amountCoolDownExisting.fillAmount = curTime / totalTime;
            SetTextCounter(curTime);
            yield return null;
        }
        skill.EndExistence();
        SetAmountCoolDown();
    }

    private IEnumerator IECoolDown()
    {
        float timeCoolDown = skillStatsConfig.cooldown;
        float curTime = timeCoolDown;
        while (curTime > 0)
        {
            curTime -= Time.deltaTime;
            amountCoolDown.fillAmount = curTime / timeCoolDown;
            SetTextCounter(curTime);
            yield return null;
        }
        // finish executing skill
        SetInteractButton(true);
        SetTextCounter();
        if (skillTable.Automatic && !BoxScreenCollision.Instance.IsEnenmiesEmpty())
        {
            ExecuteSkill();
        }
    }

    public void SetImageIcon(bool empty, Sprite sptIcon = null)
    {
        if (empty)
            imgIcon.gameObject.SetActive(false);
        else
        {
            imgIcon.gameObject.SetActive(true);
            imgIcon.sprite = sptIcon;
        }
    }

    private void SetInteractButton(bool interactable)
    {
        btn.interactable = interactable;
    }

    public void ExecuteSkill()
    {
        if (!btn.interactable || skillStats == null) // skill is executing or emprty or locked
             return;

        SetInteractButton(false);
        skill = ObjectPooling.Instance.SpawnG0InPool(skillStatsConfig.prefab.gameObject, Vector3.one, PoolType.Skill).GetComponent<Skill>();
        skill.Init(this);
        skill.SetValue(skillStatsConfig.damage);
        skill.Execute();
    }

    public void ResetExecutingSkill()
    {
        //if (skillStatsConfig == null) // item is null
        //    return;

        if (skill != null)
            skill.EndExistence();

        if (!imgIcon.gameObject.activeInHierarchy || imgIcon.sprite == skillStatsConfig.skillSpt)
        {
            SetInteractButton(true);
        }
        else
        {
            SetInteractButton(false);
        }
        StopAllCoroutines();
        amountCoolDown.fillAmount = 0;
        amountCoolDownExisting.fillAmount = 0;
        txtCounter.text = "";
    }

    public void PressSkillBtn()
    {
        if (imgIcon.gameObject.activeInHierarchy)
        {
            ExecuteSkill();
        }
        else
        {
            skillTable.GetSkillStatsManager().gameObject.SetActive(true);
        }
    }
}

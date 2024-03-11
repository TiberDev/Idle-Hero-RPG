using System.Collections;
using System.Runtime.ConstrainedExecution;
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

    private bool isExecutingSkill;

    public void SetSkillTable(SkillTable table)
    {
        skillTable = table;
    }

    public void InitSkillTblItem(SkillStats stats, SObjSkillStatsConfig config)
    {
        skillStats = stats;
        skillStatsConfig = config;
    }

    public void SetAmountExistingCoolDown(float cur, float total)
    {
        amountCoolDownExisting.fillAmount = cur / total;
        SetTextCounter(cur);
    }

    public void SetAmountCoolDown()
    {
        StartCoroutine(IECoolDown());
    }

    public void SetAmountExistingCoolDown(float totalTime)
    {
        StartCoroutine(IEExistingCounter(totalTime));
    }

    public void SetTextCounter(float time)
    {
        txtCounter.text = ((int)time + 1).ToString();
    }

    public void SetTextCounter()
    {
        txtCounter.text = "";
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
        isExecutingSkill = false;
        SetInteractButton(true);
        SetTextCounter();
        if (skillTable.Automatic)
            ExecuteSkill();
    }

    public void SetImageIcon(bool empty, Sprite sprite = null)
    {
        if (empty)
            imgIcon.gameObject.SetActive(false);
        else
        {
            imgIcon.gameObject.SetActive(true);
            imgIcon.sprite = sprite;
        }
    }

    public void SetInteractButton(bool interactable)
    {
        btn.interactable = interactable;
    }

    public void ResetChildComponents()
    {
        amountCoolDown.fillAmount = 0;
        amountCoolDownExisting.fillAmount = 0;
        txtCounter.text = "";
        StopAllCoroutines();
    }

    public void ExecuteSkill()
    {
        if (isExecutingSkill || skillStatsConfig == null) // is executing or item is null
            return;

        isExecutingSkill = true;
        SetInteractButton(false);
        Skill skill = ObjectPooling.Instance.SpawnG0InPool(skillStatsConfig.prefab.gameObject, Vector3.one, PoolType.Skill).GetComponent<Skill>();
        skill.Init(this);
        skill.SetDamage(skillStatsConfig.damage);
        skill.Execute();
    }

    public void ResetExecutingSkill()
    {
        if (skillStatsConfig == null) // item is null
            return;

        if (imgIcon.sprite == skillStatsConfig.skillSpt || !imgIcon.gameObject.activeInHierarchy)
        {
            SetInteractButton(true);
        }
        else
        {
            SetInteractButton(false);
        }
        isExecutingSkill = false;
        ResetChildComponents();
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

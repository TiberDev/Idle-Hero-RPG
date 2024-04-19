using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTableItem : MonoBehaviour
{
    [SerializeField] private GameObject goLock;
    [SerializeField] private Image imgItemIcon, imgCircleIcon;
    [SerializeField] private Image amountCoolDown, amountCoolDownExisting;
    [SerializeField] private Button btn;
    [SerializeField] private TMP_Text txtCounter;
    [SerializeField] private Color colorWorkingCircle, colorIdlingCircle;
    [SerializeField] private Sprite emptySpt;

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
        txtCounter.text = ((int)time).ToString() + "s";
    }

    public void SetTextCounter()
    {
        txtCounter.text = "";
    }

    public void SetAmountCoolDown()
    {
        imgCircleIcon.color = colorIdlingCircle;
        amountCoolDownExisting.fillAmount = 0;
        SetInteractButton(false);
        StartCoroutine(IECoolDown());
    }

    public void SetAmountExistingCoolDown(float totalTime)
    {
        imgCircleIcon.color = colorWorkingCircle;
        amountCoolDown.fillAmount = 0;
        StartCoroutine(IEExistingCounter(totalTime));
    }

    private IEnumerator IEExistingCounter(float totalTime)
    {
        float curTime = totalTime;
        while (curTime > 0)
        {
            curTime -= Time.deltaTime;
            amountCoolDownExisting.fillAmount = 1 - curTime / totalTime;
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

    public void SetImageIcon(Sprite sptIcon)
    {
        imgItemIcon.color = Color.white;
        imgItemIcon.sprite = sptIcon;
    }

    public void SetImageIcon(bool unlock)
    {
        imgItemIcon.color = unlock ? Color.white : Color.black;
        goLock.SetActive(!unlock);
    }

    public void SetImageIcon()
    {
        imgItemIcon.color = Color.white;
        imgItemIcon.sprite = emptySpt;
    }

    private void SetInteractButton(bool interactable)
    {
        btn.interactable = interactable;
    }

    public void ExecuteSkill()
    {
        if (!btn.interactable || skillStats == null) // skill is executing or empty or locked
            return;

        SetInteractButton(false);
        skill = ObjectPooling.Instance.SpawnG0InPool(skillStatsConfig.prefab.gameObject, Vector3.one, PoolType.Skill).GetComponent<Skill>();
        skill.Init(this);
        skill.SetValue(skillStatsConfig.damage);
        skill.Execute();
    }

    public void ResetExecutingSkill()
    {
        if (skill != null)
            skill.EndExistence();

        if (!goLock.activeInHierarchy)
        {
            SetInteractButton(true); // unlock
        }
        else
        {
            SetInteractButton(false); // lock
        }
        StopAllCoroutines();
        amountCoolDown.fillAmount = 0;
        amountCoolDownExisting.fillAmount = 0;
        imgCircleIcon.color = colorIdlingCircle;
        txtCounter.text = "";
    }

    public void PressSkillBtn()
    {
        if (skillStats != null)
        {
            ExecuteSkill();
        }
        else
        {
            BottomTab.Instance.OpenTab(2);
        }
    }
}

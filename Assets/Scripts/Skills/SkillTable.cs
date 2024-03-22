using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTable : MonoBehaviour
{
    [SerializeField] private SkillTableItem[] skillTableItemList;
    [SerializeField] private RectTransform rectTfmStar;
    [SerializeField] private SkillStatsManager skillStatsManager;
    [SerializeField] private TMP_Text txtAuto;
    [SerializeField] private Color colorAuto, colorNotAuto, colorTxtAuto, colorTxtNotAuto;
    [SerializeField] private Image imgAutoBtn;

    private Coroutine corouRotateStar;
    private bool automatic;

    public bool Automatic { get => automatic; }

    public void GetAutomaticData()
    {
        // 0 : false
        // 1 : true
        automatic = PlayerPrefs.GetInt("AUTOMATIC", 0) == 0 ? false : true;
        imgAutoBtn.color = automatic ? colorAuto : colorNotAuto;
        txtAuto.color = automatic ? colorTxtAuto : colorTxtNotAuto;
        if (automatic)
            corouRotateStar = StartCoroutine(IERotateStarIcon());
    }

    public void UnlockSkillTblItem(int index, bool unlock)
    {
        skillTableItemList[index].SetImageIcon(unlock);
        skillTableItemList[index].ResetExecutingSkill();
        skillTableItemList[index].SetSkillTable(this);
    }

    public void SetSkillTableItem(int index, SkillStats skillStats, SObjSkillStatsConfig config, bool cooldown)
    {
        skillTableItemList[index].Init(skillStats, config);
        skillTableItemList[index].SetImageIcon(config.skillSpt);
        skillTableItemList[index].ResetExecutingSkill();
        if (cooldown)
            skillTableItemList[index].SetAmountCoolDown();
    }

    public void SetSkillTblItemEmpty(int index)
    {
        skillTableItemList[index].Init(null, null);
        skillTableItemList[index].SetImageIcon();
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

    public void OnClickAutoBtn()
    {
        automatic = !automatic;
        imgAutoBtn.color = automatic ? colorAuto : colorNotAuto;
        txtAuto.color = automatic ? colorTxtAuto : colorTxtNotAuto;
        PlayerPrefs.SetInt("AUTOMATIC", automatic ? 1 : 0);
        if (automatic)
        {
            corouRotateStar = StartCoroutine(IERotateStarIcon());

            if (!BoxScreenCollision.Instance.IsEnenmiesEmpty())
                HandleAutomatic();
        }
        else
        {
            rectTfmStar.localEulerAngles = Vector3.zero;
            StopCoroutine(corouRotateStar);
            corouRotateStar = null;
        }
    }

    private IEnumerator IERotateStarIcon()
    {
        float curTime = 0;
        float totalTime = 1.5f;
        while (true)
        {
            curTime = Mathf.Min(totalTime, curTime + Time.deltaTime);
            float rotationZ = curTime * 360 / totalTime;
            Vector3 localRotation = rectTfmStar.localEulerAngles;
            localRotation.z = rotationZ;
            rectTfmStar.localEulerAngles = localRotation;
            yield return null;

            if (curTime == totalTime)
                curTime = 0;
        }
    }
}

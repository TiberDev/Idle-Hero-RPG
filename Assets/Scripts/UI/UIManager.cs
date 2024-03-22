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
    [SerializeField] private TMP_Text txtGold, txtGem;

    private IEnumerator IEScalingRect(RectTransform rect, Vector2 startScale, Vector2 endScale, float scalingTime, LerpType lerpType)
    {
        rect.sizeDelta = startScale;
        float curTime = 0;
        if (!rect.Equals(endScale))
        {
            while (curTime < scalingTime)
            {
                curTime += Time.deltaTime;
                float factor = EasyType.MatchedLerpType(lerpType, curTime / scalingTime);
                rect.sizeDelta = Vector2.Lerp(startScale, endScale, factor);
                yield return null;
            }
        }
    }

    private IEnumerator IEMovingRect(RectTransform rect, Vector2 startPos, Vector2 endPos, float movingTime, LerpType lerpType)
    {
        rect.anchoredPosition = startPos;
        float curTime = 0;
        if (!rect.Equals(endPos))
        {
            while (curTime < movingTime)
            {
                curTime += Time.deltaTime;
                float factor = EasyType.MatchedLerpType(lerpType, curTime / movingTime);
                rect.anchoredPosition = Vector2.Lerp(startPos, endPos, factor);
                yield return null;
            }
        }
    }

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
        gearsStatsManager.gameObject.SetActive(false);
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

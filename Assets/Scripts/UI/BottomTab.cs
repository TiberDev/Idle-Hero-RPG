using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BottomTab : Singleton<BottomTab>
{
    [SerializeField] private RectTransform[] rectItemTabBtnList;
    [SerializeField] private Image[] imgItemTabBtnList;
    [SerializeField] private GameObject[] itemTabTxtList;
    [SerializeField] private RectTransform rectHeroStatsManager;
    [SerializeField] private RectTransform rectGearsStatsManager;
    [SerializeField] private RectTransform rectSkillStatsManager;
    [SerializeField] private Color colorOpeningBtn, colorClosingBtn;

    [SerializeField] private float sizingBtnTime;

    private RectTransform curRectItemTab;
    private GameObject curItemTabTxt;
    private Image curImgItemTab;

    private IEnumerator IEScalingRect(RectTransform rect, Vector2 startScale, Vector2 endScale, float scalingTime, LerpType lerpType)
    {
        rect.localScale = startScale;
        float curTime = 0;
        if (!rect.Equals(endScale))
        {
            while (curTime < scalingTime)
            {
                curTime += Time.deltaTime;
                float factor = EasyType.MatchedLerpType(lerpType, curTime / scalingTime);
                rect.localScale = Vector2.Lerp(startScale, endScale, factor);
                yield return null;
            }
        }
    }

    private IEnumerator IESizinggRect(RectTransform rect, Vector2 startSize, Vector2 endSize, float sizingTime, LerpType lerpType)
    {
        rect.sizeDelta = startSize;
        float curTime = 0;
        if (!rect.Equals(endSize))
        {
            while (curTime < sizingTime)
            {
                curTime += Time.deltaTime;
                float factor = EasyType.MatchedLerpType(lerpType, curTime / sizingTime);
                rect.sizeDelta = Vector2.Lerp(startSize, endSize, factor);
                yield return null;
            }
        }
    }

    private void OpenButtonTab(RectTransform rectTfm, Image img, GameObject goText)
    {
        img.color = colorOpeningBtn;
        curRectItemTab = rectTfm;
        curItemTabTxt = goText;
        curImgItemTab = img;
        curItemTabTxt.SetActive(true);
        // transform button
        Vector2 endsize = new Vector2(rectTfm.sizeDelta.x, 350);
        StartCoroutine(IESizinggRect(rectTfm, rectTfm.sizeDelta, endsize, sizingBtnTime, LerpType.EaseInQuad));
        //StartCoroutine(ies(rectTfm, rectTfm.sizeDelta, endsize, sizingBtnTime, LerpType.EaseInQuad));
    }

    private void CloseButtonTab(RectTransform rect, Image img, GameObject goTxt, float height)
    {
        img.color = colorClosingBtn;
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, height);
        goTxt.SetActive(false);
        curRectItemTab = null;
        curItemTabTxt = null;
        curImgItemTab = null;
    }

    public void OnClickHeroStatsBtn()
    {
        if (rectHeroStatsManager.gameObject.activeInHierarchy)
        {
            CloseButtonTab(rectItemTabBtnList[0], imgItemTabBtnList[0], itemTabTxtList[0], 230);
        }
        else
        {
            if (curRectItemTab != null)
                CloseButtonTab(curRectItemTab, curImgItemTab, curItemTabTxt, 230);
            OpenButtonTab(rectItemTabBtnList[0], imgItemTabBtnList[0], itemTabTxtList[0]);
        }
    }

    public void OnClickGearStatsBtn()
    {
        if (rectGearsStatsManager.gameObject.activeInHierarchy)
        {
            CloseButtonTab(rectItemTabBtnList[1], imgItemTabBtnList[1], itemTabTxtList[1], 230);
        }
        else
        {
            if (curRectItemTab != null)
                CloseButtonTab(curRectItemTab, curImgItemTab, curItemTabTxt, 230);
            OpenButtonTab(rectItemTabBtnList[1], imgItemTabBtnList[1], itemTabTxtList[1]);
        }
    }

    public void OnClickSkillStatsBtn()
    {
        if (rectSkillStatsManager.gameObject.activeInHierarchy)
        {
            CloseButtonTab(rectItemTabBtnList[2], imgItemTabBtnList[2], itemTabTxtList[2], 230);
        }
        else
        {
            if (curRectItemTab != null)
                CloseButtonTab(curRectItemTab, curImgItemTab, curItemTabTxt, 230);
            OpenButtonTab(rectItemTabBtnList[2], imgItemTabBtnList[2], itemTabTxtList[2]);
        }
    }

}
using UnityEngine;
using UnityEngine.UI;

public enum BottomTabType
{
    Hero = 0,
    Gear = 1,
    Skill = 2,
}

public class BottomTab : Singleton<BottomTab>
{
    [SerializeField] private RectTransform[] rectItemTabBtnList;
    [SerializeField] private Image[] imgItemTabBtnList;
    [SerializeField] private GameObject[] itemTabTxtList;
    [SerializeField] private RectTransform rectHeroStatsManager, rectGearsStatsManager, rectSkillStatsManager;
    [SerializeField] private GameObject heroStatsManager, gearsStatsManager, skillStatsManager;
    [SerializeField] private Color colorOpeningBtn, colorClosingBtn;

    [SerializeField] private float sizingBtnTime, movingTime;

    private RectTransform curRectItemTab;
    private GameObject curItemTabTxt;
    private Image curImgItemTab;
    private Coroutine corouPanel;

    private void OpenButtonTab(RectTransform rectTfm, Image img, GameObject goText)
    {
        img.color = colorOpeningBtn;
        curRectItemTab = rectTfm;
        curItemTabTxt = goText;
        curImgItemTab = img;
        curItemTabTxt.SetActive(true);
        // transform button
        Vector2 endsize = rectTfm.sizeDelta;
        endsize.y = 350;
        StartCoroutine(UITransformController.Instance.IESizinggRect(rectTfm, rectTfm.sizeDelta, endsize, sizingBtnTime, LerpType.EaseInQuad));
    }

    private void CloseButtonTab(RectTransform rect, Image img, GameObject goTxt, float height)
    {
        img.color = colorClosingBtn;
        Vector2 sizeDelta = rect.sizeDelta;
        sizeDelta.y = height;
        rect.sizeDelta = sizeDelta;
        goTxt.SetActive(false);
        curRectItemTab = null;
        curItemTabTxt = null;
        curImgItemTab = null;
    }

    private void OpenPanel(RectTransform rectTfm, Vector2 startPos, Vector2 endPos, float time)
    {
        if (corouPanel != null)
        {
            StopCoroutine(corouPanel);
            corouPanel = null;
        }
        // moving effect
        corouPanel = StartCoroutine(UITransformController.Instance.IEMovingRect(rectTfm, startPos, endPos, time, LerpType.EaseOutBack));
    }

    public void OnClickTabBtn(int bottomTabTypeInt)
    {
        // handle bottom tab buttons
        if (curRectItemTab == rectItemTabBtnList[bottomTabTypeInt])
        {
            // close tab button
            CloseButtonTab(rectItemTabBtnList[bottomTabTypeInt], imgItemTabBtnList[bottomTabTypeInt], itemTabTxtList[bottomTabTypeInt], 230);
        }
        else
        {
            if (curRectItemTab != null)
                CloseButtonTab(curRectItemTab, curImgItemTab, curItemTabTxt, 230);
            OpenButtonTab(rectItemTabBtnList[bottomTabTypeInt], imgItemTabBtnList[bottomTabTypeInt], itemTabTxtList[bottomTabTypeInt]);
        }

        // handle panels
        BottomTabType type = (BottomTabType)bottomTabTypeInt;
        heroStatsManager.SetActive(type == BottomTabType.Hero && !heroStatsManager.activeInHierarchy);
        gearsStatsManager.SetActive(type == BottomTabType.Gear && !gearsStatsManager.activeInHierarchy);
        skillStatsManager.SetActive(type == BottomTabType.Skill && !skillStatsManager.activeInHierarchy);

        RectTransform rectTfm = null;
        Vector2 startPos = Vector2.zero;
        Vector2 endPos;

        switch (type)
        {
            case BottomTabType.Hero:
                rectTfm = rectHeroStatsManager;
                startPos = rectHeroStatsManager.anchoredPosition;
                startPos.y = -840;
                break;
            case BottomTabType.Gear:
                rectTfm = rectGearsStatsManager;
                startPos = rectGearsStatsManager.anchoredPosition;
                startPos.y = -752;
                break;
            case BottomTabType.Skill:
                rectTfm = rectSkillStatsManager;
                startPos = rectGearsStatsManager.anchoredPosition;
                startPos.y = -703;
                break;
        }
        endPos = startPos;
        endPos.y = startPos.y * -1;
        OpenPanel(rectTfm, startPos, endPos, movingTime);
    }
}
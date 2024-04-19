using UnityEngine;
using UnityEngine.UI;

public enum BottomTabType
{
    Hero = 0,
    Gear = 1,
    Skill = 2,
    Shop = 3,
}

public class BottomTab : Singleton<BottomTab>
{
    [SerializeField] private RectTransform[] rectItemTabBtnList;
    [SerializeField] private Image[] imgItemTabBtnList;
    [SerializeField] private GameObject[] itemTabTxtList;
    [SerializeField] private GameObject[] gObjPanels;
    [SerializeField] private Color colorOpeningBtn, colorClosingBtn;

    [SerializeField] private float sizingBtnTime;

    private IBottomTabHandler[] bottomTabHandlers;
    private int curTypeInt = -1;

    private void Start()
    {
        bottomTabHandlers = new IBottomTabHandler[gObjPanels.Length];
        for (int i = 0; i < gObjPanels.Length; i++)
        {
            bottomTabHandlers[i] = gObjPanels[i].GetComponent<IBottomTabHandler>();
        }
    }

    private void OpenButtonTab(RectTransform rectTfm, Image img, GameObject goText)
    {
        img.color = colorOpeningBtn;
        goText.SetActive(true);
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
    }

    public void OnClickTabBtn(int bottomTabTypeInt)
    {
        if (curTypeInt == bottomTabTypeInt)
        {
            // close tab button
            CloseButtonTab(rectItemTabBtnList[bottomTabTypeInt], imgItemTabBtnList[bottomTabTypeInt], itemTabTxtList[bottomTabTypeInt], 230);
            bottomTabHandlers[bottomTabTypeInt].SetPanelActive(false);
            curTypeInt = -1;
        }
        else
        {
            SoundManager.Instance.PlayClickSound();
            OpenTab(bottomTabTypeInt);
        }
    }

    public void OpenTab(int bottomTabTypeInt)
    {
        if (curTypeInt > -1)
        {
            CloseButtonTab(rectItemTabBtnList[curTypeInt], imgItemTabBtnList[curTypeInt], itemTabTxtList[curTypeInt], 230);
            bottomTabHandlers[curTypeInt].SetPanelActive(false);
        }
        curTypeInt = bottomTabTypeInt;
        OpenButtonTab(rectItemTabBtnList[bottomTabTypeInt], imgItemTabBtnList[bottomTabTypeInt], itemTabTxtList[bottomTabTypeInt]);
        bottomTabHandlers[bottomTabTypeInt].SetPanelActive(true);
    }
}
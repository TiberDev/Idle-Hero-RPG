using UnityEngine;

public class ShrinkButton : MonoBehaviour
{
    [SerializeField] private RectTransform rectTfm, rectTfmGeneralPanel, rectTfmSkillTblPanel, rectTfmIcon;
    [SerializeField] private Vector2 topSkillTblPos, bottomSkillTblPos, topBtnPos, bottomBtnPos;
    [SerializeField] private Vector2 topGeneralPanelSize, bottomGeneralPanelSize;
    [SerializeField] private float movingTime;

    private bool shrinking;
    private readonly string DATAKEY = "SHRINKDATA";

    public void ShrinkPanel(bool gameInit)
    {
        if (gameInit)
            shrinking = PlayerPrefs.GetInt(DATAKEY, 0) == 1 ? true : false;

        Vector2 curSTblPos = rectTfmSkillTblPanel.anchoredPosition;
        Vector2 curBtnPos = rectTfm.anchoredPosition;
        Vector2 curSize = rectTfmGeneralPanel.sizeDelta;

        if (shrinking) // shrink
        {
            rectTfmIcon.eulerAngles = Vector3.forward * 180;
            SetBtnTblAnchorPosition(curBtnPos, bottomBtnPos, !gameInit);
            SetSkillTblAnchorPosition(curSTblPos, bottomSkillTblPos, !gameInit);
            SetGeneralPanelSizeDelta(curSize, bottomGeneralPanelSize, !gameInit);
            BoxScreenCollision.Instance.SetBox(true, gameInit);
        }
        else // expand
        {
            rectTfmIcon.eulerAngles = Vector3.zero;
            SetBtnTblAnchorPosition(curBtnPos, topBtnPos, !gameInit);
            SetSkillTblAnchorPosition(curSTblPos, topSkillTblPos, !gameInit);
            SetGeneralPanelSizeDelta(curSize, topGeneralPanelSize, !gameInit);
            BoxScreenCollision.Instance.SetBox(false, gameInit);
        }
    }

    public Vector2 GetSkillTblePos(bool bottom)
    {
        rectTfmSkillTblPanel.anchoredPosition = bottom ? bottomSkillTblPos : topSkillTblPos;
        return rectTfmSkillTblPanel.position;
    }

    public void OnClickShinkButton()
    {
        SoundManager.Instance.PlayClickSound();
        StopAllCoroutines();
        shrinking = !shrinking;
        PlayerPrefs.SetInt(DATAKEY, shrinking ? 1 : 0);
        ShrinkPanel(false);
    }

    private void SetBtnTblAnchorPosition(Vector2 fromPos, Vector2 toPos, bool effect)
    {
        if (effect)
        {
            StartCoroutine(UITransformController.Instance.IEMovingRect(rectTfm, fromPos, toPos, movingTime, LerpType.Liner));
        }
        else
            rectTfm.anchoredPosition = toPos;
    }

    private void SetSkillTblAnchorPosition(Vector2 fromPos, Vector2 toPos, bool effect)
    {
        if (effect)
        {
            StartCoroutine(UITransformController.Instance.IEMovingRect(rectTfmSkillTblPanel, fromPos, toPos, movingTime, LerpType.Liner));
        }
        else
            rectTfmSkillTblPanel.anchoredPosition = toPos;
    }

    private void SetGeneralPanelSizeDelta(Vector2 startSize, Vector2 endSize, bool effect)
    {
        if (effect)
            StartCoroutine(UITransformController.Instance.IESizinggRect(rectTfmGeneralPanel, startSize, endSize, movingTime, LerpType.Liner));
        else
            rectTfmGeneralPanel.sizeDelta = endSize;
    }

}

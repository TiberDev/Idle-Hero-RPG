using UnityEngine;

public class BossKillWaitingEffect : MonoBehaviour
{
    [SerializeField] private MapManager mapManager;
    [SerializeField] private RectTransform rectTfmBtn, rectTfmText;
    [SerializeField] private GameObject gObjTfmText;

    [SerializeField] private Vector2 sizeIcon;
    [SerializeField] private Vector2 downPos, upPos;
    [SerializeField] private float iconScalingTime;
    [SerializeField] private float textUpDownTime;

    private void CreateUpDownEffect(Vector2 toPos)
    {
        StartCoroutine(UITransformController.Instance.IEMovingRect(rectTfmText, rectTfmText.anchoredPosition, toPos, textUpDownTime, LerpType.Liner, () =>
        {
            toPos = toPos == downPos ? upPos : downPos;
            CreateUpDownEffect(toPos);
        }));
    }

    public void Init()
    {
        gameObject.SetActive(true);
        // create icon appearing effect
        StartCoroutine(UITransformController.Instance.IESizinggRect(rectTfmBtn, Vector2.zero, sizeIcon, iconScalingTime, LerpType.EaseInOutBack, () =>
        {
            gObjTfmText.SetActive(true);
            CreateUpDownEffect(upPos);
        }));
    }

    public void OnClickBossIconBtn()
    {
        mapManager.LoadNextWave(true);
        gObjTfmText.SetActive(false);
        gameObject.SetActive(false);
    }
}

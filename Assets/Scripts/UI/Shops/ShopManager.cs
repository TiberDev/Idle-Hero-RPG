using UnityEngine;

public class ShopManager : MonoBehaviour, IBottomTabHandler
{
    [SerializeField] private GameObject gObj;
    [SerializeField] private RectTransform rectTfm;

    [SerializeField] private float scalingUpTime;

    public void SetPanelActive(bool active)
    {
        // effect
        if (active)
        {
            gObj.SetActive(true);
            TransformUIPanel();
        }
        else
        {
            StopAllCoroutines();
            gObj.SetActive(false);
        }
    }

    public void TransformUIPanel()
    {
        gObj.SetActive(true);
        StartCoroutine(UITransformController.Instance.IEScalingRect(rectTfm, Vector2.one * 0.5f, Vector2.one, scalingUpTime, LerpType.EaseOutBack));
    }
}

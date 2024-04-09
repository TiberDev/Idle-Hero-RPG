using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldGStatsButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GeneralItem generalItem;
    [SerializeField] private Image imgBtn;
    [SerializeField] private RectTransform rectTfm;
    [SerializeField] private GameObject maxLvGO;


    [SerializeField] private float increaseDuration, holdDuration;
    [SerializeField] private float scalingTime;

    private UITransformController uiTransformController;
    private Coroutine corouBtn;

    private float holdTime, increaseTime;
    private bool isHolding, interactive;

    private void Start()
    {
        uiTransformController = UITransformController.Instance;
        increaseTime = increaseDuration;
    }

    private void Update()
    {
        if (isHolding)
        {
            holdTime += Time.deltaTime;
            if (holdTime >= holdDuration)
            {
                increaseTime = Mathf.Min(increaseDuration, increaseTime + Time.deltaTime);
                float half = increaseDuration / 2;
                if (corouBtn == null)
                {
                    float start = 0.9f, end = 1f;
                    float temp = end - start;
                    float tempTime = increaseTime - half;
                    float scaling, t;
                    if (increaseTime <= half)
                    {
                        // scale down
                        t = temp - tempTime * temp / half;
                    }
                    else
                    {
                        // scale up
                        t = tempTime * temp / half;
                    }
                    scaling = t + start;
                    rectTfm.localScale = Vector3.one * scaling;
                }

                if (increaseTime >= increaseDuration)
                {
                    // increase gold faster
                    generalItem.EnhanceItem();
                    increaseTime = 0;
                    if (corouBtn != null)
                    {
                        StopCoroutine(corouBtn);
                        corouBtn = null;
                    }
                }
            }
        }
    }

    /// <summary>
    /// User press the button
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!interactive)
            return;

        if (corouBtn != null)
        {
            StopCoroutine(corouBtn);
            corouBtn = null;
        }

        corouBtn = StartCoroutine(uiTransformController.IEScalingRect(rectTfm, rectTfm.localScale, Vector2.one * 0.9f, scalingTime, LerpType.Liner));
        isHolding = true;
        holdTime = 0;
        increaseTime = increaseDuration;
    }

    /// <summary>
    /// User release the button
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!interactive)
            return;

        if (corouBtn != null)
        {
            StopCoroutine(corouBtn);
            corouBtn = null;
            corouBtn = StartCoroutine(uiTransformController.IEScalingRect(rectTfm, rectTfm.localScale, Vector2.one, scalingTime, LerpType.Liner));
        }
        isHolding = false;
        if (holdTime < holdDuration) // increase gold by 1 time if user doesn't hold button for long
        {
            generalItem.EnhanceItem();
        }
    }

    public void SetInteractive(bool isActive)
    {
        interactive = isActive;
        imgBtn.color = !isActive ? Color.grey : Color.green;
        if (isHolding && !isActive)
        {
            isHolding = false;
            rectTfm.localScale = Vector2.one;
            corouBtn = null;
        }
    }

    public void SetMaxLv()
    {
        maxLvGO.SetActive(true);
        isHolding = false;
    }
}

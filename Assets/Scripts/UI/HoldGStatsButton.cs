using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldGStatsButton : MonoBehaviour
{
    //[SerializeField] private GeneralItem generalItem;
    [SerializeField] private Image imgBtn;
    [SerializeField] private RectTransform rectTfm;
    [SerializeField] private GameObject maxLvGO;

    [SerializeField] private float increaseDuration, holdDuration;
    [SerializeField] private float scalingTime;

    private Coroutine corouBtn;
    private UnityAction enhanceAction;

    private float holdTime, increaseTime;
    private bool isHolding, interactive;

    public UnityAction EnhanceAction { set => enhanceAction = value; }

    private void Start()
    {
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
                    enhanceAction?.Invoke();
                    SoundManager.Instance.PlayEnhanceClickSound();
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
    public void OnPointerDown(BaseEventData eventData)
    {
        if (!interactive)
            return;

        if (corouBtn != null)
        {
            StopCoroutine(corouBtn);
            corouBtn = null;
        }

        corouBtn = StartCoroutine(UITransformController.Instance.IEScalingRect(rectTfm, rectTfm.localScale, Vector2.one * 0.9f, scalingTime, LerpType.Liner));
        isHolding = true;
        holdTime = 0;
        increaseTime = increaseDuration;
    }

    /// <summary>
    /// User release the button
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(BaseEventData eventData)
    {
        if (!interactive)
            return;

        if (corouBtn != null)
        {
            StopCoroutine(corouBtn);
            corouBtn = null;
            corouBtn = StartCoroutine(UITransformController.Instance.IEScalingRect(rectTfm, rectTfm.localScale, Vector2.one, scalingTime, LerpType.Liner));
        }
        isHolding = false;
        if (holdTime < holdDuration && (eventData as PointerEventData).pointerEnter == gameObject) // increase gold by 1 time if user doesn't hold button for long
        {
            enhanceAction?.Invoke();
            SoundManager.Instance.PlayEnhanceClickSound();
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

    public void SetMaxLv(bool max)
    {
        if (max)
        {
            maxLvGO.SetActive(true);
            isHolding = false;
        }
        else
            maxLvGO.SetActive(false);
    }
}

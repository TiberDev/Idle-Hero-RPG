using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldGStatsButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GeneralItem generalItem;
    [SerializeField] private Image imgBtn;
    [SerializeField] private GameObject maxLvGO;

    [SerializeField] private float holdDuration;
    [SerializeField] private float increaseDuration;

    private float holdTime, increaseTime;
    private bool isHolding, interactive;

    private void Start()
    {
        generalItem = GetComponentInParent<GeneralItem>();
    }

    private void Update()
    {
        if (isHolding)
        {
            holdTime += Time.deltaTime;
            if (holdTime >= holdDuration)
            {
                increaseTime += Time.deltaTime;
                if (increaseTime >= increaseDuration)
                {
                    // increase gold faster
                    generalItem.EnhanceItem();
                    increaseTime = 0;
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

        isHolding = true;
        holdTime = 0;
        increaseTime = 0;
    }

    /// <summary>
    /// User release the button
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!interactive)
            return;

        isHolding = false;
        if (holdTime < holdDuration) // increase gold by 1 time if user doesn't hold button for long
        {
            generalItem.EnhanceItem();
        }
    }

    public void SetInteractive(bool isActive)
    {
        interactive = isActive;
        imgBtn.color = !isActive ? Color.red : Color.green;
        if (isHolding && !isActive)
            isHolding = false;
    }

    public void SetMaxLv()
    {
        maxLvGO.SetActive(true);
        isHolding = false;
    }
}

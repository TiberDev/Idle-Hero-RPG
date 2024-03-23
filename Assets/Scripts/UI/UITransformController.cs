using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class UITransformController : Singleton<UITransformController>
{
    public IEnumerator IEScalingRect(RectTransform rect, Vector2 startScale, Vector2 endScale, float scalingTime, LerpType lerpType, UnityAction callback = null)
    {
        rect.localScale = startScale;
        float curTime = 0;
        if (!rect.Equals(endScale))
        {
            while (curTime < scalingTime)
            {
                curTime = Mathf.Min(scalingTime, curTime + Time.deltaTime);
                float factor = EasyType.MatchedLerpType(lerpType, curTime / scalingTime);
                rect.localScale = Vector2.LerpUnclamped(startScale, endScale, factor);
                yield return null;
            }
        }
        callback?.Invoke();
    }

    public IEnumerator IESizinggRect(RectTransform rect, Vector2 startSize, Vector2 endSize, float sizingTime, LerpType lerpType, UnityAction callback = null)
    {
        rect.sizeDelta = startSize;
        float curTime = 0;
        if (!rect.Equals(endSize))
        {
            while (curTime < sizingTime)
            {
                curTime = Mathf.Min(sizingTime, curTime + Time.deltaTime);
                //curTime += Time.deltaTime;
                float factor = EasyType.MatchedLerpType(lerpType, curTime / sizingTime);
                rect.sizeDelta = Vector2.LerpUnclamped(startSize, endSize, factor);
                yield return null;
            }
        }
        callback?.Invoke();
    }

    public IEnumerator IEMovingRect(RectTransform rect, Vector2 startPos, Vector2 endPos, float movingTime, LerpType lerpType, UnityAction callback = null)
    {
        rect.anchoredPosition = startPos;
        float curTime = 0;
        if (!rect.Equals(endPos))
        {
            while (curTime < movingTime)
            {
                curTime = Mathf.Min(movingTime, curTime + Time.deltaTime);
                float factor = EasyType.MatchedLerpType(lerpType, curTime / movingTime);
                rect.anchoredPosition = Vector2.LerpUnclamped(startPos, endPos, factor);
                yield return null;
            }
        }
        callback?.Invoke();
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DarkBoardLoadingUI : MonoBehaviour
{
    [SerializeField] private Image imgBoard;
    [SerializeField] private float fadingTime;

    public static bool Fading { get; private set; }

    private void Start()
    {
        Fading = false;
    }

    public void StartFadeBoard(bool delay, UnityAction resetAction)
    {
        Fading = true;
        StopAllCoroutines();
        StartCoroutine(IEStartFade(delay, resetAction));
    }

    private IEnumerator IEStartFade(bool delay, UnityAction resetAction)
    {
        // delay
        if (delay)
            yield return new WaitForSeconds(1.5f);
        Color transparent = Vector4.zero;
        float curTime = 0;
        while (curTime < fadingTime)
        {
            curTime += Time.deltaTime;
            transparent.a = curTime / fadingTime;
            imgBoard.color = transparent;
            yield return null;
        }
        resetAction?.Invoke();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(IEEndFade());
    }

    private IEnumerator IEEndFade()
    {
        Color dark = Color.black;
        float curTime = 0;
        while (curTime < fadingTime)
        {
            curTime += Time.deltaTime;
            dark.a = 1 - curTime / fadingTime;
            imgBoard.color = dark;
            yield return null;
        }
        Fading = false;
    }

}

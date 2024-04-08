using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class CloudTrasitionLoading : MonoBehaviour
{
    [SerializeField] private GameObject gObj;
    [SerializeField] private Image imgCloudPanel;
    [SerializeField] private RectTransform rectTfmCloud;
    [SerializeField] private CameraController cameraController;

    [SerializeField] private Color colorCloudPanel;
    [SerializeField] private Vector2 startPos, endPos;
    [SerializeField] private float movingTime, fadingTime;

    public void MoveCloud(UnityAction callback)
    {
        imgCloudPanel.color = Vector4.zero;
        gObj.SetActive(true);
        StartCoroutine(IEMoveCloud(callback));
    }

    private IEnumerator IEMoveCloud(UnityAction callback)
    {
        // delay
        yield return new WaitForSeconds(2f);
        // moving camera effect
        cameraController.MoveDown(true);
        // fade panel
        StartCoroutine(IEStartFade());

        float time = 0;
        while (time < movingTime)
        {
            time += Time.deltaTime;
            rectTfmCloud.anchoredPosition = Vector2.Lerp(startPos, endPos, time / movingTime);
            yield return null;
        }
        cameraController.MoveDown(false);
        callback?.Invoke();
        StartCoroutine(IEEndFade());
    }

    private IEnumerator IEStartFade(/*bool delay, UnityAction resetAction*/)
    {
        // delay
        //if (delay)
        //    yield return new WaitForSeconds(1f);
        Color cover = colorCloudPanel;
        float curTime = 0;
        while (curTime < fadingTime)
        {
            curTime += Time.deltaTime;
            cover.a = curTime / fadingTime;
            imgCloudPanel.color = cover;
            yield return null;
        }
        //resetAction?.Invoke();
        yield return new WaitForSeconds(0.5f);
        //StartCoroutine(IEEndFade());
    }

    private IEnumerator IEEndFade()
    {
        Color transparent = colorCloudPanel;
        float curTime = 0;
        while (curTime < fadingTime)
        {
            curTime += Time.deltaTime;
            transparent.a = 1 - curTime / fadingTime;
            imgCloudPanel.color = transparent;
            yield return null;
        }
        gObj.SetActive(false);
    }
}

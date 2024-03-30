using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SceneLoadingUI : MonoBehaviour
{
    [SerializeField] private GameObject gObjLoading;
    [SerializeField] private RectTransform rectTfmTopLoading, rectTfmBottomLoading;

    public void StartLoading(UnityAction newMapAction)
    {
        StartCoroutine(IEStartLoading(newMapAction));
    }

    public void EndLoading()
    {
        // load scene
        StartCoroutine(IEEndLoading());
    }

    private IEnumerator IEStartLoading(UnityAction newMapAction)
    {
        // delay
        yield return new WaitForSeconds(2f);
        gObjLoading.SetActive(true);
        // Move top loading panel
        StartCoroutine(UITransformController.Instance.IEMovingRect(rectTfmTopLoading, Vector2.up * 1300, Vector2.zero, 0.3f, LerpType.EaseInOutQuad));
        // Move bottom loading panel
        StartCoroutine(UITransformController.Instance.IEMovingRect(rectTfmBottomLoading, Vector2.up * -1300, Vector2.zero, 0.3f, LerpType.EaseInOutQuad));
        yield return new WaitForSeconds(1.3f); // 0.3 second effect + 1 second delay
        newMapAction?.Invoke();
    }

    private IEnumerator IEEndLoading()
    {
        // Move top loading panel
        StartCoroutine(UITransformController.Instance.IEMovingRect(rectTfmTopLoading, Vector2.zero, Vector2.up * 1300, 0.3f, LerpType.EaseInOutQuad));
        // Move bottom loading panel
        StartCoroutine(UITransformController.Instance.IEMovingRect(rectTfmBottomLoading, Vector2.zero, Vector2.up * -1300, 0.3f, LerpType.EaseInOutQuad));
        yield return new WaitForSeconds(0.3f);
        gObjLoading.SetActive(false);
    }

}

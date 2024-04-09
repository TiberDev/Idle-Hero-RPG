using UnityEngine;
using UnityEngine.Events;

public class GoldEffectController : Singleton<GoldEffectController>
{
    [SerializeField] private GameObject goldPrefab;
    [SerializeField] private ObjectPooling objectPooling;
    [SerializeField] private UITransformController uiTransformController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform rectTfmParent;

    [SerializeField] private Vector2 destinationAnchorPos;
    [SerializeField] private float movingTime;

    public void CreateGoldIconEffect(Vector3 worldSpacePosition, UnityAction callback)
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(mainCamera, worldSpacePosition);
        RectTransform goldEffectRectTfm = objectPooling.SpawnG0InPool(goldPrefab, screenPos, PoolType.Effect).GetComponent<RectTransform>();
        goldEffectRectTfm.transform.SetParent(rectTfmParent);
        goldEffectRectTfm.localScale = Vector3.one;
        StartCoroutine(uiTransformController.IEMovingRect(goldEffectRectTfm, goldEffectRectTfm.anchoredPosition, destinationAnchorPos, movingTime, LerpType.EaseInBack
             , () =>
             {
                 objectPooling.RemoveGOInPool(goldEffectRectTfm.gameObject, PoolType.Effect, goldPrefab.name);
                 callback?.Invoke();
             }
             ));
    }
}

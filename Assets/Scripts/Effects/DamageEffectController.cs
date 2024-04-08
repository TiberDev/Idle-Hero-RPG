using TMPro;
using UnityEngine;

public class DamageEffectController : Singleton<DamageEffectController>
{
    [SerializeField] private GameObject dmgEffectPrefab;
    [SerializeField] private ObjectPooling objectPooling;
    [SerializeField] private UITransformController uiTransformController;
    [SerializeField] private Transform rectTfmParent;

    [SerializeField] private Vector2 destinationAnchorPos;
    [SerializeField] private float movingTime;

    public void CreateDmgEffect(Vector3 position, string damage, DamageTakenType damageTakenType)
    {
        DamageEffect dmgEffect = objectPooling.SpawnG0InPool(dmgEffectPrefab, position, PoolType.Effect).GetComponent<DamageEffect>();
        position += (Vector3.left + Vector3.up + Vector3.forward) * 3; ;
        dmgEffect.Init(position, Vector3.right * 40 + Vector3.up * 135, rectTfmParent, damage, damageTakenType);
    }
}

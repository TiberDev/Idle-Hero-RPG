using UnityEngine;

public class DamageEffectController : Singleton<DamageEffectController>
{
    [SerializeField] private CameraController mainCamera;
    [SerializeField] private GameObject dmgEffectPrefab;
    [SerializeField] private UITransformController uiTransformController;
    [SerializeField] private Transform rectTfmParent;

    [SerializeField] private Vector3 rotationOffset;
    [SerializeField] private float offset;

    public void CreateDmgEffect(Vector3 spwningPos, string damage, DamageTakenType damageTakenType)
    {
        Vector3 position = spwningPos;
        DamageEffect dmgEffect = ObjectPooling.Instance.SpawnG0InPool(dmgEffectPrefab, position, PoolType.Effect).GetComponent<DamageEffect>();
        dmgEffect.Init(position, rotationOffset, rectTfmParent, damage, damageTakenType);
    }
}

using System.Collections;
using UnityEngine;

public class CloneHeroEffectController : Singleton<CloneHeroEffectController>
{
    [SerializeField] private Transform tfmEPool, spawningEffectPrefab;
    [SerializeField] private GameObject removingEffectPrefab;

    private Transform tfmSpawningEffect;

    private void RemoveSpawningEffect()
    {
        tfmSpawningEffect.gameObject.SetActive(false);
    }

    private IEnumerator RemoveRemovingEffect(GameObject tfm)
    {
        yield return new WaitForSeconds(1f);
        ObjectPooling.Instance.RemoveGOInPool(tfm, PoolType.Effect);
    }

    public void CreateSpawningEffect(Vector3 position, float delay)
    {
        position.y += 1f;
        if (tfmSpawningEffect == null)
        {
            tfmSpawningEffect = Instantiate(spawningEffectPrefab, position, Quaternion.identity);
            tfmSpawningEffect.SetParent(tfmEPool);
        }
        else
        {
            tfmSpawningEffect.position = position;
            tfmSpawningEffect.gameObject.SetActive(true);
        }
        Invoke("RemoveSpawningEffect", delay);
    }

    public void CreateRemovingEffect(Vector3 position)
    {
        GameObject rvmEGObj = ObjectPooling.Instance.SpawnG0InPool(removingEffectPrefab, position, PoolType.Effect);
        StartCoroutine(RemoveRemovingEffect(rvmEGObj));
    }

}

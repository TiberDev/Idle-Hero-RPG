using System.Collections;
using TMPro;
using UnityEngine;

public enum DamageTakenType
{
    Normal,
    Critical,
    Skill
}

public class DamageEffect : MonoBehaviour
{
    [SerializeField] private TMP_Text txtDamage;
    [SerializeField] private Color colorNor, colorCrit, colorSkill;
    [SerializeField] private float movingTime;
    [SerializeField] private float fadingTime;

    private Transform cachedTfm;
    private Vector3 destination;

    private void Awake()
    {
        cachedTfm = transform;
    }

    private Color GetColor(DamageTakenType type)
    {
        switch (type)
        {
            case DamageTakenType.Normal:
                return colorNor;
            case DamageTakenType.Critical:
                return colorCrit;
            case DamageTakenType.Skill:
                return colorSkill;
            default: return colorNor;
        }
    }

    public void Init(Vector3 position, Vector3 rotation, Transform parent, string damage, DamageTakenType damageTakenType)
    {
        cachedTfm.position = position;
        destination = position + Vector3.up * 5;
        cachedTfm.eulerAngles = rotation;
        cachedTfm.SetParent(parent);
        txtDamage.text = damage;
        txtDamage.color = GetColor(damageTakenType);
        StartCoroutine(IEFadeText());
    }

    private void Update()
    {
        cachedTfm.position = Vector3.MoveTowards(cachedTfm.position, destination, movingTime * Time.deltaTime);
    }

    private IEnumerator IEFadeText()
    {
        Color transparent = txtDamage.color;
        float curTime = 0;
        while (curTime < fadingTime)
        {
            curTime += Time.deltaTime;
            transparent.a = 1 - curTime / fadingTime;
            txtDamage.color = transparent;
            yield return null;
        }
        ObjectPooling.Instance.RemoveGOInPool(gameObject, PoolType.Effect, name);
    }
}

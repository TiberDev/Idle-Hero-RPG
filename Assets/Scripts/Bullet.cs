using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    private Character target;
    private Transform cachedTfm, cachedTarget;

    private void Start()
    {
        cachedTfm = transform;
    }

    private void Update()
    {
        if (target == null)
            return;

        cachedTfm.position = Vector3.MoveTowards(cachedTfm.position, cachedTarget.position, moveSpeed * Time.deltaTime);
        if (cachedTfm.position == cachedTarget.position)
        {
            Destroy(gameObject);
        }
    }

    public void Init(Character target)
    {
        this.target = target;
        cachedTarget = target.transform;
    }
}

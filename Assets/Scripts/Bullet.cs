using System.Runtime.CompilerServices;
using UnityEngine;

public class Bullet : MonoBehaviour, ICharacterCollisionHandler
{
    [SerializeField] private float moveSpeed;

    private Character target, owner;
    private Transform cachedTfm, cachedTarget;
    private ObjectPooling objectPooling;

    private Vector3 destination;
    private bool enableMove;

    private void Awake()
    {
        cachedTfm = transform;
    }

    private void Start()
    {
        objectPooling = ObjectPooling.Instance;
        cachedTfm = transform;
    }

    private void Update()
    {
        if (!enableMove)                             
            return;

        if (target != null)
        {
            destination = cachedTarget.position;
            if (target.IsBoss)
                destination.y += 2;
        }
        cachedTfm.position = Vector3.MoveTowards(cachedTfm.position, destination, moveSpeed * Time.deltaTime);
        SetDirection(destination);
        if (cachedTfm.position == destination && target == null)
        {
            enableMove = false;
            objectPooling.RemoveGOInPool(gameObject, PoolType.Bullet, name);
        }
    }

    public void SetDirection(Vector3 targetPos)
    {
        Vector3 dir = targetPos - cachedTfm.position;
        float angleY = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        float angleX = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float angleZ = Mathf.Atan2(dir.y, dir.z) * Mathf.Rad2Deg;
        cachedTfm.eulerAngles = new Vector3(angleX, angleY, angleZ);
    }

    /// <summary>
    /// Method execute when bullet has not collide target yet while target died
    /// </summary>
    private void TargetDie()
    {
        target.DieAction -= TargetDie;
        destination = cachedTarget.position;
        if (target.IsBoss)
            destination.y += 2;
        RemoveTarget();
    }

    private void RemoveTarget()
    {
        target = null;
        cachedTarget = null;
    }

    public void Init(Character _target, Character _owner)
    {
        owner = _owner;
        target = _target;
        target.DieAction += TargetDie;
        cachedTarget = _target.GetTransform();
        enableMove = true;
    }

    public Transform GetTransform() => cachedTfm;

    public void HandleCollision(Character character)
    {
        if (target == null) // when target die, it notify action event so target will null
            return;

        enableMove = false;
        target.DieAction -= TargetDie;
        RemoveTarget();
        character.TakeDamage(owner.GetTotalDamage(character.IsBoss));
        objectPooling.RemoveGOInPool(gameObject, PoolType.Bullet, name);
    }

    public void HandleEndCollision(Character character) { }
}

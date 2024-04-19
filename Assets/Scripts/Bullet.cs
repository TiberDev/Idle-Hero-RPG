using UnityEngine;

public class Bullet : MonoBehaviour, ICharacterCollisionHandler
{
    [SerializeField] private float moveSpeed;

    private Character target, owner;
    private Transform cachedTfm;

    private Vector3 destination;
    private bool enableMove;

    private void Awake()
    {
        cachedTfm = transform;
    }

    private void Update()
    {
        if (!enableMove)
            return;

        if (target != null)
        {
            destination = target.GetHeadTransform().position;
        }
        cachedTfm.position = Vector3.MoveTowards(cachedTfm.position, destination, moveSpeed * Time.deltaTime);
        SetDirection(destination);
        if (cachedTfm.position == destination && (target == null || !target.isActiveAndEnabled))
        {
            enableMove = false;
            ObjectPooling.Instance.RemoveGOInPool(gameObject, PoolType.Bullet);
        }
    }

    public void SetDirection(Vector3 targetPos)
    {
        cachedTfm.LookAt(targetPos);
        Vector3 rotation = cachedTfm.eulerAngles;
        rotation.x -= 90;
        cachedTfm.eulerAngles = rotation;
    }

    /// <summary>
    /// Method execute when bullet has not collide target yet while target died
    /// </summary>
    private void TargetDie()
    {
        target.DieAction -= TargetDie;
        RemoveTarget();
    }

    private void RemoveTarget()
    {
        destination = target.GetHeadTransform().position;
        target = null;
    }

    public void Init(Character _target, Character _owner)
    {
        owner = _owner;
        target = _target;
        target.DieAction += TargetDie;
        enableMove = true;
    }

    public void Init(Vector3 toPos, Character _owner)
    {
        target = null;
        owner = _owner;
        destination = toPos;
        enableMove = true;
    }

    public Transform GetTransform() => cachedTfm;

    public void HandleCollision(Character character)
    {
        if (target == null || character != target) // when target die but not disable yet or collide with a character different from target,
            return;

        enableMove = false;
        target.DieAction -= TargetDie;
        RemoveTarget();
        character.TakeDamage(owner.GetTotalDamage(character.IsBoss), owner.Critical ? DamageTakenType.Critical : DamageTakenType.Normal);
        ObjectPooling.Instance.RemoveGOInPool(gameObject, PoolType.Bullet);
    }

    public void HandleEndCollision(Character character) { }
}

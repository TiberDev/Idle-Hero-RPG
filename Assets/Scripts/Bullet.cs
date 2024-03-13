using UnityEngine;

public class Bullet : MonoBehaviour, ICharacterCollisionHandler
{
    [SerializeField] private float moveSpeed;

    private Character target, owner;
    private Transform cachedTfm, cachedTarget;
    private ObjectPooling objectPooling;

    private void Start()
    {
        objectPooling = ObjectPooling.Instance;
        cachedTfm = transform;
    }

    private void Update()
    {
        if (target == null)
            return;

        cachedTfm.position = Vector3.MoveTowards(cachedTfm.position, cachedTarget.position, moveSpeed * Time.deltaTime);
    }

    public void Init(Character _target, Character _owner)
    {
        owner = _owner;
        target = _target;
        cachedTarget = _target.transform;
        tag = owner.tag;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Hero") && owner.GetCharacterType() != CharacterType.Hero ||
    //     other.CompareTag("Enemy") && owner.GetCharacterType() != CharacterType.Enemy)
    //    {

    //        other.GetComponent<Character>().TakeDamage(owner.GetDamage(), owner.TargetDie);
    //        Destroy(gameObject);
    //    }
    //}

    public void HandleCollision(Character character)
    {
        character.TakeDamage(owner.GetDamage(), owner.TargetDie);
        objectPooling.RemoveGOInPool(gameObject, PoolType.Bullet, name);
    }
    
    public void HandleEndCollision(Character character) { }
}

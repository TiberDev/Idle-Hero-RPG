using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    private Character target, owner;
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
    }

    public void Init(Character _target, Character _owner)
    {
        owner = _owner;
        target = _target;
        cachedTarget = _target.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hero") && owner.GetCharacterType() != CharacterType.Hero ||
         other.CompareTag("Enemy") && owner.GetCharacterType() != CharacterType.Enemy)
        {

            other.GetComponent<Character>().TakeDamage(owner.GetDamage(), owner.TargetDie);
            Destroy(gameObject);
        }
    }
}

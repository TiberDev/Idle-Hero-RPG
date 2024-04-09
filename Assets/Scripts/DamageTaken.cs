using UnityEngine;

public class DamageTaken : MonoBehaviour
{
    [SerializeField] private Character character;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hero") || other.CompareTag("Enemy"))
        {
            Character collider = other.GetComponentInParent<Character>();
            character.TakeDamage(collider.GetTotalDamage(character.IsBoss), DamageTakenType.Skill);
        }
    }
}

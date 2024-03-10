using UnityEngine;

public class CharacterAttackDetect : MonoBehaviour
{
    [SerializeField] private Character character;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hero") && character.GetCharacterType() != CharacterType.Hero ||
            other.CompareTag("Enemy") && character.GetCharacterType() != CharacterType.Enemy)
        {
            Character collider = other.GetComponentInParent<Character>();
            collider.TakeDamage(character.GetDamage(), character.TargetDie);
        }
    }
}

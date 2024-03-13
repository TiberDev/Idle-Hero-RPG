using UnityEngine;

public class CharacterAttackDetect : MonoBehaviour, ICharacterCollisionHandler
{
    [SerializeField] private Character owner;

    void ICharacterCollisionHandler.HandleCollision(Character character)
    {
        character.TakeDamage(owner.GetDamage(), owner.TargetDie);
    }

    void ICharacterCollisionHandler.HandleEndCollision(Character character) { }
}

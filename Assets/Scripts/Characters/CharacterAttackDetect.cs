using UnityEngine;

public class CharacterAttackDetect : MonoBehaviour, ICharacterCollisionHandler
{
    [SerializeField] private Character owner;

    void ICharacterCollisionHandler.HandleCollision(Character character)
    {
        character.TakeDamage(owner.GetTotalDamage(character.IsBoss), owner.Critical ? DamageTakenType.Critical : DamageTakenType.Normal);
    }

    void ICharacterCollisionHandler.HandleEndCollision(Character character) { }
}

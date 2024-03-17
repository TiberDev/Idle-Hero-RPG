using UnityEngine;

public class InRangeDetection : MonoBehaviour, ICharacterCollisionHandler
{
    [SerializeField] private Character owner;

    void ICharacterCollisionHandler.HandleCollision(Character character)
    {
        owner.SetTarget(character);
    }

    void ICharacterCollisionHandler.HandleEndCollision(Character character) { }
}

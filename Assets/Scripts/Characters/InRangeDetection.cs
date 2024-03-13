using UnityEngine;

public class InRangeDetection : MonoBehaviour, ICharacterCollisionHandler
{
    [SerializeField] private Character owner;
    [SerializeField] private bool nearRange;

    void ICharacterCollisionHandler.HandleCollision(Character character)
    {
        owner.SetTargetInRange(character, nearRange);
    }

    void ICharacterCollisionHandler.HandleEndCollision(Character character) { }
}

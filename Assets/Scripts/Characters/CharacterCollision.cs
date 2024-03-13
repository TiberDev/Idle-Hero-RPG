using UnityEngine;

public class CharacterCollision : MonoBehaviour
{
    public Character owner;

    private void OnTriggerEnter(Collider other)
    {
        ICharacterCollisionHandler handler = other.GetComponent<ICharacterCollisionHandler>();
        if (handler != null)
            handler.HandleCollision(owner);
    }

    private void OnTriggerExit(Collider other)
    {
        ICharacterCollisionHandler handler = other.GetComponent<ICharacterCollisionHandler>();
        if (handler != null)
            handler.HandleEndCollision(owner);
    }
}

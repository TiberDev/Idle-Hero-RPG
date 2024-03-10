using UnityEngine;

public class NearDetect : MonoBehaviour
{
    [SerializeField] private Character parent;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hero") && parent.GetCharacterType() != CharacterType.Hero ||
         other.CompareTag("Enemy") && parent.GetCharacterType() != CharacterType.Enemy)
        {
            Character character = other.GetComponent<Character>();
            parent.DetectCharacterInNearRange(character);
        }
    }
}

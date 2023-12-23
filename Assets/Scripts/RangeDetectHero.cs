using UnityEngine;

public class RangeDetectHero : MonoBehaviour
{
    [SerializeField] private Character thisEnemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hero"))
        {
            thisEnemy.DetectHero(other.GetComponent<Character>());
        }
    }
}

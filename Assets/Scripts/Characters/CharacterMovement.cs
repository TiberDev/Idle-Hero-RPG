using UnityEngine;
using UnityEngine.AI;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float speed = 3;

    public void Move(Transform target)
    {
        if (agent != null)
        {
            agent.enabled = true;
            agent.speed = speed;
            agent.SetDestination(target.position);
        }
    }

    public void StopMoving()
    {
        agent.enabled = false;
    }
}

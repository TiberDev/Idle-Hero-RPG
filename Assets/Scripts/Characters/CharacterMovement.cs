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
            agent.speed = speed;
            agent.acceleration = speed;
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
    }

    public void StopMoving()
    {
        if (!agent.isStopped)
        {
            agent.isStopped = true;
            agent.acceleration = 0;
            agent.speed = 0;
        }
    }
}

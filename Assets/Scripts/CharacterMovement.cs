using UnityEngine;
using UnityEngine.AI;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;

    private Transform tfmTarget;

    private void Update()
    {
        if (tfmTarget != null)
            Move(tfmTarget);
    }

    public void Move(Transform target)
    {
        tfmTarget = target;
        if (agent != null)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
    }

    public void StopMoving()
    {
        tfmTarget = null;
        agent.isStopped = true;
    }
}

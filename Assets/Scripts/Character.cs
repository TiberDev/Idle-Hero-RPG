using UnityEngine;
using UnityEngine.AI;

public enum CharacterType
{
    Hero = 0,
    Enemy = 1
}

[RequireComponent(typeof(NavMeshAgent),typeof(Animator))]
[RequireComponent(typeof(CharacterAnimator), typeof(CharacterMovement))]
public class Character : MonoBehaviour
{
    [SerializeField] protected CharacterAnimator characterAnimator;
    [SerializeField] protected CharacterMovement characterMovement;
    [SerializeField] protected CharacterType characterType;

    [SerializeField] protected float rangeAttack;

    protected Character target;
    private GameManager instanceGM;

    private void Start()
    {
        instanceGM = GameManager.Instance;
        Init();
    }

    protected virtual void Update()
    {
        if (target != null)
        {
            if (rangeAttack >= Vector3.Distance(target.transform.position, transform.position)) // in attack range
            {
                // do attack
                DoAttack();
                // stop move
                characterMovement.StopMoving();
            }
            else
            {
                characterAnimator.PlayMoveAnimation();
                characterMovement.Move(target.transform);
            }
        }
    }

    private void Init()
    {
        switch (characterType)
        {
            case CharacterType.Hero:
                target = FindEnemy();
                // do move animation
                characterAnimator.PlayMoveAnimation();
                characterMovement.Move(target.transform);
                break;
            case CharacterType.Enemy:
                // start idle animation
                characterAnimator.PlayIdleAnimation();
                break;
        }
    }

    protected virtual void DoAttack() { }

    public Character FindEnemy()
    {
        // Find enemy
        return instanceGM.GetEnemy();
    }

    public virtual void DetectHero(Character heroDetected)
    {
        // do move animation 
        target = heroDetected;
        characterMovement.Move(heroDetected.transform);
    }

    private void OnDrawGizmos()
    {
        if (transform.rotation.eulerAngles.y < -90 || transform.rotation.eulerAngles.y > 90)
        {
            Gizmos.DrawLine(transform.position, transform.position + rangeAttack * Vector3.back);
        }
        else
        {
            Gizmos.DrawLine(transform.position, transform.position + rangeAttack * Vector3.forward);
        }
        //float xAxis = Mathf.Tan(transform.rotation.eulerAngles.y / Mathf.Rad2Deg) * rangeAttack;
    }
}

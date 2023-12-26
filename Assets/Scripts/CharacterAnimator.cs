using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    int isDeadID, isMovingID, attackTypeID;

    public Animator Animator { get => animator; }

    private void Awake()
    {
        GetID();
    }

    private void GetID()
    {
        isDeadID = Animator.StringToHash("isDead");
        isMovingID = Animator.StringToHash("isMoving");
        attackTypeID = Animator.StringToHash("attackType");
    }

    public void PlayIdleAnimation()
    {
        //animator.SetBool(isDeadID, true);
    }

    public void PlayMoveAnimation()
    {
        animator.SetBool(isMovingID, true);
    }

    public void PlayAttackAnimation(int type)
    {
        animator.SetInteger(attackTypeID, type);
        animator.SetBool(isMovingID, false);
    }

    public void PlayDieAnimation()
    {
        animator.SetBool(isDeadID, true);
    }
}

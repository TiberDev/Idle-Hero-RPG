using UnityEngine;

public enum AttackAnimationType
{
    MeleeAttack, LongAttack
}

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private static readonly int longAttackType = Animator.StringToHash("longAttackType");
    private static readonly int meleeAttackType = Animator.StringToHash("meleeAttackType");
    private static readonly int isDead = Animator.StringToHash("isDead");
    private static readonly int isMoving = Animator.StringToHash("isMoving");
    private static readonly int isIdle = Animator.StringToHash("isIdle");
    private static readonly int isOver = Animator.StringToHash("isOver");
    private static readonly int triggerAttack = Animator.StringToHash("triggerAttack");
    private static readonly int AttackSpeedMultiplier = Animator.StringToHash("AttackSpeedMultiplier");

    public Animator Animator { get => animator; }

    //public void PlayIdleAnimation()
    //{
    //    animator.SetBool(isIdle, true);
    //    animator.SetBool(isMoving, false);
    //}

    public void PlayMoveAnimation()
    {
        animator.SetBool(isMoving, true);
        animator.SetBool(isIdle, false);
    }

    public void PlayAttackAnimation(int type, AttackAnimationType attackAnimationType)
    {
        switch (attackAnimationType)
        {
            case AttackAnimationType.MeleeAttack:
                animator.SetInteger(meleeAttackType, type);
                break;
            case AttackAnimationType.LongAttack:
                animator.SetInteger(longAttackType, type);
                break;
        }
        animator.SetBool(isIdle, false);
        animator.SetBool(isMoving, false);
        animator.SetTrigger(triggerAttack);
    }

    public void PlayAttackAnimation(float speed)
    {
        SetAttackSpeedAnimaton(speed);
        animator.SetInteger(longAttackType, 1);
        animator.SetBool(isIdle, false);
        animator.SetBool(isMoving, false);
        animator.SetTrigger(triggerAttack);
    }

    public void PlayDieAnimation()
    {
        animator.SetBool(isDead, true);
    }

    public void PlayVictoryAnimation()
    {
        animator.SetBool(isOver, true);
    }

    public void SetAttackSpeedAnimaton(float multiplier)
    {
        animator.SetFloat(AttackSpeedMultiplier, multiplier);
    }
}

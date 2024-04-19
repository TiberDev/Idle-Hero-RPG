using System.Collections;
using UnityEngine;

public class LongAttackCharacter : Character
{
    [SerializeField] private Transform tfmSwnBullet;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private int[] numberOfRepeatAttackType;

    private Coroutine coroutineAttack;
    private Character aimingTarget;

    public int indexType, indexRepeat;
    private bool prepareShoot;

    protected override void OnEnable()
    {
        base.OnEnable();
        aimingTarget = null;
        if (coroutineAttack != null)
        {
            StopCoroutine(coroutineAttack);
            coroutineAttack = null;
        }
    }

    protected override void DoAttack()
    {
        if (coroutineAttack == null)
        {
            coroutineAttack = StartCoroutine(IEPlayAttackAnimation(numberOfRepeatAttackType, AttackAnimationType.LongAttack));
        }
    }

    private IEnumerator IEPlayAttackAnimation(int[] numberOfAttackType, AttackAnimationType attackType)
    {
        if (numberOfAttackType.Length <= 0) // doesn't have any attack
            yield break;

        prepareShoot = true;
        attackDone = false;
        if (indexRepeat >= numberOfAttackType[indexType])
        {
            indexRepeat = 0;
            indexType++;
            if (indexType >= numberOfAttackType.Length) // reset 
                indexType = 0;
        }
        characterAnimator.PlayAttackAnimation(indexType + 1, attackType);
        yield return new WaitUntil(() => attackDone); // wait for the end of animation state to start next attack
        indexRepeat++;
        // stop current coroutine to start new coroutine
        coroutineAttack = null;
    }

    public override void CheckTargetDie()
    {
        base.CheckTargetDie();
        if (prepareShoot)
        {
            aimingTarget = preTarget;
        }
        else
        {
            aimingTarget = null;
        }
    }

    /// <summary>
    /// Method runs from an animation event
    /// </summary>
    public void ActiveButtlet()
    {
        prepareShoot = false;
        Bullet bullet = ObjectPooling.Instance.SpawnG0InPool(bulletPrefab.gameObject, tfmSwnBullet.position, PoolType.Bullet).GetComponent<Bullet>();
        bullet.GetTransform().SetParent(gameManager.GetBulletPoolTfm());
        if (aimingTarget != null)
        {
            bullet.Init(aimingTarget, this);
            aimingTarget = null;
        }
        else
        {
            if (target == null)
            {
                bullet.Init(preTarget.GetHeadTransform().position, this);
            }
            else
                bullet.Init(target, this);
        }
    }
}



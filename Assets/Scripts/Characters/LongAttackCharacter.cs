using System.Collections;
using UnityEngine;

public class LongAttackCharacter : Character
{
    [SerializeField] private Transform tfmSwnBullet;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private int[] numberOfRepeatAttackType;

    private Coroutine coroutineAttack;

    public int indexType, indexRepeat;

    protected override void DoAttack()
    {
        if (coroutineAttack == null)
            coroutineAttack = StartCoroutine(IEPlayAttackAnimation(numberOfRepeatAttackType,AttackAnimationType.LongAttack));
    }

    private IEnumerator IEPlayAttackAnimation(int[] numberOfAttackType, AttackAnimationType attackType)
    {
        if (numberOfAttackType.Length <= 0) // doesn't have any attack
            yield break;

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

    /// <summary>
    /// Method runs from an event animation
    /// </summary>
    public void ActiveButtlet()
    {
        Bullet bullet = Instantiate(bulletPrefab, tfmSwnBullet.position, Quaternion.identity);
        bullet.Init(target == null ? preTarget : target, this);
    }
}



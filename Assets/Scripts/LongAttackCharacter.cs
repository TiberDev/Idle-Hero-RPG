using System.Collections;
using UnityEngine;

public class LongAttackCharacter : Character
{
    [SerializeField] private Transform tfmSwnBullet;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private int[] numberOfAttackType;

    private Coroutine coroutineAttack;
    protected override void DoAttack()
    {
        if (coroutineAttack == null)
            coroutineAttack = StartCoroutine(IEPlayAttackAnimations());
    }

    private IEnumerator IEPlayAttackAnimations()
    {
        while (true)
        {
            for (int indexType = 0; indexType < numberOfAttackType.Length; indexType++)
            {
                for (int indexRepeat = 0; indexRepeat < numberOfAttackType[indexType]; indexRepeat++)
                {
                    characterAnimator.PlayAttackAnimation(indexType + 1);
                    yield return new WaitForSeconds(characterAnimator.Animator.GetCurrentAnimatorStateInfo(0).length); // Wait for the end of animation
                }
            }
        }
    }

    /// <summary>
    /// Method runs from an event animation
    /// </summary>
    public void ActiveButtlet()
    {
        Bullet bullet = Instantiate(bulletPrefab,tfmSwnBullet.position,Quaternion.identity);
        bullet.Init(target);
    }
}


using System.Collections;
using UnityEngine;

public class MeleeAttackCharacter : Character
{
    [SerializeField] private int[] numberOfAttackType;
    [SerializeField] private GameObject gObjATKBox;

    private Coroutine coroutineAttack;

    public int indexType, indexRepeat;

    private void OnDisable()
    {
        gObjATKBox.SetActive(false);
    }

    private void OnEnable()
    {
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
            coroutineAttack = StartCoroutine(IEPlayAttackAnimation(numberOfAttackType, AttackAnimationType.MeleeAttack));
        }
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


}

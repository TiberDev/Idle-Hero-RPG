using UnityEngine;
using UnityEngine.Events;

public class Test : MonoBehaviour
{
    [SerializeField] private CharacterAnimator animator;

    [SerializeField] private Transform tfmd;

    public string number_1;
    public string number_1_1;
    public string number_2;

    public UnityAction dieAction;

   public void OnClickScreen()
    {
        Debug.Log("On Click Screen");
    }

    public void OnClickIdle()
    {
        //animator.PlayIdleAnimation();
    }

    public void OnClickMove()
    {
        animator.PlayMoveAnimation();
    }

    public float attackSpeed;
    public void OnClickAttack()
    {
        animator.PlayAttackAnimation(attackSpeed);
    }

    public void OnClickAttackSpeed()
    {
        //animator.SetAttackSpeedAnimaton();
    }

    public void OnClickDie()
    {
        animator.PlayDieAnimation();
    }

    public void OnClickVictory()
    {
        animator.PlayVictoryAnimation();
    }
    public GameObject tfm;

    private void Update()
    {
        //Stopwatch stopwatch = new Stopwatch();
        //stopwatch.Start();
        //for (int i = 0; i < number; i++)
        //{
        //    //Vector3 pos = camera.WorldToScreenPoint(tfmd.position);
        //}
        //stopwatch.Stop();
        //Debug.Log(stopwatch.ElapsedMilliseconds / 1000f);
    }

    public int number;
}




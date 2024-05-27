using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public interface A
{
    void RunI();
}

public abstract class Person
{
    public abstract string Run();
}

public class Student : Person
{
    public override string Run()
    {
        return "I run by foots";

    }
}

public class Teacher : Person, A
{
    public override string Run()
    {
        throw new NotImplementedException();
    }

    public void RunI()
    {
        throw new NotImplementedException();
    }
}

public class Test : MonoBehaviour, IPointerUpHandler
{
    [SerializeField] private CharacterAnimator animator;

    [SerializeField] private Transform tfmd;

    public string number_1;
    public string number_1_1;
    public string number_2;

    public UnityAction dieAction;

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

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerEnter);
    }

    public int number;
}



using System.Collections;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField] protected float timeExistingCoolDown;

    private SkillTableItem skillTableItem;

    protected int damage;
    private float curTimeExistingCoolDown;

    public void Init(SkillTableItem item)
    {
        skillTableItem = item;
    }

    public virtual void Execute()
    {

    }

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    protected void SetExistingCooldown()
    {
        //StartCoroutine(IEExistingCounter());
        skillTableItem.SetAmountExistingCoolDown(timeExistingCoolDown);
    }

    //private IEnumerator IEExistingCounter()
    //{
    //    curTimeExistingCoolDown = timeExistingCoolDown;
    //    while (curTimeExistingCoolDown > 0)
    //    {
    //        curTimeExistingCoolDown -= Time.deltaTime;
    //        skillTableItem.SetAmountExistingCoolDown(curTimeExistingCoolDown, timeExistingCoolDown);
    //        yield return null;
    //    }
    //    skillTableItem.SetAmountCoolDown();
    //}
}

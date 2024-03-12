using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField] protected float timeExistingCoolDown;

    protected SkillTableItem skillTableItem;
    protected GameManager gameManager;
    protected ObjectPooling objectPooling;
    protected Transform cachedTfm;

    protected int value;

    public void Init(SkillTableItem item)
    {
        skillTableItem = item;
        gameManager = GameManager.Instance;
        objectPooling = ObjectPooling.Instance;
        cachedTfm = transform;
    }

    public virtual void Execute()
    {
    }

    public virtual void SetParent(Transform tfm, bool worldStay)
    {
        transform.SetParent(tfm, worldStay);
    }

    public void SetValue(int _value)
    {
        value = _value;
    }

    protected void SetExistingCooldown()
    {
        skillTableItem.SetAmountExistingCoolDown(timeExistingCoolDown);
    }

    public virtual void EndExistence()
    {
        objectPooling.RemoveGOInPool(gameObject, PoolType.Skill, name);
    }
}

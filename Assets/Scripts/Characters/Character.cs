using BigInteger = System.Numerics.BigInteger;
using UnityEngine;
using UnityEngine.Events;

public enum CharacterType
{
    Hero = 0,
    Enemy = 1
}

[RequireComponent(typeof(CharacterAnimator), typeof(CharacterMovement))]
public class Character : MonoBehaviour
{
    [SerializeField] protected CharacterHpBar characterHpBar;
    [SerializeField] protected CharacterAnimator characterAnimator;
    [SerializeField] protected CharacterMovement characterMovement;
    [SerializeField] protected CharacterType characterType;
    [SerializeField] protected CharacterInfo characterInfo;

    [SerializeField] private bool isBoss;
    [SerializeField] protected float rangeAttack;

    public Character preTarget, target;
    protected GameManager gameManager;
    protected ObjectPooling objectPooling;
    protected Transform cachedTfm;

    protected bool attackDone;

    private UserInfo userInfo;

    public bool IsBoss { get => isBoss; }

    private void Update()
    {
        // hp recovery
        if (characterInfo.curHp < characterInfo.maxHp && characterType == CharacterType.Hero)
        {
            BigInteger increasedHp = characterInfo.maxHp * (BigInteger)userInfo.hpRecovery;
            characterInfo.curHp = BigInteger.Min(characterInfo.maxHp, characterInfo.curHp + increasedHp);
        }

        if (target == null)
        {
            characterMovement.StopMoving();
            //characterAnimator.PlayIdleAnimation();
            return;
        }

        if (rangeAttack >= Vector3.Distance(target.transform.position, cachedTfm.position)) // in attack range
        {
            if (attackDone) // avoid situation where character rotates while attacking
                SetDirection(target.transform.position);

            // do attack
            DoAttack();
            // stop move
            characterMovement.StopMoving();
        }
        else // move state
        {
            characterAnimator.PlayMoveAnimation();
            characterMovement.Move(target.transform);
            target = characterType == CharacterType.Hero ? FindEnemy() : FindHero(); // in move state character still has to find nearest enemy more
        }
    }

    public virtual void Init()
    {
        cachedTfm = transform;
        gameManager = GameManager.Instance;
        objectPooling = ObjectPooling.Instance;
        switch (characterType)
        {
            case CharacterType.Hero:
                target = FindEnemy();
                break;
            case CharacterType.Enemy:
                // start idle animation 
                characterAnimator.PlayIdleAnimation();
                break;
        }
        // Show info to UI
        characterHpBar.SetHpUI(characterInfo.curHp, characterInfo.maxHp, false);
        // delegate method 
        gameManager.NotifyGameOverAction += SetGameOverState;
    }

    protected void SetGameOverState()
    {
        characterAnimator.PlayVictoryAnimation();
    }

    protected void SetDirection(Vector3 targetPos)
    {
        Vector3 dir = targetPos - cachedTfm.position;
        float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        cachedTfm.eulerAngles = new Vector3(0, -angle + 90, 0);
    }

    protected virtual void AttackEndEvent()
    {
        attackDone = true;
    }

    protected virtual void DieEvent()
    {
        objectPooling.RemoveGOInPool(gameObject, characterType == CharacterType.Enemy ? PoolType.Enemy : PoolType.Hero, gameObject.name);
    }

    protected virtual void DoAttack() { }

    public virtual void SetHpBar(CharacterHpBar hpBar) { }

    /// <summary>
    /// An hero needs to find the target which is the enemy
    /// </summary>
    /// <returns></returns>
    private Character FindEnemy()
    {
        // Find enemy
        return gameManager.FindNearestCharacter(cachedTfm.position, CharacterType.Enemy);
    }

    /// <summary>
    /// An enemy needs to find the target which is the hero
    /// </summary>
    /// <returns></returns>
    protected Character FindHero()
    {
        return gameManager.FindNearestCharacter(cachedTfm.position, CharacterType.Hero);
    }

    public void SetCharacterInfo(UserInfo info)
    {
        userInfo = info;
        if (characterType == CharacterType.Hero)
        {
            characterInfo.damage = 10;
            characterInfo.maxHp = 10000;
            characterInfo.curHp = 10000;
            characterInfo._damage = characterInfo.damage.ToString();
            characterInfo._maxHp = characterInfo.maxHp.ToString();
            characterInfo._curHp = characterInfo.curHp.ToString();
        }
    }

    public void SetCharacterInfo(int damage, int maxHp)
    {
        characterInfo.damage = 1;
        characterInfo.maxHp = 30;
        characterInfo.curHp = 30;
        characterInfo._damage = characterInfo.damage.ToString();
        characterInfo._maxHp = characterInfo.maxHp.ToString();
        characterInfo._curHp = characterInfo.curHp.ToString();
    }

    public void SetAttack()
    {
        characterInfo.damage = userInfo.atk;
    }

    public void SetHp(BigInteger addtionalHp)
    {
        characterInfo.curHp += addtionalHp;
        characterInfo.maxHp += addtionalHp;
        characterHpBar.SetHpUI(characterInfo.curHp, characterInfo.maxHp, true);
    }

    public void SetAttackSpeed(float attackSpeed)
    {
        characterAnimator.SetAttackSpeedAnimaton(attackSpeed);
    }

    public virtual void DetectHero(Character heroDetected)
    {
        if (target != null)
            return;

        // do move animation 
        target = heroDetected;
    }

    public void TakeDamage(BigInteger damageTaken, UnityAction action)
    {
        if (characterInfo.curHp <= 0)
            return;

        characterInfo.curHp -= damageTaken;
        // Show info to UI
        characterHpBar.SetHpUI(characterInfo.curHp, characterInfo.maxHp, true);
        if (characterInfo.curHp <= 0) // die
        {
            target = null;
            characterAnimator.PlayDieAnimation();
            gameManager.NotifyGameOverAction -= SetGameOverState;
            // Remove character in list
            gameManager.RemoveCharacterFromList(this, characterType);
            action?.Invoke();
        }
        characterInfo._damage = characterInfo.damage.ToString();
        characterInfo._maxHp = characterInfo.maxHp.ToString();
        characterInfo._curHp = characterInfo.curHp.ToString();
    }

    public BigInteger GetDamage()
    {
        // hero : caculate damage, hit chance, hit damage, boss damage
        BigInteger totalDamage = characterInfo.damage;
        if (characterType == CharacterType.Hero)
        {
            float random = Random.Range(0f, 1f);
            BigInteger percent = 0;
            if (target.isBoss)
                percent += userInfo.bossDamage - 100;
            if (random <= userInfo.criticalHitChance / 100)
                percent += userInfo.criticalHitDamage;
            totalDamage = characterInfo.damage + userInfo.atk * percent;
        }
        return totalDamage;
    }

    public void TargetDie()
    {
        if (target != null)
        {
            preTarget = target;
            target = characterType == CharacterType.Hero ? FindEnemy() : FindHero();
        }
    }

    public void DetectCharacterInNearRange(Character character)
    {
        target = character;
    }

    public CharacterType GetCharacterType() => characterType;

    private void OnDrawGizmos()
    {
        if (transform.rotation.eulerAngles.y < -90 || transform.rotation.eulerAngles.y > 90)
        {
            Gizmos.DrawLine(transform.position, transform.position + rangeAttack * Vector3.back);
        }
        else
        {
            Gizmos.DrawLine(transform.position, transform.position + rangeAttack * Vector3.forward);
        }
        //float xAxis = Mathf.Tan(transform.rotation.eulerAngles.y / Mathf.Rad2Deg) * rangeAttack;
    }
}

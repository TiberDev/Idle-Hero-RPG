using BigInteger = System.Numerics.BigInteger;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

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
    private float curTimeHpRecovery, decreasedAttackSpeed;
    private BigInteger decreasedAttack;

    private MagicShieldSkill shieldSkill;
    private UserInfo userInfo;

    public bool IsBoss { get => isBoss; }

    private void Update()
    {
        // character die
        if (characterInfo.curHp <= 0)
            return;

        // hp recovery
        if (characterType == CharacterType.Hero && characterInfo.curHp < characterInfo.maxHp)
        {
            curTimeHpRecovery += Time.deltaTime;
            if (curTimeHpRecovery >= 1)
            {
                curTimeHpRecovery = 0;
                BigInteger increasedHp = characterInfo.maxHp * (BigInteger)userInfo.hpRecovery / 100;
                characterInfo.curHp = BigInteger.Min(characterInfo.maxHp, characterInfo.curHp + increasedHp);
                // Show info to UI
                characterHpBar.SetHpUI(characterInfo.curHp, characterInfo.maxHp, true);
            }
        }

        if (target == null)
        {
            characterMovement.StopMoving();
            //characterAnimator.PlayIdleAnimation();
            return;
        }

        if (rangeAttack >= Vector3.Distance(target.GetTransform().position, cachedTfm.position)) // in attack range
        {
            if (attackDone) // avoid situation where character rotates while attacking
                SetDirection(target.GetTransform().position);

            // do attack
            DoAttack();
            // stop move
            characterMovement.StopMoving();
        }
        else // move state
        {
            characterAnimator.PlayMoveAnimation();
            characterMovement.Move(target.GetTransform());
            target = characterType == CharacterType.Hero ? FindEnemy() : FindHero(); // in move state character still has to find nearest enemy more
        }
    }

    public Transform GetTransform() => cachedTfm;

    public virtual void Init()
    {
        curTimeHpRecovery = 0;
        cachedTfm = transform;
        gameManager = GameManager.Instance;
        objectPooling = ObjectPooling.Instance;
        switch (characterType)
        {
            case CharacterType.Hero:
                //target = FindEnemy();
                //SetDirection(target.GetTransform().position);
                break;
            case CharacterType.Enemy:
                List<Character> heroes = gameManager.GetCharacters(CharacterType.Hero);
                if (heroes.Count > 0)
                    SetDirection(heroes[0].GetTransform().position);
                //target = FindHero();
                break;
        }
        // start idle animation 
        characterAnimator.PlayIdleAnimation();
        // Show info to UI
        characterHpBar.SetHpUI(characterInfo.curHp, characterInfo.maxHp, false);
        // delegate method 
        gameManager.NotifyGameOverAction += SetGameOverState;
    }

    protected void SetGameOverState()
    {
        characterAnimator.PlayVictoryAnimation();
    }

    public void SetDirection(Vector3 targetPos)
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
        objectPooling.RemoveGOInPool(gameObject, characterType == CharacterType.Enemy ? PoolType.Enemy : PoolType.Hero, name);
    }

    protected virtual void DoAttack() { }

    public virtual void SetHpBar(CharacterHpBar hpBar) { }

    public CharacterHpBar GetHpBar() => characterHpBar;

    /// <summary>
    /// An hero needs to find the target which is the enemy
    /// </summary>
    /// <returns></returns>
    public Character FindEnemy()
    {
        // Find enemy
        return gameManager.FindNearestCharacter(cachedTfm.position, CharacterType.Enemy);
    }

    /// <summary>
    /// An enemy needs to find the target which is the hero
    /// </summary>
    /// <returns></returns>
    public Character FindHero()
    {
        return gameManager.FindNearestCharacter(cachedTfm.position, CharacterType.Hero);
    }

    public void SetCharacterInfo(UserInfo info)
    {
        userInfo = info;
        if (characterType == CharacterType.Hero)
        {
            characterInfo.damage = 5;
            characterInfo.maxHp = 100;
            characterInfo.curHp = 100;
            characterInfo._damage = characterInfo.damage.ToString();
            characterInfo._maxHp = characterInfo.maxHp.ToString();
            characterInfo._curHp = characterInfo.curHp.ToString();
        }
    }

    public void SetCharacterInfo(int damage, int maxHp)
    {
        userInfo = new UserInfo();
        userInfo.atkSpeed = 1; // attack speed of all enemies is 1
        characterInfo.damage = 5;
        characterInfo.maxHp = 100;
        characterInfo.curHp = 100;
        characterInfo._damage = characterInfo.damage.ToString();
        characterInfo._maxHp = characterInfo.maxHp.ToString();
        characterInfo._curHp = characterInfo.curHp.ToString();
    }
    /// <summary>
    /// Set attack  for hero
    /// </summary>
    public void SetAttack()
    {
        characterInfo.damage = userInfo.atk;
    }

    /// <summary>
    /// Set attack for enemy
    /// </summary>
    public void SetAttack(int percent, bool addtional)
    {
        if (!addtional)
        {
            decreasedAttack = characterInfo.damage * percent / 100;
            characterInfo.damage -= decreasedAttack;
        }
        else
        {
            characterInfo.damage += decreasedAttack;
            decreasedAttack = 0;
        }
    }

    public void SetMaxHp(BigInteger addtionalHp)
    {
        characterInfo.curHp += addtionalHp;
        characterInfo.maxHp += addtionalHp;
        characterHpBar.SetHpUI(characterInfo.curHp, characterInfo.maxHp, true);
    }

    public void SetShieldSkill(MagicShieldSkill shield)
    {
        shieldSkill = shield;
    }

    public void SetAttackSpeed(float attackSpeed)
    {
        characterAnimator.SetAttackSpeedAnimaton(attackSpeed);
    }

    public void SetAttackSpeed(int percent, bool addtional)
    {
        if (!addtional)
        {
            decreasedAttackSpeed = GetAttackSpeed() * percent / 100;
            userInfo.atkSpeed -= decreasedAttackSpeed;
        }
        else
        {
            userInfo.atkSpeed += decreasedAttackSpeed;
            decreasedAttackSpeed = 0;
        }
        characterAnimator.SetAttackSpeedAnimaton(userInfo.atkSpeed);
    }

    public float GetAttackSpeed() => userInfo.atkSpeed;

    public void TakeDamage(BigInteger damageTaken, UnityAction<Character> action)
    {
        if (characterInfo.curHp <= 0)
        {
            action?.Invoke(this);
            return;
        }

        if (shieldSkill != null)
            damageTaken = shieldSkill.DecreaseDamageTaken(damageTaken);

        characterInfo.curHp -= damageTaken;
        // Show info to UI
        characterHpBar.SetHpUI(characterInfo.curHp, characterInfo.maxHp, true);
        if (characterInfo.curHp <= 0) // die
        {
            target = null;
            characterMovement.StopMoving();
            characterAnimator.PlayDieAnimation();
            gameManager.NotifyGameOverAction -= SetGameOverState;
            // Remove character in list
            gameManager.RemoveCharacterFromList(this, characterType);
            action?.Invoke(this);
        }
        characterInfo._damage = characterInfo.damage.ToString();
        characterInfo._maxHp = characterInfo.maxHp.ToString();
        characterInfo._curHp = characterInfo.curHp.ToString();
    }

    public BigInteger GetTotalDamage(bool boss)
    {
        // hero : caculate damage, hit chance, hit damage, boss damage
        BigInteger totalDamage = characterInfo.damage;
        if (characterType == CharacterType.Hero)
        {
            float random = Random.Range(0f, 1f);
            BigInteger percent = 0;
            if (boss) // interactive character is boss
                percent += userInfo.bossDamage - 100;
            if (random <= userInfo.criticalHitChance / 100)
                percent += userInfo.criticalHitDamage;
            totalDamage = characterInfo.damage + userInfo.atk * percent;
        }
        return totalDamage;
    }

    public BigInteger GetDamage() => characterInfo.damage;

    public BigInteger GetMaxHp() => characterInfo.maxHp;

    public Vector3 GetTargetPosition()
    {
        if (target != null)
            return target.GetTransform().position;
        if (preTarget != null)
            return preTarget.GetTransform().position;
        return Vector3.zero;
    }

    /// <summary>
    /// Change target
    /// </summary>
    /// <param name="newTarget">Target is found</param>
    public void SetTarget(Character newTarget)
    {
        target = newTarget;
    }

    public void CheckTarget(Character characterDie)
    {
        if (target == characterDie)
        {
            preTarget = target;
            target = characterType == CharacterType.Hero ? FindEnemy() : FindHero();
        }
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


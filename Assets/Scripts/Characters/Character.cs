using BigInteger = System.Numerics.BigInteger;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor.Experimental.GraphView;

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
    [SerializeField] private Transform tfmHead;

    [SerializeField] private bool isBoss;
    [SerializeField] protected float rangeAttack;

    protected GameManager gameManager;
    protected ObjectPooling objectPooling;
    protected Transform cachedTfm;
    protected Character preTarget, target;

    private MagicShieldSkill shieldSkill;
    private UserInfo userInfo;
    private UnityAction dieAction;

    protected bool attackDone;
    private bool critical, isAttacking;
    private float curTimeHpRecovery, decreasedAttackSpeed;
    private BigInteger decreasedAttack;

    public bool IsBoss { get => isBoss; }
    public UnityAction DieAction { get => dieAction; set => dieAction = value; }
    public bool Critical { get => critical; }

    private void Awake()
    {
        cachedTfm = transform;
    }

    private void Update()
    {
        // character die
        if (characterInfo.curHp <= 0)
            return;

        // hp recovery
        if (characterType == CharacterType.Hero && characterInfo.curHp < characterInfo.maxHp)
        {

            //curTimeHpRecovery += Time.deltaTime;
            //if (curTimeHpRecovery >= 1)
            //{
            //    curTimeHpRecovery = 0;
            //    BigInteger increasedHp = characterInfo.maxHp * (int)userInfo.hpRecovery / 100;
            //    characterInfo.curHp = BigInteger.Min(characterInfo.maxHp, characterInfo.curHp + increasedHp);
            //    characterInfo._curHp = characterInfo.curHp.ToString();
            //    // Show info to UI
            //    characterHpBar.SetFillAmountUI(characterInfo.curHp, characterInfo.maxHp, true);
            //}
        }

        if (target == null)
        {
            //characterMovement.StopMoving();
            isAttacking = false;
            //characterAnimator.PlayIdleAnimation();
            return;
        }

        if (rangeAttack >= Vector3.Distance(target.GetTransform().position, cachedTfm.position)) // in attack range
        {
            if (!isAttacking)
            {
                // stop move
                characterMovement.StopMoving();
                SetDirection(target.GetTransform().position);
                isAttacking = true;
            }
            if (attackDone) // avoid situation where character rotates while attacking
                SetDirection(target.GetTransform().position);

            // do attack
            DoAttack();
            if(characterType == CharacterType.Hero)
            Debug.Log("Attackin");
        }
        else // move state
        {
            if(characterType == CharacterType.Hero)
            Debug.Log("Moving");
            isAttacking = false;
            characterAnimator.PlayMoveAnimation();
            characterMovement.Move(target.GetTransform());
            SetTarget(characterType == CharacterType.Hero ? FindEnemy() : FindHero()); // in move state character still has to find nearest enemy or hero more
        }
    }

    public Transform GetHeadTransform() => tfmHead;

    public Transform GetTransform() => cachedTfm;

    public virtual void Init()
    {
        curTimeHpRecovery = 0;
        gameManager = GameManager.Instance;
        objectPooling = ObjectPooling.Instance;

        if (characterType == CharacterType.Enemy)
        {
            Character hero = FindHero();
            SetTarget(hero);
            SetDirection(hero.GetTransform().position);
        }
        //// start idle animation 
        //characterAnimator.PlayIdleAnimation();
        // Show info to UI
        characterHpBar.SetFillAmountUI(characterInfo.curHp, characterInfo.maxHp, false);
        // delegate method 
        gameManager.NotifyGameOverAction += EndGameState;
    }

    protected void EndGameState()
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
            characterInfo.damage = userInfo.atk;
            characterInfo.maxHp = userInfo.hp;
            characterInfo.curHp = userInfo.hp;
            characterInfo._damage = characterInfo.damage.ToString();
            characterInfo._maxHp = characterInfo.maxHp.ToString();
            characterInfo._curHp = characterInfo.curHp.ToString();
        }
    }

    public void SetCharacterInfo(BigInteger damage, BigInteger maxHp)
    {
        userInfo = new UserInfo();
        userInfo.atkSpeed = 1; // attack speed of all enemies is 1
        characterInfo.damage = damage;
        characterInfo.maxHp = maxHp;
        characterInfo.curHp = maxHp;
        characterInfo._damage = characterInfo.damage.ToString();
        characterInfo._maxHp = characterInfo.maxHp.ToString();
        characterInfo._curHp = characterInfo.curHp.ToString();
    }

    /// <summary>
    /// Set attack for hero
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

    public void SetMaxHp(BigInteger newMaxHP)
    {
        BigInteger percent = characterInfo.curHp * 100 / characterInfo.maxHp;
        Debug.Log($"Early: {characterInfo.curHp * 100 / characterInfo.maxHp}");
        if (percent > 0)
            characterInfo.curHp = newMaxHP * percent / 100;
        characterInfo.maxHp = newMaxHP;
        characterInfo._curHp = characterInfo.curHp.ToString();
        characterInfo._maxHp = characterInfo.maxHp.ToString();
        Debug.Log($"Later: {characterInfo.curHp * 100 / characterInfo.maxHp}");
        characterHpBar.SetFillAmountUI(characterInfo.curHp, characterInfo.maxHp, false);
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

    public void TakeDamage(BigInteger damageTaken, DamageTakenType damageTakenType)
    {
        if (characterInfo.curHp <= 0 || DarkBoardLoadingUI.Fading || damageTaken <= 0)
            return;

        if (shieldSkill != null)
            damageTaken = shieldSkill.DecreaseDamageTaken(damageTaken);

        characterInfo.curHp -= damageTaken;
        // damage effect
        DamageEffectController.Instance.CreateDmgEffect(tfmHead.position, FillData.Instance.FormatNumber(damageTaken), damageTakenType);
        // Show info to UI
        characterHpBar.SetFillAmountUI(characterInfo.curHp, characterInfo.maxHp, true);
        if (characterInfo.curHp <= 0) // die
        {
            target = null;
            characterMovement.StopMoving();
            characterAnimator.PlayDieAnimation();
            gameManager.NotifyGameOverAction -= EndGameState;
            // Remove character in list
            gameManager.RemoveCharacterFromList(this, characterType);
            dieAction?.Invoke();
            dieAction = null;
            // gold effect
            if (characterType == CharacterType.Enemy)
            {
                BigInteger goldKillEnemy = MapManager.Instance.GetGoldKillEnemy(isBoss);
                goldKillEnemy += goldKillEnemy * (gameManager.UserInfo.goldObtain - 100) / 100;
                Debug.Log($"total gold kill enemy: {goldKillEnemy}  goldObtain: {gameManager.UserInfo.goldObtain}");
                gameManager.SetGold(goldKillEnemy, true);
                GoldEffectController.Instance.CreateGoldIconEffect(cachedTfm.position, () => gameManager.UiManager.SetTextGold(goldKillEnemy, true));
            }
        }
        characterInfo._damage = characterInfo.damage.ToString();
        characterInfo._maxHp = characterInfo.maxHp.ToString();
        characterInfo._curHp = characterInfo.curHp.ToString();
    }

    public BigInteger GetTotalDamage(bool boss)
    {
        if (characterInfo.curHp <= 0)
            return 0;

        // hero : caculate damage, hit chance, hit damage, boss damage
        BigInteger totalDamage = characterInfo.damage;
        if (characterType == CharacterType.Hero)
        {
            float random = Random.Range(0f, 100f);
            BigInteger percent = 0;
            if (boss) // interactive character is boss
                percent += userInfo.bossDamage - 100;

            if (random <= userInfo.criticalHitChance)
            {
                percent += userInfo.criticalHitDamage;
                critical = true;
            }
            else
                critical = false;

            totalDamage = characterInfo.damage + characterInfo.damage * percent / 100;
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
        if (target != null) // remove pre action
            target.dieAction -= CheckTargetDie;

        target = newTarget;
        if (target == null)
            return;

        target.dieAction += CheckTargetDie;
    }

    public virtual void CheckTargetDie()
    {
        preTarget = target;
        SetTarget(characterType == CharacterType.Hero ? FindEnemy() : FindHero());
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


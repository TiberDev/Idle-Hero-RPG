using BigInteger = System.Numerics.BigInteger;
using UnityEngine;
using UnityEngine.Events;

public enum CharacterType
{
    Hero = 0,
    Enemy = 1
}

public enum HeroType
{
    Knight,
    Gunner,
    Wizard
}

[RequireComponent(typeof(CharacterAnimator), typeof(CharacterMovement))]
public class Character : MonoBehaviour
{
    [SerializeField] protected CharacterHpBar characterHpBar;
    [SerializeField] protected CharacterAnimator characterAnimator;
    [SerializeField] protected CharacterMovement characterMovement;
    [SerializeField] protected CharacterType characterType;
    [SerializeField] private HeroType heroType;
    [SerializeField] private Transform tfmHead;

    [SerializeField] private bool isBoss;
    [SerializeField] protected float rangeAttack;

    protected GameManager gameManager;
    protected Transform cachedTfm;
    protected Character preTarget, target;

    private MagicShieldSkill shieldSkill;
    protected UserInfo userInfo;
    private UnityAction dieAction;

    protected bool attackDone;
    private bool critical, isAttacking;
    private float curTimeHpRecovery, decreasedAttackSpeed;
    protected BigInteger curHp;
    private BigInteger decreasedAttack;

    public bool IsBoss { get => isBoss; }
    public UnityAction DieAction { get => dieAction; set => dieAction = value; }
    public bool Critical { get => critical; }

    private void Awake()
    {
        cachedTfm = transform;
    }

    protected virtual void OnEnable()
    {
        attackDone = true;
    }

    private void Update()
    {
        // character die
        if (curHp <= 0)
            return;

        // hp recovery
        if (characterType == CharacterType.Hero && curHp < userInfo.hp)
        {

            curTimeHpRecovery += Time.deltaTime;
            if (curTimeHpRecovery >= 1)
            {
                curTimeHpRecovery = 0;
                BigInteger increasedHp = userInfo.hp * (int)userInfo.hpRecovery / 100;
                curHp = BigInteger.Min(userInfo.hp, curHp + increasedHp);
                // Show info to UI
                characterHpBar.SetFillAmountUI(curHp, userInfo.hp, true);
            }
        }

        if (target == null)
        {
            isAttacking = false;
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
            if (attackDone)
            {
                // do attack
                DoAttack();
                // avoid situation where character rotates while attacking
                SetDirection(target.GetTransform().position);
                if (characterType == CharacterType.Hero)
                    SoundManager.Instance.PlayAttackSound(heroType);
            }
        }
        else // move state
        {
            if (!attackDone) // avoid moving while attack animation is executing
                return;

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
        if (characterType == CharacterType.Enemy)
        {
            Character hero = FindHero();
            SetTarget(hero);
            SetDirection(hero.GetTransform().position);
        }
        // Show info to UI
        characterHpBar.SetFillAmountUI(curHp, userInfo.hp, false);
        // Register for game over event
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
        ObjectPooling.Instance.RemoveGOInPool(gameObject, characterType == CharacterType.Enemy ? PoolType.Enemy : PoolType.Hero);
    }

    protected virtual void DoAttack() { }

    protected virtual void DoDie() { }

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

    /// <summary>
    /// Set info to hero
    /// </summary>
    /// <param name="info"></param>
    public void SetCharacterInfo(UserInfo info)
    {
        userInfo = info;
        curHp = userInfo.hp;
    }

    /// <summary>
    /// Set info to enemy
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="maxHp"></param>
    public void SetCharacterInfo(BigInteger damage, BigInteger maxHp)
    {
        userInfo = new UserInfo();
        userInfo.atkSpeed = 1; // attack speed of all enemies is 1
        userInfo.atk = damage;
        userInfo.hp = maxHp;
        curHp = maxHp;
    }

    /// <summary>
    /// Set attack for enemy
    /// </summary>
    public void SetAttack(int percent, bool addtional)
    {
        if (!addtional)
        {
            decreasedAttack = userInfo.atk * percent / 100;
            userInfo.atk -= decreasedAttack;
        }
        else
        {
            userInfo.atk += decreasedAttack;
            decreasedAttack = 0;
        }
    }

    public void SetMaxHp(BigInteger preHp)
    {
        BigInteger percent = curHp * 100 / preHp;
        if (percent > 0)
        {
            curHp = userInfo.hp * percent / 100 + 1;
            if (curHp > userInfo.hp)
                curHp = userInfo.hp;
        }
        characterHpBar.SetFillAmountUI(curHp, userInfo.hp, false);
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
        if (curHp <= 0 || DarkBoardLoadingUI.Fading || damageTaken <= 0)
            return;

        if (shieldSkill != null)
            damageTaken = shieldSkill.DecreaseDamageTaken(damageTaken);

        curHp -= damageTaken;
        // damage effect
        DamageEffectController.Instance.CreateDmgEffect(tfmHead.position, NumberConverter.Instance.FormatNumber(damageTaken), damageTakenType);
        // Show info to UI
        characterHpBar.SetFillAmountUI(curHp, userInfo.hp, true);
        if (curHp <= 0) // die
        {
            DoDie();
            preTarget = target;
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
                gameManager.SetGold(goldKillEnemy, true);
                GoldEffectController.Instance.CreateGoldIconEffect(cachedTfm.position, () => gameManager.UiManager.SetTextGold(goldKillEnemy, true));
            }
        }
    }

    public BigInteger GetTotalDamage(bool boss)
    {
        if (curHp <= 0)
            return 0;

        // hero : caculate damage, hit chance, hit damage, boss damage
        BigInteger totalDamage = userInfo.atk;
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

            totalDamage = userInfo.atk + userInfo.atk * percent / 100;
        }
        return totalDamage;
    }

    public BigInteger GetDamage() => userInfo.atk;

    public BigInteger GetMaxHp() => userInfo.hp;

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


 using UnityEngine;

public enum CharacterType
{
    Hero = 0,
    Enemy = 1
}

public class Character : MonoBehaviour
{
    [SerializeField] protected CharacterMovement characterMovement;
    [SerializeField] protected CharacterType characterType;

    [SerializeField] protected float rangeAttack;

    private Character target;
    private GameManager instanceGM;

    private void Start()
    {
        instanceGM = GameManager.Instance;
        Init();
    }

    private void Init()
    {
        switch (characterType)
        {
            case CharacterType.Hero:
                target = FindEnemy();
                // do move animation

                characterMovement.Move(target.transform);
                break;
            case CharacterType.Enemy:
                // start idle animation
                break;
        }
    }

    private void Update()
    {
        if(target != null)
        {
            if(rangeAttack >= Vector3.Distance(target.transform.position, transform.position)) // in attack range
            {
                // do attack
                // stop move
                characterMovement.StopMoving();
            }
            else
            {
                characterMovement.Move(target.transform);
            }
        }
    }

    public Character FindEnemy()
    {
        // Find enemy
        return instanceGM.GetEnemy();
    }

    public virtual void DetectHero(Character heroDetected)
    {
        // do move animation 
        target = heroDetected;
        characterMovement.Move(heroDetected.transform);
    }

    private void OnDrawGizmos()
    {
        if(transform.rotation.eulerAngles.y < -90 || transform.rotation.eulerAngles.y > 90)
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

public class Boss : MeleeAttackCharacter
{
    public override void Init()
    {
        gameManager = GameManager.Instance;
        // Find hero
        Character hero = FindHero();
        SetTarget(hero);
        SetDirection(hero.GetTransform().position);
        gameManager.NotifyGameOverAction += EndGameState;
    }

    public override void SetHpBar(CharacterHpBar hpBar)
    {
        characterHpBar = hpBar;
        // Show info to UI
        characterHpBar.SetFillAmountUI(curHp, userInfo.hp, false);
    }
}

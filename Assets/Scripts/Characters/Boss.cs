
using UnityEngine;

public class Boss : MeleeAttackCharacter
{
    public override void Init()
    {
        cachedTfm = transform;
        gameManager = GameManager.Instance;
        objectPooling = ObjectPooling.Instance;
        // Find hero
        SetTarget(FindHero());
        //// Show info to UI
        //characterHpBar.SetHpUI(characterInfo.curHp, characterInfo.maxHp, false);
        gameManager.NotifyGameOverAction += SetGameOverState;
    }

    public override void SetHpBar(CharacterHpBar hpBar)
    {
        characterHpBar = hpBar;
        // Show info to UI
        characterHpBar.SetHpUI(characterInfo.curHp, characterInfo.maxHp, false);
    }
}

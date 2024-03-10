using UnityEngine;
using UnityEngine.UI;

public class AddtionalEffectItem : MonoBehaviour
{
    [SerializeField] private Image imgEffect;
    [SerializeField] private GameObject gObjBlock;
    [SerializeField] private RectTransform rectTfmItem;

    private AddtionalEffect addtionalEffect;
    private HeroInfoUI heroInfoUI;

    public void Init(AddtionalEffect effect,HeroInfoUI heroInfo)
    {
        heroInfoUI = heroInfo;
        addtionalEffect = effect;
    }

    public void SetEffectImage(Sprite spt)
    {
        imgEffect.sprite = spt;
    }

    public void SetEffectBlock(bool isActive)
    {
        gObjBlock.SetActive(isActive);
    }

    public void OnClickEffect()
    {
        heroInfoUI.SetDescribeEffect(rectTfmItem, GetDescribe(addtionalEffect.type, addtionalEffect.percent));
    }

    public string GetDescribe(AddtionalEffectType type, int percent)
    {
        string describe;
        switch (type)
        {
            case AddtionalEffectType.IncreaseATK:
                describe = "ATK";
                break;
            case AddtionalEffectType.IncreaseHp:
                describe = "Hp";
                break;
            case AddtionalEffectType.IncreaseCriticalHitDamage:
                describe = "Critical Hit Damage";
                break;
            case AddtionalEffectType.IncreaseGoldObtain:
                describe = "Gold Obtain";
                break;
            case AddtionalEffectType.IncreaseSkillDamage:
                describe = "Skill Damage";
                break;
            case AddtionalEffectType.IncreaseBossDamage:
                describe = "Boss Damage";
                break;
            default:
                describe = null;
                break;
        }
        return percent + "%" + " increased " + describe;
    }
}
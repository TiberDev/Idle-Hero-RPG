using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UserInfoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text txtATK, txtHp, txtHitCriticalChance, txtHitCriticalDamage, txtATKSpeed, txtHpRecovery, txtSkillDamage, txtGoldObtain, txtBossDamage;
    [SerializeField] private GameObject gObj;
    [SerializeField] private RectTransform rectTfm;

    [SerializeField] private float scalingUpTime, scalingDownTime;

    private Coroutine corouInfo;

    private void OnEnable()
    {
        SoundManager.Instance.PlayClickSound();
        UserInfo userInfo = UserInfoManager.Instance.GetUserInfo();
        txtATK.text = NumberConverter.Instance.FormatNumber(userInfo.atk);
        txtHp.text = NumberConverter.Instance.FormatNumber(userInfo.hp);
        txtATKSpeed.text = userInfo.atkSpeed.ToString();
        txtHpRecovery.text = userInfo.hpRecovery.ToString() + "%";
        txtHitCriticalChance.text = userInfo.criticalHitChance.ToString() + "%";
        txtHitCriticalDamage.text = NumberConverter.Instance.FormatNumber(userInfo.criticalHitDamage) + "%";
        txtSkillDamage.text = NumberConverter.Instance.FormatNumber(userInfo.skillDamage) + "%";
        txtGoldObtain.text = NumberConverter.Instance.FormatNumber(userInfo.goldObtain) + "%";
        txtBossDamage.text = NumberConverter.Instance.FormatNumber(userInfo.bossDamage) + "%";
    }

    public void SetActive(bool active)
    {
        // effect
        TransformUIPanel(active);
    }

    private void SetInActive()
    {
        gObj.SetActive(false);
        corouInfo = null;
    }

    public void TransformUIPanel(bool open)
    {
        if (open)
        {
            gObj.SetActive(true);
            corouInfo = StartCoroutine(UITransformController.Instance.IEScalingRect(rectTfm, Vector2.one * 0.5f, Vector2.one, scalingUpTime, LerpType.EaseOutBack));
        }
        else
        {
            StopCoroutine(corouInfo);
            corouInfo = null;
            corouInfo = StartCoroutine(UITransformController.Instance.IEScalingRect(rectTfm, rectTfm.localScale, Vector2.one * 0.5f, scalingDownTime, LerpType.EaseInBack, SetInActive));
        }

    }

    public void OnClickCover(BaseEventData baseEventData)
    {
        PointerEventData pointerEvent = baseEventData as PointerEventData;
        if (pointerEvent.pointerEnter == gObj)
        {
            SetActive(false);
        }
    }
}

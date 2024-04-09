using TMPro;
using UnityEngine;

public class UserInfoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text txtATK, txtHp, txtHitCriticalChance, txtHitCriticalDamage, txtATKSpeed, txtHpRecovery, txtSkillDamage, txtGoldObtain, txtBossDamage;
    [SerializeField] private GameObject gObj;
    [SerializeField] private RectTransform rectTfm;

    [SerializeField] private float scalingUpTime, scalingDownTime;

    private Coroutine corouInfo;

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

    private void OnEnable()
    {
        UserInfo userInfo = UserInfoManager.Instance.GetUserInfo();
        txtATK.text = FillData.Instance.FormatNumber(userInfo.atk);
        txtHp.text = FillData.Instance.FormatNumber(userInfo.hp);
        txtATKSpeed.text = userInfo.atkSpeed.ToString();
        txtHpRecovery.text = userInfo.hpRecovery.ToString() + "%";
        txtHitCriticalChance.text = userInfo.criticalHitChance.ToString() + "%";
        txtHitCriticalDamage.text = FillData.Instance.FormatNumber(userInfo.criticalHitDamage) + "%";
        txtSkillDamage.text = FillData.Instance.FormatNumber(userInfo.skillDamage) + "%";
        txtGoldObtain.text = FillData.Instance.FormatNumber(userInfo.goldObtain) + "%";
        txtBossDamage.text = FillData.Instance.FormatNumber(userInfo.bossDamage) + "%";
    }
}

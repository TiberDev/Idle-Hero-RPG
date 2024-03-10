using TMPro;
using UnityEngine;

public class UserInfoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text txtATK, txtHp, txtHitCriticalChance, txtHitCriticalDamage, txtATKSpeed, txtHpRecovery, txtSkillDamage, txtGoldObtain, txtBossDamage;

    private void OnEnable()
    {
        UserInfo userInfo = UserInfoManager.Instance.userInfo;
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

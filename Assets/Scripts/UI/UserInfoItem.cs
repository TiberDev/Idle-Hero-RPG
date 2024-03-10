using TMPro;
using UnityEngine;

public class UserInfoItem : MonoBehaviour
{
    [SerializeField] private TMP_Text txtName, txtStat;
    [SerializeField] private string statName;

    private void Start()
    {
        txtName.text = statName;
    }

    public void SetItemInfo(string txt)
    {
        txtStat.text = txt;
    }

}

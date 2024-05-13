using System.Numerics;
using TMPro;
using UnityEngine;

public class GemsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text txtGold, txtPinkGem, txtBlueGem;

    public void SetTextGold(BigInteger gold)
    {
        txtGold.text = NumberConverter.Instance.FormatNumber(gold);
    }

    public void SetTextBlueGem(BigInteger gem)
    {
        txtBlueGem.text = NumberConverter.Instance.FormatNumber(gem);
    }

    public void SetTextPinkGem(BigInteger gem)
    {
        txtPinkGem.text = NumberConverter.Instance.FormatNumber(gem);
    }
}

using System.Numerics;
using TMPro;
using UnityEngine;

public class GemsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text txtGold, txtPinkGem, txtBlueGem;

    private BigInteger goldUI, blueGemUI, pinkGemUI;

    public void SetTextGold(BigInteger gold, bool addtional)
    {
        goldUI += addtional ? gold : -gold;
        txtGold.text = NumberConverter.Instance.FormatNumber(goldUI);
        EventDispatcher.Push(EventId.CheckGoldToEnhance, goldUI);
    }

    public void SetTextBlueGem(BigInteger gem, bool addtional)
    {
        blueGemUI += addtional ? gem : -gem;
        txtBlueGem.text = NumberConverter.Instance.FormatNumber(blueGemUI);
    }

    public void SetTextPinkGem(BigInteger gem, bool additional)
    {
        pinkGemUI += additional ? gem : -gem;
        txtPinkGem.text = NumberConverter.Instance.FormatNumber(pinkGemUI);
    }
}

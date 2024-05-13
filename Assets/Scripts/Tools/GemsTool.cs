using UnityEngine;

public class GemsTool : MonoBehaviour
{
    public void OnClickGet100Golds()
    {
        GameManager.Instance.SetGold(100, true, false);
    }

    public void OnClickGet100BlueGems()
    {
        GameManager.Instance.SetBlueGem(100, true);
    }

    public void OnClickGet100PinkGems()
    {
        GameManager.Instance.SetPinkGem(100, true);
    }
}

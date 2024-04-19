using UnityEngine;

public class CharacterPositionManager : MonoBehaviour
{
    [SerializeField] private Wave[] waves;

    private GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.Instance;
        MapManager mapManager = gameManager.GetMapManager();
        mapManager.SetWaveList(waves);
        gameManager.RenewGameState(false);
    }

}

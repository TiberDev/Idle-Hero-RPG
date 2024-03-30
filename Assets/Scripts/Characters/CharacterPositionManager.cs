using UnityEngine;

[System.Serializable]
public class CharacterPosition
{
    public Transform tfmSwnHero;
    public int frontWaveIndex, behindWaveIndex;
}

public class CharacterPositionManager : MonoBehaviour
{
    [SerializeField] private Wave[] waves;
    [SerializeField] private CharacterPosition[] characterPositions;

    private GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.Instance;
        MapManager mapManager = gameManager.GetMapManager();
        mapManager.SetWaveList(waves);
        gameManager.SetHeroSwnPosition(characterPositions);
        gameManager.RenewGameState(false);
    }

}

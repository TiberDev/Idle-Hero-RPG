using UnityEngine;

public class CharacterPositionManager : MonoBehaviour
{
    [SerializeField] private Transform[] tfmSwnEnemyList;
    [SerializeField] private CharacterPosition[] characterPositions;

    private GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.SetEnemySwnPosition(tfmSwnEnemyList);
        gameManager.SetHeroSwnPosition(characterPositions);
        gameManager.StartGameState();
    }

}

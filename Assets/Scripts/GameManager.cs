using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Character tfmE;

    public Character GetEnemy()
    {
        return tfmE;
    }
}

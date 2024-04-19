using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MapConfig", menuName = "ScriptableObjects/MapConfig")]
public class SObjMapConfig : ScriptableObject
{
    public int goldKillCreep, goldKillBoss;
    public int baseATKCreep, baseHPCreep;
    public int baseATKBoss, baseHPBoss;
    public Round[] roundList;

    [Space(5)]
    [Header("Loop map")]
    public int increasedBaseATK;
    public int increasedBaseHP;
    public int increasedGold;
}

[Serializable]
public class Round
{
    public int increaseGoldPercentage;
    public int increaseBaseATKPercentage;
    public int increaseBaseHPPercentage;
    public Turn[] turnList;
}

[Serializable]
public class Turn
{
    public int increaseGoldPercentage;
    public int increaseATKPercentage;
    public int increaseHPPercentage;
    public int numberOfWaves;
    public Character boss;
}

[Serializable]
public class Wave
{
    public EnemyTypeInWave[] enemyTypesInWave;
    public float leftRange, rightRange, topRange, bottomRange;
    public float swnYPosition;
}

[Serializable]
public class EnemyTypeInWave
{
    public Character prefab;
    public int amount;
}


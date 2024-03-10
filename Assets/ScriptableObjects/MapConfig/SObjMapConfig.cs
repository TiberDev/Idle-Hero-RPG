using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MapConfig", menuName = "ScriptableObjects/MapConfig")]
public class SObjMapConfig : ScriptableObject
{
    public int baseATKCreep, baseHPCreep;
    public int baseATKBoss, baseHPBoss;
    public Round[] roundList;
}

[Serializable]
public class Round
{
    public int increaseBaseATKPercentage;
    public int increaseBaseHPPercentage;
    public Turn[] turnList;
}

[Serializable]
public class Turn
{
    public int increaseATKPercentage;
    public int increaseHPPercentage;
    public Wave[] waveList;
    public Character boss;
}

[Serializable]
public class Wave
{
    public EnemiesInWave[] enemies;
}

[Serializable]
public class EnemiesInWave
{
    public int quantity;
    public Character prefab;
}


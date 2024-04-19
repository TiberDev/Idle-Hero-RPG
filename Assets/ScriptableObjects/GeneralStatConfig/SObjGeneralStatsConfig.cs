using UnityEngine;

[CreateAssetMenu(fileName = "GeneralStatsConfig", menuName = "ScriptableObjects/GeneralStatsConfig")]
public class SObjGeneralStatsConfig : ScriptableObject
{
    public string statName;
    public string statLv1, goldLv1;
    public float goldPerLv, statPerLv;
    public int levelMax;
    public bool isValueSmall;
    public GeneralStatsType type;
}


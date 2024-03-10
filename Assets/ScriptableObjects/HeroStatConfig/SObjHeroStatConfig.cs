using UnityEngine;

[CreateAssetMenu(fileName ="HeroStatConfig",menuName = "ScriptableObjects/HeroStatConfig")]
public class SObjHeroStatConfig : ScriptableObject
{
    public string heroName;
    public int levelMax = 100;
    public int gemToEnhance = 1;
    public int increasedPoint = 3;
    public int pointPerLv = 5;
    public AddtionalEffect[] addtionalEffects;
    public Sprite heroSpt;
    public Character prefabHero;
}

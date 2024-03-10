using UnityEngine;
using UnityEngine.UI;

public class HeroItem : MonoBehaviour
{
    [SerializeField] private Image imgHero;
    [SerializeField] private Button btnHero;
    [SerializeField] private GameObject gObjBlock;

    private HeroStats heroStats;
    private HeroStatsManager heroStatsManager;
    private SObjHeroStatConfig heroStatConfig;

    public void Init(HeroStatsManager statsManager,HeroStats stats,SObjHeroStatConfig config)
    {
        heroStatsManager = statsManager;
        heroStats = stats;
        heroStatConfig = config;
        btnHero.interactable = true;
    }

    public void SetUnblockedHero()
    {
        gObjBlock.SetActive(!heroStats.unblocked);
    }

    public void SetHeroImage(Sprite spt)
    {
        imgHero.sprite = spt; 
    }

    public void OnClickHeroImage()
    {
        heroStatsManager.SetHeroItemSelected(this,heroStatConfig);
        heroStatsManager.SetHeroInfoUI(heroStats,imgHero.sprite);
    }
}

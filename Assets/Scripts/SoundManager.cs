using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioSource asMusic, asSound;
    [SerializeField] private AudioClip norMusicClip, bossMusicClip;
    [SerializeField] private AudioClip clickClip, clickBtnClip, enhanceClip, slashClip, fireClip, shootClip, footstepClip;
    [SerializeField] private float footstepDelay;

    public void PlayNormalMusic()
    {
        asMusic.clip = norMusicClip;
        asMusic.Play();
    }

    public void PlayBossMusic()
    {
        asMusic.clip = bossMusicClip;
        asMusic.Play();
    }

    public void PlayClickSound()
    {
        asSound.PlayOneShot(clickClip);
    }

    public void PlayButtonSound()
    {
        asSound.PlayOneShot(clickBtnClip);
    }

    public void PlayEnhanceClickSound()
    {
        asSound.PlayOneShot(enhanceClip);
    }

    public void PlayFootstepSound()
    {
        asSound.PlayOneShot(footstepClip);
    }

    public void PlayAttackSound(HeroType heroType)
    {
        switch (heroType)
        {
            case HeroType.Knight:
                asSound.PlayOneShot(slashClip);
                break;
            case HeroType.Gunner:
                asSound.PlayOneShot(shootClip);
                break;
            case HeroType.Wizard:
                asSound.PlayOneShot(fireClip);
                break;
        }
    }
}

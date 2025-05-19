using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public AudioMixer AudioMixer;
    public AudioSource BgmSource;
    public AudioSource SfxSource;
    public AudioSource CharacterSource;

    [Header("BGM Clips")]
    public AudioClip MainStart;
    public AudioClip StartGame;
    public AudioClip DrawScrene;
    public AudioClip Game1;

    [Header("SFX Clips")]
    public AudioClip Button;
    public AudioClip Attack;
    public AudioClip Paring;

    [Header("Character Clips")]
    public AudioClip CharacterANBI;
    public AudioClip CharacterLONGINUS;
    public AudioClip CharacterOokumaMari;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlayBGM(AudioClip clip)
    {
        if (BgmSource.isPlaying)
            BgmSource.Stop();

        BgmSource.clip = clip;
        BgmSource.Play();
    }

    public void PlayMainStart() => PlayBGM(MainStart);
    public void PlayStartGame() => PlayBGM(StartGame);
    public void PlayDrawScrene() => PlayBGM(DrawScrene);
    public void PlayGame1() => PlayBGM(Game1);

    public void PlaySFX(AudioClip clip)
    {
        SfxSource.PlayOneShot(clip);
    }

    public void PlayButton() => PlaySFX(Button);
    public void PlayAttack() => PlaySFX(Attack);
    public void PlayParing() => PlaySFX(Paring);

    public void PlayCharacterSource(AudioClip clip)
    {
        CharacterSource.PlayOneShot(clip);
    }

    public void PlayCharacterANBI() => PlayCharacterSource(CharacterANBI);
    public void PlayCharacterLONGINUS() => PlayCharacterSource(CharacterLONGINUS);
    public void PlayCharacterOokumaMari() => PlayCharacterSource(CharacterOokumaMari);


    public void SetBGMVolume(float volume)
    {
        AudioMixer.SetFloat("BGMVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        AudioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
    }

    public void SetCharacterVolume(float volume)
    {
        AudioMixer.SetFloat("CharacterVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
    }
}

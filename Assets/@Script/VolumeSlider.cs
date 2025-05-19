using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public enum VolumeType { BGM, SFX, Character }
    public VolumeType volumeType;

    public Slider slider;

    void Start()
    {
        // ���� AudioMixer�� ���� ���� �����ͼ� �����̴� �ʱ�ȭ
        float volume;
        string parameter = GetParameterName();

        if (SoundManager.Instance.AudioMixer.GetFloat(parameter, out volume))
        {
            slider.value = Mathf.Pow(10, volume / 20f); // dB -> linear
        }

        slider.onValueChanged.AddListener(OnVolumeChanged);
    }

    void OnVolumeChanged(float value)
    {
        switch (volumeType)
        {
            case VolumeType.BGM:
                SoundManager.Instance.SetBGMVolume(value);
                break;
            case VolumeType.SFX:
                SoundManager.Instance.SetSFXVolume(value);
                break;
            case VolumeType.Character:
                SoundManager.Instance.SetCharacterVolume(value);
                break;
        }
    }

    string GetParameterName()
    {
        return volumeType switch
        {
            VolumeType.BGM => "BGMVolume",
            VolumeType.SFX => "SFXVolume",
            VolumeType.Character => "CharacterVolume",
            _ => ""
        };
    }
}

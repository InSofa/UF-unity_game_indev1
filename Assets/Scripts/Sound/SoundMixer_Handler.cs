using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMixer_Handler : MonoBehaviour
{
    [SerializeField]
    private AudioMixer audioMixer;

    [SerializeField]
    private Slider masterVolumeSlider, musicVolumeSlider, sfxVolumeSlider;

    private void Start() {
        // Set the volume levels to the saved values
        SetMasterVolume(PlayerPrefs.GetFloat("masterVolume", 1f));
        SetMusicVolume(PlayerPrefs.GetFloat("musicVolume", 1f));
        SetSFXVolume(PlayerPrefs.GetFloat("sfxVolume", 1f));
    }

    public void SetMasterVolume(float level) {
        //audioMixer.SetFloat("masterVolume", level);
        audioMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20f);
        masterVolumeSlider.value = level;
        PlayerPrefs.SetFloat("masterVolume", level);
    }

    public void SetMusicVolume(float level) {
        //audioMixer.SetFloat("musicVolume", level);
        audioMixer.SetFloat("musicVolume", Mathf.Log10(level) * 20f);
        musicVolumeSlider.value = level;
        PlayerPrefs.SetFloat("musicVolume", level);
    }

    public void SetSFXVolume(float level) {
        //audioMixer.SetFloat("sfxVolume", level);
        audioMixer.SetFloat("sfxVolume", Mathf.Log10(level) * 20f);
        sfxVolumeSlider.value = level;
        PlayerPrefs.SetFloat("sfxVolume", level);
    }
}

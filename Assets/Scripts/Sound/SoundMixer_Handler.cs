using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMixer_Handler : MonoBehaviour
{
    [SerializeField]
    private AudioMixer audioMixer;

    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    private void Awake() {
        // Set the volume levels to the saved values
        float masterVolume = PlayerPrefs.GetFloat("masterVolume", 1f);
        float musicVolume = PlayerPrefs.GetFloat("musicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 1f);

        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);

        masterVolumeSlider.onValueChanged.AddListener(delegate { SetMasterVolume(); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { SetMusicVolume(); });
        sfxVolumeSlider.onValueChanged.AddListener(delegate { SetSFXVolume(); });
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

    public void SetMasterVolume() {
        //audioMixer.SetFloat("masterVolume", level);
        float level = masterVolumeSlider.value;
        audioMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("masterVolume", level);
    }

    public void SetMusicVolume() {
        //audioMixer.SetFloat("musicVolume", level);
        float level = musicVolumeSlider.value;
        audioMixer.SetFloat("musicVolume", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("musicVolume", level);
    }

    public void SetSFXVolume() {
        //audioMixer.SetFloat("sfxVolume", level);
        float level = sfxVolumeSlider.value;
        audioMixer.SetFloat("sfxVolume", Mathf.Log10(level) * 20f);
        PlayerPrefs.SetFloat("sfxVolume", level);
    }
}

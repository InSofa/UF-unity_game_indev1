using UnityEngine;
using UnityEngine.Audio;

public class SoundMixer_Handler : MonoBehaviour
{
    [SerializeField]
    private AudioMixer audioMixer;

    public void SetMasterVolume(float level) {
        //audioMixer.SetFloat("masterVolume", level);
        audioMixer.SetFloat("masterVolume", Mathf.Log10(level) * 20f);
    }

    public void SetMusicVolume(float level) {
        //audioMixer.SetFloat("musicVolume", level);
        audioMixer.SetFloat("musicVolume", Mathf.Log10(level) * 20f);
    }

    public void SetSFXVolume(float level) {
        //audioMixer.SetFloat("sfxVolume", level);
        audioMixer.SetFloat("sfxVolume", Mathf.Log10(level) * 20f);
    }
}

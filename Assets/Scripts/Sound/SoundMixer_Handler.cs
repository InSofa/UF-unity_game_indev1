using UnityEngine;
using UnityEngine.Audio;

public class SoundMixer_Handler : MonoBehaviour
{
    [SerializeField]
    private AudioMixer audioMixer;

    public void SetMasterVolume(float level) {
        audioMixer.SetFloat("masterVolume", level);
    }

    public void SetMusicVolume(float level) {
        audioMixer.SetFloat("musicVolume", level);
    }

    public void SetSFXVolume(float level) {
        audioMixer.SetFloat("sfxVolume", level);
    }
}

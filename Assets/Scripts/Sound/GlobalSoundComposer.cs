using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class GlobalSoundComposer : MonoBehaviour {
    public GameObject soundEmitterPrefab;
    
    public Dictionary<string, SoundClass> Registry = new Dictionary<string, SoundClass>();
    public Dictionary<string, GameObject> TrackList = new Dictionary<string, GameObject>();

    // Instantiate DictionarySerializers for Registry and TrackList
    [SerializeField]
    public SoundClass_DictionarySerializer registrySerializer;
    [SerializeField]
    public GameObject_DictionarySerializer trackListSerializer;


    // Singleton pattern deffinitions
    private static GlobalSoundComposer _instance;
    public static GlobalSoundComposer Instance { get { return _instance; } }
    // Singelton pattern logic
    public void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        // Link the dictionary serializers to the dictionaries
        registrySerializer.LinkDictionary(Registry);
        registrySerializer.inspectorDescription = "Global SoundFX-Registry";

        trackListSerializer.LinkDictionary(TrackList);
        trackListSerializer.inspectorDescription = "Global TrackList";
    }

    #region
    public IEnumerator FadeOutCoroutine(AudioSource audioSource, float fadeDuration) {
        float startVolume = audioSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime) {
            // Lerp the volume down based on the time elapsed
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        // Ensure the volume is set to zero at the end
        audioSource.volume = 0f;
        audioSource.Stop(); // Optionally stop the audio source
    }

    public IEnumerator FadeInCoroutine(AudioSource audioSource, float fadeDuration) {
        float startVolume = audioSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime) {
            // Lerp the volume up based on the time elapsed
            audioSource.volume = Mathf.Lerp(0f, startVolume, t / fadeDuration);
            yield return null;
        }

        // Ensure the volume is set to the original volume at the end
        audioSource.volume = startVolume;
    }
    #endregion
    public void PlayFx(string id, Vector3 pos) {
        GameObject instance = Instantiate(soundEmitterPrefab, this.gameObject.transform);
        instance.transform.position = pos;

        SoundClass soundInstance = Registry[id];
        AudioSource audioSource = instance.GetComponent<AudioSource>();
        audioSource.clip = soundInstance.audioClip;
        audioSource.outputAudioMixerGroup = soundInstance.audioMixerGroup;
        audioSource.priority = soundInstance.priority;
        audioSource.volume = soundInstance.volume;
        audioSource.pitch = soundInstance.pitch;
        audioSource.spatialBlend = soundInstance.spatialBlend;
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        // Play after intializing
        audioSource.Play();
    }

    // Overload for PlayFx, places in the middle of the scene
    public void PlayFx(string id) {
        PlayFx(id, Vector3.zero);
    }

    public void PlayTrack(string id) {
        GameObject instance = TrackList[id];
        AudioSource audioSource = instance.GetComponent<AudioSource>();
        audioSource.Play();
    }

    // Play random track of selection, if selection is empty play fully random
    public void PlayRandomTrack(List<string> ids) {
        if (ids.Count == 0) {
            List<string> keys = new List<string>(TrackList.Keys);
            PlayTrack(keys[UnityEngine.Random.Range(0, keys.Count)]);
        } else {
            PlayTrack(ids[UnityEngine.Random.Range(0, ids.Count)]);
        }
    }

    public void StopAllTracks() {
        foreach (KeyValuePair<string, GameObject> entry in TrackList) {
            GameObject instance = entry.Value;
            AudioSource audioSource = instance.GetComponent<AudioSource>();
            audioSource.Stop();
        }
    }

    public void PauseTrack(string id) {
        GameObject instance = TrackList[id];
        AudioSource audioSource = instance.GetComponent<AudioSource>();
        audioSource.Pause();
    }

    public void ResumeTrack(string id) {
        GameObject instance = TrackList[id];
        AudioSource audioSource = instance.GetComponent<AudioSource>();
        audioSource.UnPause();
    }

    public void ChangeTrackVolume(string id, float diff) {
        GameObject instance = TrackList[id];
        AudioSource audioSource = instance.GetComponent<AudioSource>();
        audioSource.volume += diff;
    }

    // Fade out track by lerping the volume to 0
    public void FadeOutTrack(string id, float seconds = 0) {
        GameObject instance = TrackList[id];
        AudioSource audioSource = instance.GetComponent<AudioSource>();
        if (seconds == 0) {
            audioSource.Stop();
        } else {
            StartCoroutine(FadeOutCoroutine(audioSource, seconds));
        }
    }

    // Fade in track by lerping the volume
    public void FadeInTrack(string id, float seconds = 0) {
        GameObject instance = TrackList[id];
        AudioSource audioSource = instance.GetComponent<AudioSource>();

        audioSource.Stop();

        float finalVolume = audioSource.volume;

        audioSource.volume = 0;

        audioSource.Play();

        if (seconds != 0) {
            StartCoroutine(FadeInCoroutine(audioSource, seconds));
        } else {
            audioSource.volume = finalVolume;
        }
    }


    // If fade is 0, we stop old and play new, else we during fade-seconds fade out old and fade in new samulatinously
    public void SwitchTrack(string old_id, string new_id, float fade = 0) {
        if (fade == 0) {
            GameObject old_instance = TrackList[old_id];
            AudioSource old_audioSource = old_instance.GetComponent<AudioSource>();

            GameObject new_instance = TrackList[new_id];
            AudioSource new_audioSource = new_instance.GetComponent<AudioSource>();

            old_audioSource.Stop();
            new_audioSource.Play();
        } else {
            FadeOutTrack(old_id, fade);
            FadeInTrack(new_id, fade);
        }
    }

    public bool TrackIsPlaying(string id) {
        GameObject instance = TrackList[id];
        AudioSource audioSource = instance.GetComponent<AudioSource>();
        return audioSource.isPlaying;
    }

    public void InitiallyUpdateSerializers() {
        trackListSerializer.InitializeDictionarySync();
        registrySerializer.InitializeDictionarySync();
    }

    public void _DebugLogTracks() {
        trackListSerializer.ApplyChanges();
        // if length is 0 log that
        if (TrackList.Count == 0) {
            Debug.Log("TrackList is empty");
        }
        int i = 0;
        foreach (KeyValuePair<string, UnityEngine.GameObject> kvp in TrackList) {
            Debug.Log($"\nTL.Pair {i}");
            Debug.Log(kvp.Key);
            Debug.Log(kvp.Value);
            i++;
        }
    }

    public void _DebugLogRegistry() {
        registrySerializer.ApplyChanges();
        // if length is 0 log that
        if (Registry.Count == 0) {
            Debug.Log("Registry is empty");
        }
        int i = 0;
        foreach (KeyValuePair<string, SoundClass> kvp in Registry) {
            Debug.Log($"\nReg.Pair {i}");
            Debug.Log(kvp.Key);
            Debug.Log(kvp.Value);
            i++;
        }
    }
}

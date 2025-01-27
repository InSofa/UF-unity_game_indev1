using System.Collections.Generic;
using UnityEngine;

public class LocalSoundComposer : MonoBehaviour
{
    private GlobalSoundComposer globalSoundComposer;

    private GameObject soundEmitterPrefab;
    private Dictionary<string, SoundClass> Registry = new Dictionary<string, SoundClass>();
    private Dictionary<string, GameObject> TrackList = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> LocalTrackList = new Dictionary<string, GameObject>();

    public void Awake() {
        globalSoundComposer = GameObject.FindGameObjectWithTag("GlobalSoundComposer").GetComponent<GlobalSoundComposer>();
        soundEmitterPrefab = globalSoundComposer.soundEmitterPrefab;
        Registry = globalSoundComposer.Registry;
        TrackList = globalSoundComposer.TrackList;
        //globalSoundComposer.InitiallyUpdateSerializers();
    }

    public void PlayFx(string id) {
        GameObject instance = Instantiate(soundEmitterPrefab, this.gameObject.transform);
        instance.transform.localPosition = Vector3.zero;

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

    public void PlayTrack(string id) {
        // Reinstantiate under this.GameObject then place in LocalTrackList

        // If exists restart the track
        if (LocalTrackList.ContainsKey(id)) {
            GameObject localInstance = LocalTrackList[id];
            AudioSource localAudioSource = localInstance.GetComponent<AudioSource>();
            localAudioSource.Stop();
            localAudioSource.Play();
            return;
        }

        GameObject instance = Instantiate(TrackList[id], this.gameObject.transform);
        AudioSource audioSource = instance.GetComponent<AudioSource>();
        audioSource.Play();
        LocalTrackList.Add(id, instance);
    }

    // Play random track of selection, if selection is empty play fully random
    public void PlayRandomTrack(List<string> ids) {
        if (ids.Count == 0) {
            List<string> keys = new List<string>(TrackList.Keys);
            PlayTrack(keys[Random.Range(0, keys.Count)]);
        } else {
            PlayTrack(ids[Random.Range(0, ids.Count)]);
        }
    }

    public void StopAllTracks() {
        foreach (KeyValuePair<string, GameObject> entry in LocalTrackList) {
            GameObject instance = entry.Value;
            AudioSource audioSource = instance.GetComponent<AudioSource>();
            audioSource.Stop();
            // Remove the entry from LocalTrackList
            LocalTrackList.Remove(entry.Key);
        }
    }

    public void PauseTrack(string id) {
        GameObject instance = LocalTrackList[id];
        AudioSource audioSource = instance.GetComponent<AudioSource>();
        audioSource.Pause();
    }

    public void ResumeTrack(string id) {
        GameObject instance = LocalTrackList[id];
        AudioSource audioSource = instance.GetComponent<AudioSource>();
        audioSource.UnPause();
    }

    public void ChangeTrackVolume(string id, float diff) {
        GameObject instance = LocalTrackList[id];
        AudioSource audioSource = instance.GetComponent<AudioSource>();
        audioSource.volume += diff;
    }

    // Fade out track by lerping the volume to 0
    public void FadeOutTrack(string id, float seconds = 0) {
        GameObject instance = LocalTrackList[id];
        AudioSource audioSource = instance.GetComponent<AudioSource>();
        if (seconds == 0) {
            audioSource.Stop();
        } else {
            StartCoroutine(globalSoundComposer.FadeOutCoroutine(audioSource, seconds));
        }
    }

    // Fade in track by lerping the volume
    public void FadeInTrack(string id, float seconds = 0) {
        GameObject instance = LocalTrackList[id];
        AudioSource audioSource = instance.GetComponent<AudioSource>();

        audioSource.Stop();

        float finalVolume = audioSource.volume;

        audioSource.volume = 0;

        audioSource.Play();

        if (seconds != 0) {
            StartCoroutine(globalSoundComposer.FadeInCoroutine(audioSource, seconds));
        } else {
            audioSource.volume = finalVolume;
        }
    }

    // If fade is 0, we stop old and play new, else we during fade-seconds fade out old and fade in new samulatinously
    public void SwitchTrack(string old_id, string new_id, float fade = 0) {
        if (fade == 0) {
            GameObject old_instance = LocalTrackList[old_id];
            AudioSource old_audioSource = old_instance.GetComponent<AudioSource>();

            GameObject new_instance = LocalTrackList[new_id];
            AudioSource new_audioSource = new_instance.GetComponent<AudioSource>();

            old_audioSource.Stop();
            new_audioSource.Play();
        } else {
            FadeOutTrack(old_id, fade);
            FadeInTrack(new_id, fade);
        }
    }

    public bool TrackIsPlaying(string id) {
        GameObject instance = LocalTrackList[id];
        AudioSource audioSource = instance.GetComponent<AudioSource>();
        return audioSource.isPlaying;
    }
}

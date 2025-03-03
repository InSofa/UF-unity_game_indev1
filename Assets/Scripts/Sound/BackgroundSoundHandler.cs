using UnityEngine;

public class BackgroundSoundHandler : MonoBehaviour
{


    //IMPORTANT: This script is a temporary solution for now, it will be replaced by a more robust system in the future


    private double musicDuration = 0;
    private double goalTime = 0;

    private bool introStarted = false;
    private bool loopStarted = false;

    [SerializeField]
    bool debug = false;

    [SerializeField]
    private string introTrack = "music:generic/BackgroundIntro", loopTrack = "music:generic/BackgroundLoop";

    GlobalSoundComposer globalSoundComposer;
    private void Start() {
        globalSoundComposer = GlobalSoundComposer.Instance;

        PlayIntro(introTrack);
    }

    private void Update() {
        if(introStarted && AudioSettings.dspTime > goalTime) {
            if (!loopStarted) {
                PlayLoop(loopTrack);
                loopStarted = true;
            }
        }
    }

    private void PlayIntro(string id) {
        AudioSource audioSource = globalSoundComposer.TrackList[id].GetComponent<AudioSource>();

        audioSource.PlayScheduled(AudioSettings.dspTime);

        AudioClip clip = audioSource.clip;
        musicDuration = (double)clip.samples / clip.frequency;
        goalTime = AudioSettings.dspTime + musicDuration;

        if (debug)
            Debug.Log($"Goal time: {goalTime}, Current time: {AudioSettings.dspTime}");

        introStarted = true;
    }

    private void PlayLoop(string id) {
        if (debug)
            Debug.Log("Playing loop");

        AudioSource audioSource = globalSoundComposer.TrackList[id].GetComponent<AudioSource>();

        audioSource.PlayScheduled(goalTime);
        audioSource.loop = true;
    }
}

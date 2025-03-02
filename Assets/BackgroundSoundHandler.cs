using UnityEngine;

public class BackgroundSoundHandler : MonoBehaviour
{


    //IMPORTANT: This script is a temporary solution for now, it will be replaced by a more robust system in the future


    GlobalSoundComposer globalSoundComposer;

    bool loopIsPlaying = false;

    private void Start() {
        globalSoundComposer = GlobalSoundComposer.Instance;
        globalSoundComposer.PlayTrack("music:generic/BackgroundIntro");
        StartCoroutine(globalSoundComposer.QueueTrack("music:generic/BackgroundIntro", "music:generic/BackgroundLoop"));
    }
}

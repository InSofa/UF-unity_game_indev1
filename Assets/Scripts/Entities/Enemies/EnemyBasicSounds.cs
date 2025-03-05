using NUnit.Framework;
using System.Collections.Generic;
using System;
using UnityEngine;

public class EnemyBasicSounds : MonoBehaviour
{
    LocalSoundComposer lsc;

    [SerializeField]
    List<String> walkSounds;

    [SerializeField]
    float walkSoundDistanceInterval;

    [Space]

    [SerializeField]
    List<String> grunts;

    [SerializeField]
    float gruntIntervalMax, gruntIntervalMin;

    Vector2 lastEffectPos;

    float time;
    float nextGruntTime;

    private void Start() {
        lsc = GetComponent<LocalSoundComposer>();

        nextGruntTime = UnityEngine.Random.Range(gruntIntervalMin, gruntIntervalMax);
    }

    private void Update() {
        if (Vector2.Distance(lastEffectPos, transform.position) >= walkSoundDistanceInterval) {
            lastEffectPos = transform.position;
            lsc.PlayRandomFx(walkSounds);
        }

        time += Time.deltaTime;
        if(time >= nextGruntTime) {
            lsc.PlayRandomFx(grunts);
            time = 0;
            nextGruntTime = UnityEngine.Random.Range(gruntIntervalMin, gruntIntervalMax);
        }
    }
}

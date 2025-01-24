using System;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SoundClass", menuName = "Scriptable Objects/SoundClass")]
public class SoundClass : ScriptableObject
{
    public AudioClip audioClip;
    public AudioMixerGroup audioMixerGroup;
    public int priority;
    public float volume;
    public float pitch;
    public float spatialBlend;
}

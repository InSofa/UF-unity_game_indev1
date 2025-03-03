using UnityEngine;

[CreateAssetMenu(fileName = "Wave", menuName = "Scriptable Objects/Wave")]
public class Wave : ScriptableObject
{
    public string waveName;
    public int waveNumber;

    public bool bossWave;

    public Pyle[] pyles;
}

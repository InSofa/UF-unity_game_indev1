using UnityEngine;

[CreateAssetMenu(fileName = "Pyle", menuName = "Scriptable Objects/Pyle")]
public class Pyle : ScriptableObject
{
    public string pyleName;
    public GameObject[] enemiesToSpawn;
}

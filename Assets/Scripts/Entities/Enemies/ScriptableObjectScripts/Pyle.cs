using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "Pyle", menuName = "Scriptable Objects/Pyle")]
public class Pyle : ScriptableObject
{
    public string pyleName;
    public EntityToSpawn[] enemiesToSpawn;

    [System.Serializable]
    public struct EntityToSpawn {
        public string entityID;
        public int amount;
    }
}

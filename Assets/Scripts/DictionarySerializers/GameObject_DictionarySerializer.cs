using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameObject_DictionaryEntry {
    public string Key;
    public GameObject Value; // Can be a ScriptableObject or GameObject
}

public class GameObject_DictionarySerializer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject_DictionaryEntry> entries = new List<GameObject_DictionaryEntry>();

    private Dictionary<string, GameObject> linkedDictionary = new Dictionary<string, GameObject>();

    /// <summary>
    /// Links a dictionary to this serializer and synchronizes entries.
    /// </summary>
    /// <param name="dictionary">The dictionary to link to this serializer.</param>
    public void LinkDictionary(Dictionary<string, GameObject> dictionary) {
        linkedDictionary = dictionary;
        SynchronizeFromDictionary();
    }

    /// <summary>
    /// Applies changes from the serialized list to the linked dictionary.
    /// </summary>
    public void ApplyChanges() {
        linkedDictionary.Clear();

        foreach (var entry in entries) {
            if (string.IsNullOrEmpty(entry.Key)) {
                Debug.LogWarning("Empty key detected. Skipping entry.");
                continue;
            }

            if (!linkedDictionary.ContainsKey(entry.Key)) {
                linkedDictionary.Add(entry.Key, entry.Value);
            } else {
                Debug.LogWarning($"Duplicate key detected: {entry.Key}. Only the first occurrence will be used.");
            }
        }
    }

    /// <summary>
    /// Synchronizes the serialized list from the linked dictionary.
    /// </summary>
    public void SynchronizeFromDictionary() {
        entries.Clear();

        foreach (var kvp in linkedDictionary) {
            entries.Add(new GameObject_DictionaryEntry { Key = kvp.Key, Value = kvp.Value });
        }
    }

    private void OnValidate() {
        // When changes are made in the Inspector, apply them to the linked dictionary
        ApplyChanges();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[Serializable]
public class String_DictionaryEntry {
    public string Key;
    public string Value;
}

public class String_DictionarySerializer : MonoBehaviour
{
    public string inspectorDescription = "String Serializer";

    [SerializeField]
    private List<String_DictionaryEntry> entries = new List<String_DictionaryEntry>();

    private Dictionary<string, string> linkedDictionary = new Dictionary<string, string>();

    /// <summary>
    /// Links a dictionary to this serializer and synchronizes entries.
    /// </summary>
    /// <param name="dictionary">The dictionary to link to this serializer.</param>
    public void LinkDictionary(Dictionary<string, string> dictionary) {
        linkedDictionary = dictionary;
        InitializeDictionarySync();
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
            entries.Add(new String_DictionaryEntry { Key = kvp.Key, Value = kvp.Value });
        }
    }

    /// <summary>
    /// Synchronizes the serialized list from the linked dictionary.
    /// But keeps any pre-existing entries inside the serializer, it does this by saving the currnent entries in the seralizer,
    /// then syncing using `SynchronizeFromDictionary` then finally re-merges the saved entries, beforing removing the save.
    /// </summary>
    public void InitializeDictionarySync() {
        // Make sure we have the same entries in the linkedDictionary as in the serializer `SynchronizeFromDictionary()` is wrong
        // as it will remove any entries that are not in the linkedDictionary
        List<String_DictionaryEntry> savedEntries = new List<String_DictionaryEntry>(entries);

        SynchronizeFromDictionary();

        foreach (var entry in savedEntries) {
            if (!entries.Contains(entry)) {
                entries.Add(entry);
            }
        }

        ApplyChanges();
    }

    private void OnValidate() {
        // When changes are made in the Inspector, apply them to the linked dictionary
        ApplyChanges();
    }
}

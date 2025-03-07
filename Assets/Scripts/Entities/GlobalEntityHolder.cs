using System.Collections.Generic;
using UnityEngine;

public class GlobalEntityHolder : MonoBehaviour {
    public Dictionary<string, GameObject> EntityList = new Dictionary<string, GameObject>();
    [SerializeField]
    public GameObject_DictionarySerializer entitySerializer;

    // Singleton pattern deffinitions
    private static GlobalEntityHolder _instance;
    public static GlobalEntityHolder Instance { get { return _instance; } }
    // Singelton pattern logic
    public void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        entitySerializer.LinkDictionary(EntityList);
    }

    public GameObject Resolve(string entity_id) {
        if (EntityList.ContainsKey(entity_id)) {
            return EntityList[entity_id];
        } else {
            Debug.LogWarning($"Entity with ID {entity_id} not found in EntityList.");
            return null;
        }
    }
}

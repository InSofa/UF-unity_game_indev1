using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

public class UIControls_Handler : MonoBehaviour {

    public Dictionary<string, string> PC_UI_Controls_IndexMap = new Dictionary<string, string>();
    public Dictionary<string, string> PS4_UI_Controls_IndexMap = new Dictionary<string, string>();
    public Dictionary<string, string> PS5_UI_Controls_IndexMap = new Dictionary<string, string>();
    public Dictionary<string, string> XBOX_UI_Controls_IndexMap = new Dictionary<string, string>();

    // Reference the atlas for each platform
    [SerializeField]
    public TMP_SpriteAsset pcUIControlsAtlas;
    [SerializeField]
    public TMP_SpriteAsset ps4UIControlsAtlas;
    [SerializeField]
    public TMP_SpriteAsset ps5UIControlsAtlas;
    [SerializeField]
    public TMP_SpriteAsset xboxUIControlsAtlas;

    // Reference DictionarySerializers for icon-indexes per platform
    [SerializeField]
    public String_DictionarySerializer pcUIControlsIndexSerializer;
    [SerializeField]
    public String_DictionarySerializer ps4UIControlsIndexSerializer;
    [SerializeField]
    public String_DictionarySerializer ps5UIControlsIndexSerializer;
    [SerializeField]
    public String_DictionarySerializer xboxUIControlsIndexSerializer;

    // Singleton pattern deffinitions
    private static UIControls_Handler _instance;
    public static UIControls_Handler Instance { get { return _instance; } }
    // Singelton pattern logic
    public void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

        // Link the dictionary serializers to the dictionaries
        pcUIControlsIndexSerializer.LinkDictionary(PC_UI_Controls_IndexMap);
        ps4UIControlsIndexSerializer.LinkDictionary(PS4_UI_Controls_IndexMap);
        ps5UIControlsIndexSerializer.LinkDictionary(PS5_UI_Controls_IndexMap);
        xboxUIControlsIndexSerializer.LinkDictionary(XBOX_UI_Controls_IndexMap);
    }
}

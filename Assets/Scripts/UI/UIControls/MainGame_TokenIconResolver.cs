using System.Text.RegularExpressions;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

public class MainGame_TokenIconResolver : MonoBehaviour {
    // The UIControls_Handler prefab with the icon registry for each platform
    [SerializeField]
    private UIControls_Handler globalUIControls;

    // The text to tokenize and display
    [SerializeField]
    private string textline_ui_pc;
    [SerializeField]
    private string textline_ui_rest;
    [SerializeField]
    private string textline_general_pc;
    [SerializeField]
    private string textline_general_rest;
    [SerializeField]
    private string textline_melee;
    [SerializeField]
    private string textline_build_pc;
    [SerializeField]
    private string textline_build_rest;

    // Current state
    private TMP_Text tmpText;
    private string currentPlatform;
    private int currentMenu;
    private bool isMeleeMode;

    // Start
    void Start() {
        tmpText = GetComponent<TMP_Text>();
        if (PlatformInputHandler.Instance && PlatformInputHandler.Instance.currentPlatform != null) {
            currentPlatform = PlatformInputHandler.Instance.currentPlatform;
        }
        UpdateText();
    }

    // Update is called once per frame
    void Update() {
        if (PlatformInputHandler.Instance && PlatformInputHandler.Instance.currentPlatform != null && currentPlatform != null) {
            if (PlatformInputHandler.Instance.currentPlatform != currentPlatform) {
                currentPlatform = PlatformInputHandler.Instance.currentPlatform;
                UpdateText();
            }
        }
    }

    public void UpdateCurrentMenu(int _currentMenu) {
        currentMenu = _currentMenu;
        UpdateText();
    }

    public void UpdateMeleeMode(bool _isMeleeMode) {
        isMeleeMode = _isMeleeMode;
        UpdateText();
    }

    void UpdateText() {

        if (tmpText == null) {
            return;
        }

        // Start with the base text
        StringBuilder sb = new StringBuilder("");

        // Get the general or ui text
        if (currentMenu == 0) {
            if (currentPlatform == "pc") {
                // General PC
                sb.Append(textline_general_pc+"\n");
                // Melee/Build PC
                if (isMeleeMode) {
                    sb.Append(textline_melee);
                } else {
                    sb.Append(textline_build_pc);
                }
            } else {
                // General Rest
                sb.Append(textline_general_rest+"\n");
                // Melee/Build Rest
                if (isMeleeMode) {
                    sb.Append(textline_melee);
                } else {
                    sb.Append(textline_build_rest);
                }
            }
        } else {
            if (currentPlatform == "pc") {
                // UI PC
                sb.Append(textline_ui_pc);
            } else {
                // UI Rest
                sb.Append(textline_ui_rest);
            }
        }
        sb.Append("\n");

        string textToDisplay = sb.ToString();

        // If we have a valid controls dictionary, replace tokens with proper sprite tags
        string pattern = @"<[^>]+>"; // Regex pattern to find tokens
        MatchCollection matches = Regex.Matches(textToDisplay, pattern);

        // Foreach match, check if the token is in the currentTokenIndexmap, to get it's index, then get the name for the currentIconAtlas which we use as sprite.
        if (globalUIControls && globalUIControls.currentTokenIndexmap != null) {
            foreach (Match match in matches) {
                string token = match.Value;
                string tokenName = token.Trim('<', '>');
                if (globalUIControls.currentTokenIndexmap.ContainsKey(token)) {
                    string spriteId = globalUIControls.currentTokenIndexmap[token];
                    string imageTag = $"<sprite=\"{globalUIControls.currentIconAtlas.name}\" index={spriteId}>";
                    sb.Replace(token, imageTag);
                }
            }
        }

        textToDisplay = sb.ToString();

        tmpText.text = textToDisplay;
        tmpText.ForceMeshUpdate();

        // Adjust inline sprite vertices
        TMP_TextInfo textInfo = tmpText.textInfo;
        for (int i = 0; i < textInfo.characterCount; i++) {
            if (textInfo.characterInfo[i].elementType == TMP_TextElementType.Sprite) {
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                // Calculate width & height
                float width = Mathf.Abs(vertices[vertexIndex + 2].x - vertices[vertexIndex].x);
                float height = Mathf.Abs(vertices[vertexIndex + 2].y - vertices[vertexIndex].y);

                // Shift each vertex
                Vector3 offset = new Vector3(width/2, height/2, 0);

                vertices[vertexIndex] += offset;
                vertices[vertexIndex + 1] += offset;
                vertices[vertexIndex + 2] += offset;
                vertices[vertexIndex + 3] += offset;
            }
        }
        tmpText.UpdateVertexData();
    }

#if UNITY_EDITOR
    /// <summary>
    /// Ensures text updates when values change in the Unity editor.
    /// Only runs in the editor to avoid unnecessary updates during gameplay.
    /// </summary>
    void OnValidate() {
        if (!Application.isPlaying) return;
        UpdateText();
    }
#endif
}

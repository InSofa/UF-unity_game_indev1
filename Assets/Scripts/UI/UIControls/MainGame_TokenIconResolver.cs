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
    [SerializeField]
    private UIHandler globalUIHandler;
    [SerializeField]
    private PlayerHand globalPlayerHand;

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

    // Start
    void Start() {
        tmpText = GetComponent<TMP_Text>();
        currentPlatform = globalUIControls.currentPlatform;
        UpdateText();
    }

    // Update is called once per frame
    void Update() {
        if (globalUIControls.currentPlatform != currentPlatform) {
            currentPlatform = globalUIControls.currentPlatform;
            UpdateText();
        }
    }

    void UpdateText() {

        // Start with the base text
        StringBuilder sb = new StringBuilder("");

        // Get the general or ui text
        if (globalUIHandler.currentMenu == 0) {
            if (currentPlatform == "pc") {
                // General PC
                sb.Append(textline_general_pc+"\n");
                // Melee/Build PC
                if (globalPlayerHand.isMeleeMode) {
                    sb.Append(textline_melee);
                } else {
                    sb.Append(textline_build_pc);
                }
            } else {
                // General Rest
                sb.Append(textline_general_rest+"\n");
                // Melee/Build Rest
                if (globalPlayerHand.isMeleeMode) {
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

        Debug.Log(textToDisplay);

        // If we have a valid controls dictionary, replace tokens with proper sprite tags
        string pattern = @"<[^>]+>"; // Regex pattern to find tokens
        MatchCollection matches = Regex.Matches(textToDisplay, pattern);

        // Foreach match, check if the token is in the currentTokenIndexmap, to get it's index, then get the name for the currentIconAtlas which we use as sprite.
        foreach (Match match in matches) {
            string token = match.Value;
            string tokenName = token.Trim('<', '>');
            if (globalUIControls.currentTokenIndexmap.ContainsKey(token)) {
                string spriteId = globalUIControls.currentTokenIndexmap[token];
                string imageTag = $"<sprite=\"{globalUIControls.currentIconAtlas.name}\" index={spriteId}>";
                sb.Replace(token, imageTag);
            }
        }

        textToDisplay = sb.ToString();

        tmpText.text = textToDisplay;
        tmpText.ForceMeshUpdate();
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

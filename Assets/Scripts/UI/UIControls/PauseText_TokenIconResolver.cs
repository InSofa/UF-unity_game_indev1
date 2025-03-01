using System.Text.RegularExpressions;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

public class PauseText_TokenIconResolver : MonoBehaviour {
    // The UIControls_Handler prefab with the icon registry for each platform
    [SerializeField]
    private GameObject GlobalUIControls;
    private UIControls_Handler globalUIControls;

    // The text to tokenize and display
    [SerializeField]
    private string textLine;

    // Current state
    private TMP_Text tmpText;
    private string currentPlatform;

    // Start
    void Start() {
        tmpText = GetComponent<TMP_Text>();
        globalUIControls = GlobalUIControls.GetComponent<UIControls_Handler>();
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
        StringBuilder sb = new StringBuilder(textLine + "\n");

        string textToDisplay = sb.ToString();

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

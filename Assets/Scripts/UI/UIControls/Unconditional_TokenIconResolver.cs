using System.Text.RegularExpressions;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

public class Unconditional_TokenIconResolver : MonoBehaviour {
    // The UIControls_Handler prefab with the icon registry for each platform
    [SerializeField]
    private UIControls_Handler globalUIControls;

    // The text to tokenize and display
    [SerializeField]
    private string textLine;

    // Ant other options
    [SerializeField]
    private List<string> skipPlatforms;

    // Current state
    private TMP_Text tmpText;
    private string currentPlatform;

    // Start
    void Start() {
        tmpText = GetComponent<TMP_Text>();
        currentPlatform = InputHandler.Instance.currentPlatform;
        UpdateText();
    }

    // Update is called once per frame
    void Update() {
        if (InputHandler.Instance.currentPlatform != currentPlatform) {
            currentPlatform = InputHandler.Instance.currentPlatform;
            UpdateText();
        }
    }

    void UpdateText() {

        if (skipPlatforms.Contains(currentPlatform)) {
            tmpText.text = "";
            tmpText.ForceMeshUpdate();
            return;
        }

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
                Vector3 offset = new Vector3(width / 2, height / 2, 0);

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

using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.Audio;



#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Xml;

public class DebugConsole : MonoBehaviour {
    public TMP_InputField inputField;
    public TMP_Text outputText;
    public ScrollRect scrollRect;

    public bool inputIsFocused = false;

    [SerializeField]
    private UIHandler uiHandler;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private EnemySpawner enemySpawner;
    [SerializeField]
    private SoundMixer_Handler soundMixerHandler;

    // Singleton pattern deffinitions
    private static DebugConsole _instance;
    public static DebugConsole Instance { get { return _instance; } }
    // Singelton pattern logic
    public void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    void Start() {
        // Ensure the input field is assigned
        if (inputField != null) {
            inputField.onSubmit.AddListener(HandleInput);
        }
    }
    private enum SpiralDirection {
        Up,
        Right,
        Down,
        Left
    }

    private List<(int x, int y)> GetSpiralNodes(
        int startX,
        int startY,
        int amount,
        bool skipBuildings = false
    ) {
        List<(int x, int y)> spiralNodes = new List<(int x, int y)>();
        int gridWidth = PathfindingGrid.instance.grid.GetLength(0);
        int gridHeight = PathfindingGrid.instance.grid.GetLength(1);

        int placedCount = 0;
        int xOffset = 0;
        int yOffset = 1;
        int moveCount = 1;
        int moveIncrement = 1;
        SpiralDirection currentDirection = SpiralDirection.Right;

        while (placedCount < amount) {
            for (int i = 0; i < moveCount; i++) {
                int x = startX + xOffset;
                int y = startY + yOffset;

                if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight) {
                    Node currentNode = PathfindingGrid.instance.grid[x, y];
                    if (!currentNode.isBed && (!skipBuildings || currentNode.building == null)) {
                        spiralNodes.Add((x, y));
                        placedCount++;
                        if (placedCount >= amount) break;
                    }
                }

                // Move to the next position in the current direction
                switch (currentDirection) {
                    case SpiralDirection.Right:
                        xOffset++;
                        break;
                    case SpiralDirection.Down:
                        yOffset--;
                        break;
                    case SpiralDirection.Left:
                        xOffset--;
                        break;
                    case SpiralDirection.Up:
                        yOffset++;
                        break;
                }
            }

            if (currentDirection == SpiralDirection.Left ||
                currentDirection == SpiralDirection.Right) {
                moveIncrement++;
            }
            moveCount = moveIncrement;

            // Change direction
            currentDirection = (SpiralDirection)(((int)currentDirection + 1) % 4);
        }

        return spiralNodes;
    }

    public void HandleInput(string input) {
        if (string.IsNullOrWhiteSpace(input)) return;

        // Append input to the output text
        AppendToConsole("> " + input);

        // Execute the input, then if the input is "https://example.com", make it clickable
        string output = ConvertUrlsToClickable(ExecuteInput(input));

        AppendToConsole(output);

        // Clear input field
        inputField.text = "";
        inputField.ActivateInputField();
    }

    void AppendToConsole(string message) {
        outputText.text += message + "\n";
        Canvas.ForceUpdateCanvases();
        if (scrollRect != null) {
            //scrollRect.scrollOffset = scrollRect.contentContainer.layout.max - scrollRect.contentViewport.layout.size;
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }

    string ConvertUrlsToClickable(string text) {
        string pattern = @"(http[s]?://\S+)";
        return Regex.Replace(text, pattern, "<link=\"$1\"><color=blue><u>$1</u></color></link>");
    }

    public void OnFocused() {
        inputIsFocused = true;
    }

    public void OnFocusLost() {
        inputIsFocused = false;
    }

    private static List<string> SplitBySpaceExcludingQuotes(string input) {
        var result = new List<string>();
        int startIndex = 0;
        bool inDoubleQuote = false;
        bool inSingleQuote = false;

        for (int i = 0; i < input.Length; i++) {
            char currentChar = input[i];

            // Check for the starting or closing of double quotes
            if (currentChar == '\"' && !inSingleQuote) {
                inDoubleQuote = !inDoubleQuote;
            }
            // Check for the starting or closing of single quotes
            else if (currentChar == '\'' && !inDoubleQuote) {
                inSingleQuote = !inSingleQuote;
            }

            // If we find a space and we are not inside any quotes, we split
            else if (currentChar == ' ' && !inDoubleQuote && !inSingleQuote) {
                result.Add(input.Substring(startIndex, i - startIndex));
                startIndex = i + 1; // Move start index past the space
            }
        }

        // Add the last part after the final space
        result.Add(input.Substring(startIndex));

        return result;
    }

    private static List<string> SplitBySemicolonExcludingSpace(string input) {
        List<string> result = new List<string>();
        bool inSingleQuote = false;
        bool inDoubleQuote = false;
        int startIndex = 0;

        for (int i = 0; i < input.Length; i++) {
            char currentChar = input[i];

            if (currentChar == '\'' && !inDoubleQuote) // Toggle single quote flag when not in double quotes
            {
                inSingleQuote = !inSingleQuote;
            } else if (currentChar == '\"' && !inSingleQuote) // Toggle double quote flag when not in single quotes
              {
                inDoubleQuote = !inDoubleQuote;
            } else if (currentChar == ';' && !inSingleQuote && !inDoubleQuote) // Split when outside quotes
              {
                result.Add(input.Substring(startIndex, i - startIndex));
                startIndex = i + 1;
            }
        }

        // Add the last segment if there is any
        if (startIndex < input.Length) {
            result.Add(input.Substring(startIndex));
        }

        return result;
    }

    public string ExecuteInput(string rawInput) {

        List<string> inputs = SplitBySemicolonExcludingSpace(rawInput);

        string output = "";

        foreach (string preCleanedInputs in inputs) {

            // Remove any trailing or leading semicolons from input
            string input = preCleanedInputs.Trim(';');
            input = input.Trim();
            // If input is only whitespace, skip
            if (string.IsNullOrWhiteSpace(input)) continue;

            Debug.Log("Executing: " + input);

            //List<string> parts = SplitBySpaceExcludingQuotes(input);
            List<string> parts = new List<string>(input.Split(' '));
            string command = parts[0];
            // args are all parts except the first one
            string[] args = parts.GetRange(1, parts.Count - 1).ToArray();
            string sargs = string.Join(" ", args);

            try {

                switch (command) {
                    case "clear":
                        outputText.text = "";
                        output = "";
                        break;

                    case "echo":
                        output += "\n" + sargs;
                        break;

                    case "health":
                        // Set health to Parseint of args[0]
                        if (args.Length < 1) {
                            output += "\n" + $"Health: {player.GetComponent<PlayerHealth>().GetHealth()}";
                        } else {
                            player.GetComponent<PlayerHealth>().SetHealth(int.Parse(args[0]));
                        }
                        break;

                    case "startWave":
                        // If no args start next wave, else start wave with args[0]'
                        if (args.Length < 1) {
                            enemySpawner.SpawnWave();
                        } else {
                            int waveIndexToStart = int.Parse(args[0]) - 1;
                            if (waveIndexToStart > 0) {
                                enemySpawner.currentWave = waveIndexToStart;
                                enemySpawner.SpawnWave();
                            } else {
                                output += "\n" + "<color=red>Wave index to low!</color>";
                            }
                        }
                        break;

                    case "canDie":
                        if (args.Length > 0) {
                            // args[0] is either the string "true" or "false"
                            player.GetComponent<PlayerHealth>().canDie = bool.Parse(args[0]);
                        } else {
                            output += "\n" + $"Can die: {player.GetComponent<PlayerHealth>().canDie}";
                        }
                        break;

                    case "pillows":
                        // Set health to Parseint of args[0]
                        if (args.Length < 1) {
                            output += "\n" + $"Pillows: {player.GetComponent<PlayerHand>().getPillows()}";
                        } else {
                            player.GetComponent<PlayerHand>().setPillows(int.Parse(args[0]));
                        }
                        break;

                    case "player.playsfx":
                        player.GetComponent<LocalSoundComposer>().PlayFx(sargs);
                        break;

                    case "player.playtrack":
                        player.GetComponent<LocalSoundComposer>().PlayTrack(sargs);
                        break;

                    case "playsfx":
                        GlobalSoundComposer.Instance.PlayFx(sargs);
                        break;

                    case "playtrack":
                        GlobalSoundComposer.Instance.PlayTrack(sargs);
                        break;

                    case "build":
                        switch (args[0]) {
                            case "spiral":
                                if (args.Length < 3) {
                                    output += "\n" + "Usage: build spiral <selection> <amount> [<skip-buildings=false>]";
                                    break;
                                }

                                int spiral_selection = int.Parse(args[1]);
                                int spiral_amount = int.Parse(args[2]);
                                bool skipBuildings = args.Length > 3 ? bool.Parse(args[3]) : false;
                                Node spiral_playerNode = PathfindingGrid.instance.NodeFromWorldPoint(
                                    player.transform.position
                                );
                                int spiral_x = spiral_playerNode.gridX;
                                int spiral_y = spiral_playerNode.gridY;

                                List<(int x, int y)> spiralNodes = GetSpiralNodes(
                                    spiral_x,
                                    spiral_y,
                                    spiral_amount,
                                    skipBuildings
                                );

                                foreach (var node in spiralNodes) {
                                    player.GetComponent<PlayerHand>().SwitchBuildSelection(spiral_selection);
                                    player
                                        .GetComponent<PlayerHand>()
                                        .ForcePlaceBuildingAt(node.x, node.y);
                                }

                                break;

                            case "place":
                                if (args.Length < 4) output += "\n" + "Usage: build <place/remove/get> <x> <y> <selection>";
                                int x = int.Parse(args[1]);
                                int y = int.Parse(args[2]);
                                int buildingIndex = int.Parse(args[3]);
                                player.GetComponent<PlayerHand>().SwitchBuildSelection(buildingIndex);
                                player.GetComponent<PlayerHand>().ForcePlaceBuildingAt(x, y);
                                break;
                            case "remove":
                                if (args.Length < 3) output += "\n" + "Usage: build remove <x> <y>";
                                x = int.Parse(args[1]);
                                y = int.Parse(args[2]);
                                PathfindingGrid.instance.ForceRemoveBuildingAtNode(x, y);
                                break;
                            case "get":
                                if (args.Length < 3) output += "\n" + "Usage: build get <x> <y>";
                                x = int.Parse(args[1]);
                                y = int.Parse(args[2]);
                                GameObject building = PathfindingGrid.instance.GetBuildingAtNode(x, y);
                                output += "\n" + (building == null ? "No building at node" : building.name);
                                break;
                            case "fill":
                                if (args.Length < 2) output += "\n" + "Usage: build fill <selection> [<overriding>]";
                                buildingIndex = int.Parse(args[1]);
                                bool overriding = args.Length > 2 && bool.Parse(args[2]);
                                player.GetComponent<PlayerHand>().SwitchBuildSelection(buildingIndex);
                                PathfindingGrid.instance.DEBUG_FillGridWith(player.GetComponent<PlayerHand>().GetSelectedBuilding().buildingPrefab, overriding);
                                break;
                            default:
                                output += "\n" + "Unknown subcommand: " + args[0];
                                break;
                        }
                        break;

                    case "getPlayerNode":
                        Node playerNode = PathfindingGrid.instance.NodeFromWorldPoint(player.transform.position);
                        output += "\n" + $"Player node: {playerNode.gridX}, {playerNode.gridY}";
                        break;

                    case "volume":
                        // "volume master/sfx/music <volume>"
                        // "volume"
                        // soundMixerHandler.SetMasterVolume, soundMixerHandler.SetMusicVolume, soundMixerHandler.SetSFXVolume
                        // PlayerPrefs.GetFloat("masterVolume", 1f), PlayerPrefs.GetFloat("musicVolume", 1f), PlayerPrefs.GetFloat("sfxVolume", 1f);
                        if (args.Length == 0) {
                            output += "\n" + $"Master volume: {PlayerPrefs.GetFloat("masterVolume", 1f)}";
                            output += "\n" + $"Music volume: {PlayerPrefs.GetFloat("musicVolume", 1f)}";
                            output += "\n" + $"SFX volume: {PlayerPrefs.GetFloat("sfxVolume", 1f)}";
                        } else if (args.Length == 2) {
                            float volume = float.Parse(args[1]);
                            if (args[0] == "master") {
                                soundMixerHandler.SetMasterVolume(volume);
                            } else if (args[0] == "music") {
                                soundMixerHandler.SetMusicVolume(volume);
                            } else if (args[0] == "sfx") {
                                soundMixerHandler.SetSFXVolume(volume);
                            } else {
                                output += "\n" + "Usage: volume master/sfx/music <volume>" + "\n" + "volume";
                            }
                        } else {
                            output += "\n" + "Usage: volume master/sfx/music <volume>" + "\n" + "volume";
                        }
                        break;

                    case "summon":
                        // "summon <entity_id> <x> <y> [<u-nor>]"
                        if (args.Length < 2) output += "\n" + "Usage: summon <entity_id> <x> <y> [<u-nor>]" + "\nsummon fill <entity_id> [<u-nor>]" + "\nsummon spiral <entity_id> <amount> [<u-nor>]";
                        if (args[0] == "fill") {
                            string entityId = args[1];
                            string uNor = "";
                            if (args.Length > 2) {
                                // Unor is space joined of al others args after the first 2
                                uNor = string.Join(" ", args, 2, args.Length - 2);
                            }
                            // Get entity from entityId
                            GameObject entityPrefab = GlobalEntityHolder.Instance.Resolve(entityId);
                            // For each node in the grid that is not bed or player we instantiate the entity
                            for (int x = 0; x < PathfindingGrid.instance.grid.GetLength(0); x++) {
                                for (int y = 0; y < PathfindingGrid.instance.grid.GetLength(1); y++) {
                                    Node node = PathfindingGrid.instance.grid[x, y];
                                    if (node.isBed || node.building != null) continue;
                                    GameObject entity = Instantiate(entityPrefab, node.worldPosition, Quaternion.identity);
                                    // If we got U-NOR data we use DebugConsole_UNOR_Properties.ParseAndApply
                                    if (args.Length > 2) {
                                        DebugConsole_UNOR_Properties.ParseAndApply(entity, uNor);
                                    }
                                }
                            }
                        } else if (args[0] == "spiral") {
                            if (args.Length < 3) output += "\n" + "Usage: summon spiral <entity_id> <amount> [<u-nor>]";
                            string entityId = args[1];
                            int spiral_amount = int.Parse(args[2]);
                            string uNor = "";
                            if (args.Length > 3) {
                                // Unor is space joined of al others args after the first 3
                                uNor = string.Join(" ", args, 3, args.Length - 3);
                            }
                            // Get entity from entityId
                            GameObject entityPrefab = GlobalEntityHolder.Instance.Resolve(entityId);
                            // Get player node
                            Node spiral_summon_playerNode = PathfindingGrid.instance.NodeFromWorldPoint(player.transform.position);
                            // Get spiral nodes
                            List<(int x, int y)> spiralNodes = GetSpiralNodes(
                                spiral_summon_playerNode.gridX,
                                spiral_summon_playerNode.gridY,
                                spiral_amount
                            );
                            foreach (var node in spiralNodes) {
                                GameObject entity = Instantiate(entityPrefab, PathfindingGrid.instance.grid[node.x, node.y].worldPosition, Quaternion.identity);
                                // If we got U-NOR data we use DebugConsole_UNOR_Properties.ParseAndApply
                                if (args.Length > 3) {
                                    DebugConsole_UNOR_Properties.ParseAndApply(entity, uNor);
                                }
                            }
                        } else {
                            if (args.Length < 3) output += "\n" + "Usage: summon <entity_id> <x> <y> [<u-nor>]" + "\n" + "summon fill <entity_id> [<u-nor>]";

                            string entityId = args[0];
                            int summon_x = int.Parse(args[1]);
                            int summon_y = int.Parse(args[2]);
                            string uNor = "";
                            if (args.Length > 3) {
                                // Unor is space joined of al others args after the first 3
                                uNor = string.Join(" ", args, 3, args.Length - 3);
                            }
                            // Get entity from enemyId
                            GameObject entityPrefab = GlobalEntityHolder.Instance.Resolve(entityId);
                            // Instantiate enemy at x, y
                            GameObject entity = Instantiate(entityPrefab, PathfindingGrid.instance.grid[summon_x, summon_y].worldPosition, Quaternion.identity);
                            // If we got U-NOR data we use DebugConsole_UNOR_Properties.ParseAndApply
                            if (args.Length > 3) {
                                DebugConsole_UNOR_Properties.ParseAndApply(entity, uNor);
                            }
                        }
                        break;

                    case "object":
                        // Modify existing object, first use U-NOR to get object, then apply U-NOR
                        // "object <u-nor-select> <u-nor-data>"
                        if (args.Length < 2) output += "\n" + "Usage: object <u-nor-select> <u-nor-data>";
                        List<string> object_parts = SplitBySpaceExcludingQuotes(sargs);
                        if (object_parts.Count < 2) output += "\n" + "Usage: object <u-nor-select> <u-nor-data>";
                        string uNorSelect = object_parts[0];
                        string uNorData = object_parts[1];
                        DebugConsole_UNOR_Properties.UNOR_ObjectReference selection = new DebugConsole_UNOR_Properties.UNOR_ObjectReference(uNorSelect, "component");
#nullable enable
                        object? selectionResult = selection.Resolve();
                        if (selectionResult == null) output += "\n" + "<color=red>Object not found</color>";
#nullable disable
                        GameObject gameObject = selectionResult as GameObject;
                        DebugConsole_UNOR_Properties.ParseAndApply(gameObject, uNorData);
                        break;

                    case "inflation":
                        // "inflation <buy/sell/pickup> <amount>" to set inflation for either buy or sell
                        // "inflation <amount>" to set inflation for both buy and sell
                        // "inflation" to get current inflation for both buy and sell
                        if (args.Length == 0) {
                            output += "\n" + $"Buy inflation: {player.GetComponent<PlayerHand>().GlobalBuyInflationMultiplier}";
                            output += "\n" + $"Sell inflation: {player.GetComponent<PlayerHand>().GlobalSellInflationMultiplier}";
                            output += "\n" + $"Pickup inflation: {player.GetComponent<PlayerHand>().GlobalPickupInflationMultiplier}";
                        } else if (args.Length == 1) {
                            int inflation = int.Parse(args[0]);
                            player.GetComponent<PlayerHand>().GlobalBuyInflationMultiplier = inflation;
                            player.GetComponent<PlayerHand>().GlobalSellInflationMultiplier = inflation;
                            player.GetComponent<PlayerHand>().GlobalPickupInflationMultiplier = inflation;
                        } else if (args.Length == 2) {
                            int inflation = int.Parse(args[1]);
                            if (args[0] == "buy") {
                                player.GetComponent<PlayerHand>().GlobalBuyInflationMultiplier = inflation;
                            } else if (args[0] == "sell") {
                                player.GetComponent<PlayerHand>().GlobalSellInflationMultiplier = inflation;
                            } else if (args[0] == "pickup") {
                                player.GetComponent<PlayerHand>().GlobalPickupInflationMultiplier = inflation;
                            } else {
                                output += "\n" + "Usage: inflation <buy/sell/pickup> <amount>" + "\n" + "inflation <amount>";
                            }
                        } else {
                            output += "\n" + "Usage: inflation <buy/sell/pickup> <amount>" + "\n" + "inflation <amount>";
                        }
                        break;

                    case "quit":
#if UNITY_EDITOR
                        EditorApplication.isPlaying = false; // Stop play mode in Unity Editor
#else
                        Application.Quit(); // Quit standalone build
#endif
                        break;

#if UNITY_EDITOR
                    case "log":
                        Debug.Log(sargs);
                        break;

                    case "error":
                        Debug.LogError(sargs);
                        break;

                    case "warn":
                        Debug.LogWarning(sargs);
                        break;
#endif

                    case "mainmenu":
                        // Load main menu scene
                        uiHandler.loadScene(0);
                        break;

                    case "help":
                        // Returned strings with colors
                        output += "\n" +
                            "<color=yellow>clear</color> - Clear the console\n" +
                            "<color=yellow>echo</color> - Echo the input\n" +
                            "<color=yellow>health</color> - Get or set player health\n" +
                            "<color=yellow>startWave</color> - Start next wave or specific wave\n" +
                            "<color=yellow>canDie</color> - Get or set if player can die\n" +
                            "<color=yellow>pillows</color> - Get or set player pillows\n" +
                            "<color=yellow>player.playsfx</color> - Play sound effect on player\n" +
                            "<color=yellow>player.playtrack</color> - Play track on player\n" +
                            "<color=yellow>playsfx</color> - Play sound effect globally\n" +
                            "<color=yellow>playtrack</color> - Play track globally\n" +
                            "<color=yellow>build</color> - Build commands\n" +
                            "<color=yellow>getPlayerNode</color> - Get player node\n" +
                            "<color=yellow>summon</color> - Summon enemy\n" +
                            "<color=yellow>object</color> - Modify existing object\n" +
                            "<color=yellow>quit</color> - Quit the game\n" +
                            "<color=yellow>mainmenu</color> - Load main menu scene\n" +
                            "<color=yellow>inflation</color> - Set inflation multiplier\n" +
#if UNITY_EDITOR
                            "<color=yellow>log</color> - Log a message\n" +
                            "<color=yellow>error</color> - Log an error\n" +
                            "<color=yellow>warn</color> - Log a warning\n" +
#endif
                            "<color=yellow>help</color> - Show this help message";
                        break;

                    default:
                        output += "\n" + "<color=red>Unknown command: " + command + "</color>";
                        break;
                }
            }
            catch (System.Exception ex) {
                output += "\n" + $"<color=red>{ex.Message}</color>";
            }
        }
        // Remove leading newlines
        output = output.TrimStart('\n');
        return output;
    }
}
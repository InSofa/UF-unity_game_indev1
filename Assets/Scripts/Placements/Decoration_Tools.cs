using System.Collections.Generic;
using UnityEngine;
using static Decoration_Tools;

public class Decoration_Tools : MonoBehaviour {
    // General Usage Parameters (things both random and perlin have in common ex nonosquares)
    [SerializeField]
    private Bounds2D GrassOuterBounds;
    [SerializeField]
    private List<Bounds2D> GrassNoNoSquares;
    [SerializeField]
    private bool GrassUsePerlin = true;
    [SerializeField]
    private bool GrassDebugLog = false;

    // Random Usage Parameters
    [SerializeField]
    private int GrassRMaxValidations = 100;
    [SerializeField]
    private int GrassRFreq = 1750;

    // Perlin Usage Parameters
    [SerializeField]
    private Texture2D GrassPUseImage; // If not defined we generate
    [SerializeField]
    private int GrassPFreq = 15;
    [SerializeField]
    private float GrassPNoiseThreshold = 0.3f;

    // Perlin Generation Parameters
    [SerializeField]
    private int GrassPgenSeed = -1; // Negative gives random seed
    [SerializeField]
    private float GrassPgenScale = 10;

    [Space]
    [SerializeField]
    private List<DecorationPrefab> grassPrefabs = new List<DecorationPrefab>();

    [SerializeField]
    private List<ColorOption> colors = new List<ColorOption>();


    private List<Vector2> grassPoints = new List<Vector2>();


    [System.Serializable]
    public struct DecorationPrefab {
        public GameObject prefab;
        public float weight;
        public bool changeColor;

        public DecorationPrefab(GameObject prefab, float weight, bool changeColor) {
            this.prefab = prefab;
            this.weight = weight;
            this.changeColor = changeColor;
        }
    }

    [System.Serializable]
    public struct ColorOption {
        public Color color;
        public float weight;

        public ColorOption(Color color, float weight) {
            this.color = color;
            this.weight = weight;
        }
    }


    // Define a new struct for Bounds (center and width/height)
    [System.Serializable]
    public struct Bounds2D {
        public Vector2 center;
        public Vector2 size;
            
        public Bounds2D(Vector2 center, Vector2 size) {
            this.center = center;
            this.size = size;
        }

        // Convert to Rect for compatibility in some cases (optional)
        public Rect ToRect() {
            return new Rect(center.x - size.x / 2, center.y - size.y / 2, size.x, size.y);
        }
    }

    /// <summary>
    /// Generates a Perlin noise texture.
    /// </summary>
    public static Texture2D MakePerlinNoiseMap(int width, int height, float scale, int seed) {
        Texture2D texture = new Texture2D(width, height);
        UnityEngine.Random.InitState(seed);

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                float perlinValue = Mathf.PerlinNoise(x / scale, y / scale);
                texture.SetPixel(x, y, new Color(perlinValue, perlinValue, perlinValue));
            }
        }

        texture.Apply();
        return texture;
    }

    /// <summary>
    /// Generates decoration points using a Perlin noise texture.
    /// </summary>
    public static List<Vector2> Perlin(Texture2D perlinImage, int freq, Bounds2D outerBounds, List<Bounds2D> noNoSquares, float noiseThreshold = 0.3f, bool debug = false) {
        List<Vector2> points = new List<Vector2>();

        int width = perlinImage.width;
        int height = perlinImage.height;
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(freq * 10));

        for (int gx = 0; gx < gridSize; gx++) {
            for (int gy = 0; gy < gridSize; gy++) {
                float px = Mathf.Lerp(outerBounds.center.x - outerBounds.size.x / 2, outerBounds.center.x + outerBounds.size.x / 2, gx / (float)gridSize);
                float py = Mathf.Lerp(outerBounds.center.y - outerBounds.size.y / 2, outerBounds.center.y + outerBounds.size.y / 2, gy / (float)gridSize);

                Color pixel = perlinImage.GetPixelBilinear(px / width, py / height);
                float noiseValue = pixel.grayscale;

                if (noiseValue > noiseThreshold) {
                    int localFreq = Mathf.RoundToInt(freq * noiseValue);

                    for (int i = 0; i < localFreq; i++) {
                        Vector2 candidate = new Vector2(
                            UnityEngine.Random.Range(px, px + outerBounds.size.x / gridSize),
                            UnityEngine.Random.Range(py, py + outerBounds.size.y / gridSize)
                        );

                        if (!IsInNoNoZone(candidate, noNoSquares)) {
                            points.Add(candidate);
                        }
                    }
                }
            }
        }

        if (debug) Debug.Log($"Generated {points.Count} Perlin-based points.");
        return points;
    }

    /// <summary>
    /// Generates random decoration points with fallback validation.
    /// </summary>
    public static List<Vector2> Random(int freq, Bounds2D outerBounds, List<Bounds2D> noNoSquares, int maxValidations = 100, bool debug = false) {
        List<Vector2> points = new List<Vector2>();
        List<List<Vector2>> attemptedGenerations = new List<List<Vector2>>();

        for (int attempt = 0; attempt < maxValidations; attempt++) {
            List<Vector2> currentTry = new List<Vector2>();

            for (int i = 0; i < freq; i++) {
                Vector2 candidate = new Vector2(
                    UnityEngine.Random.Range(outerBounds.center.x - outerBounds.size.x / 2, outerBounds.center.x + outerBounds.size.x / 2),
                    UnityEngine.Random.Range(outerBounds.center.y - outerBounds.size.y / 2, outerBounds.center.y + outerBounds.size.y / 2)
                );

                if (!IsInNoNoZone(candidate, noNoSquares)) {
                    currentTry.Add(candidate);
                }
            }

            attemptedGenerations.Add(currentTry);

            if (currentTry.Count == freq) {
                if (debug) Debug.Log($"Random generation succeeded in {attempt + 1} tries.");
                return currentTry;
            }
        }

        List<Vector2> bestTry = attemptedGenerations[0];
        foreach (var attempt in attemptedGenerations) {
            if (attempt.Count > bestTry.Count) bestTry = attempt;
        }

        if (debug) Debug.Log($"Random generation failed after {maxValidations} attempts, best result had {bestTry.Count} points.");
        return bestTry;
    }

    public int GetWeightedRandomIndex(List<float> weights) {
        float totalWeight = 0;
        foreach (var weight in weights) {
            totalWeight += weight;
        }

        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        for (int i = 0; i < weights.Count; i++) {
            cumulativeWeight += weights[i];
            if (randomValue <= cumulativeWeight) {
                return i;
            }
        }

        return weights.Count - 1; // Fallback
    }


    /// <summary>
    /// Checks if a point is inside any no-go zone.
    /// </summary>
    private static bool IsInNoNoZone(Vector2 point, List<Bounds2D> noNoSquares) {
        foreach (Bounds2D bounds in noNoSquares) {
            Rect rect = bounds.ToRect();
            if (rect.Contains(point)) return true;
        }
        return false;
    }

    private void Start() {

        // Grass Decorations
        if (GrassUsePerlin) {
            // Generate or use existing image? (if make use bounds as width and height)
            Texture2D perlinImage = GrassPUseImage;
            if (GrassPgenSeed < 0) GrassPgenSeed = UnityEngine.Random.Range(0, 1000000);
            if (perlinImage == null) {
                perlinImage = MakePerlinNoiseMap(
                    Mathf.CeilToInt(GrassOuterBounds.size.x),
                    Mathf.CeilToInt(GrassOuterBounds.size.y),
                    GrassPgenScale,
                    GrassPgenSeed
                );
            }

            // Generate the points using the perlin image
            grassPoints = Perlin(
                perlinImage,
                GrassPFreq,
                GrassOuterBounds,
                GrassNoNoSquares,
                GrassPNoiseThreshold,
                GrassDebugLog
            );
        } else {
            grassPoints = Random(
                GrassRFreq,
                GrassOuterBounds,
                GrassNoNoSquares,
                GrassRMaxValidations,
                GrassDebugLog
            );
        }


        /*
        for (int i = 0; i < grassPoints.Count; i++) {
            GameObject prefab = grassPrefabs[UnityEngine.Random.Range(0, grassPrefabs.Length)];
            Instantiate(prefab, grassPoints[i], Quaternion.identity);
        }
        */

        // Pre-calc the weights
        List<float> colorWeights = new List<float>();
        foreach (var colorOption in colors) {
            colorWeights.Add(colorOption.weight);
        }

        List<float> prefabWeights = new List<float>();
        foreach (var decorationPrefab in grassPrefabs) {
            prefabWeights.Add(decorationPrefab.weight);
        }

        for (int i = 0; i < grassPoints.Count; i++) {
            // Select the prefab based on weight
            int selectedPrefabIndex = GetWeightedRandomIndex(prefabWeights);
            DecorationPrefab selectedPrefab = grassPrefabs[selectedPrefabIndex];

            GameObject prefab = selectedPrefab.prefab;
            GameObject instantiatedPrefab = Instantiate(prefab, grassPoints[i], Quaternion.identity);

            // Handle color change if applicable
            if (selectedPrefab.changeColor) {
                // Select a color based on weight
                int selectedColorIndex = GetWeightedRandomIndex(colorWeights);
                Color selectedColor = colors[selectedColorIndex].color;

                // Apply the color to the SpriteRenderer component
                SpriteRenderer spriteRenderer = instantiatedPrefab.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null) {
                    spriteRenderer.color = selectedColor;
                }
            }
        }
    }

    private void OnDrawGizmos() {
        // Grass Decorations
        //// Bounds
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(GrassOuterBounds.center, GrassOuterBounds.size);

        //// NoNoSquares
        Gizmos.color = Color.black;
        foreach (Bounds2D bounds in GrassNoNoSquares) {
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        //// Points
        Gizmos.color = Color.red;
        foreach (Vector2 point in grassPoints) {
            Gizmos.DrawSphere(point, 0.1f);
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

public class Decoration_Tools : MonoBehaviour {

    // Serialized fields for the "Grass" decorations, for both perlin and not perlin
    [SerializeField]
    private bool Grass_UsePerlin = true;
    [SerializeField]
    private Texture2D Grass_Perlin_UseImage; // If not defined we generate

    // General Usage Parameters (things both random and perlin have in common ex nonosquares)
    [SerializeField]
    private Bounds2D Grass_OuterBounds;
    [SerializeField]
    private List<Bounds2D> Grass_NoNoSquares;
    [SerializeField]
    private bool Grass_Debug = false;

    // Perlin Generation Parameters
    [SerializeField]
    private int Grass_Perlin_Gen_Seed = -1; // Negative gives random seed
    [SerializeField]
    private float Grass_Perlin_Gen_Scale = 10;
    [SerializeField]
    private int Grass_Perlin_Frequency = 50;

    // Perlin Usage Parameters
    [SerializeField]
    private float Grass_Perlin_NoiseThreshold = 0.3f;

    // Random Usage Parameters
    [SerializeField]
    private int Grass_Random_MaxValidations = 100;
    [SerializeField]
    private int Grass_Random_Frequency = 200;

    private List<Vector2> grassPoints;

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
        if (Grass_UsePerlin) {
            // Generate or use existing image? (if make use bounds as width and height)
            Texture2D perlinImage = Grass_Perlin_UseImage;
            if (Grass_Perlin_Gen_Seed < 0) Grass_Perlin_Gen_Seed = UnityEngine.Random.Range(0, 1000000);
            if (perlinImage == null) {
                perlinImage = MakePerlinNoiseMap(
                    Mathf.CeilToInt(Grass_OuterBounds.size.x),
                    Mathf.CeilToInt(Grass_OuterBounds.size.y),
                    Grass_Perlin_Gen_Scale,
                    Grass_Perlin_Gen_Seed
                );
            }

            // Generate the points using the perlin image
            grassPoints = Perlin(
                perlinImage,
                Grass_Perlin_Frequency,
                Grass_OuterBounds,
                Grass_NoNoSquares,
                Grass_Perlin_NoiseThreshold,
                Grass_Debug
            );
        } else {
            grassPoints = Random(
                Grass_Random_Frequency,
                Grass_OuterBounds,
                Grass_NoNoSquares,
                Grass_Random_MaxValidations,
                Grass_Debug
            );
        }
    }

    private void OnDrawGizmos() {
        // Grass Decorations
        //// Bounds
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Grass_OuterBounds.center, Grass_OuterBounds.size);

        //// NoNoSquares
        Gizmos.color = Color.black;
        foreach (Bounds2D bounds in Grass_NoNoSquares) {
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        //// Points
        Gizmos.color = Color.red;
        foreach (Vector2 point in grassPoints) {
            Gizmos.DrawSphere(point, 0.1f);
        }
    }
}

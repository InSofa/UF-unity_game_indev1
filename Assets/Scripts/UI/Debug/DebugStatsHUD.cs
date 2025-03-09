using UnityEngine;
using TMPro;
using UnityEngine.Profiling;
using System.Diagnostics; // For thread count, etc.

public class DebugStatsHUD : MonoBehaviour {
    // Reference to the TextMeshPro UI Text component
    public TMP_Text debugText;

    // Time interval for updating stats (in seconds)
    public float updateInterval = 0.5f;

    private float timeSinceLastUpdate = 0.0f;
    private int frameCount = 0;
    private float fps;
    private float frameTime;
    private int objectCount;
    private long memoryUsage;
    private int loadedAssets;
    private float physicsStepTime;
    private int threadCount;

    private bool hasFirstFrame = false;

    void Start() {
        debugText = GetComponent<TMP_Text>();
    }

    void Update() {
        // Accumulate time and count frames for FPS calculation
        timeSinceLastUpdate += Time.deltaTime;
        frameCount++;

        if (timeSinceLastUpdate >= updateInterval || hasFirstFrame == false) {
            if (hasFirstFrame == false) {
                hasFirstFrame = true;
            }
            // Calculate FPS and frame time
            fps = frameCount / timeSinceLastUpdate;
            frameTime = Time.deltaTime * 1000f; // Frame time in milliseconds

            // Reset counters
            timeSinceLastUpdate = 0;
            frameCount = 0;

            // Get system information
            objectCount = FindObjectsByType<GameObject>(FindObjectsSortMode.None).Length;
            memoryUsage = System.GC.GetTotalMemory(false);
            loadedAssets = UnityEngine.Object.FindObjectsByType<UnityEngine.Object>(UnityEngine.FindObjectsSortMode.None).Length;

            physicsStepTime = Time.fixedDeltaTime * 1000f; // in ms
            threadCount = System.Diagnostics.Process.GetCurrentProcess().Threads.Count;

            // Update the TMP text with new stats
            UpdateDebugStats();
        }
    }

    void UpdateDebugStats() {
        // Replace placeholders for CPU and GPU usage with your actual implementation
        string cpuUsage = "CPU: N/A (NotImplemented)"; // Placeholder (requires custom implementation or external library)
        string gpuUsage = "GPU: N/A (NotImplemented)"; // Placeholder (requires custom implementation or external library)

        debugText.text = $"FPS: {fps:F2}\n" +
                         $"Frame Time: {frameTime:F2} ms\n" +
                         $"Object Count: {objectCount}\n" +
                         $"Memory Usage: {memoryUsage / (1024f * 1024f):F2} MB\n" +
                         $"Loaded Assets: {loadedAssets}\n" +
                         "\n" +
                         $"Physics Step Time: {physicsStepTime:F2} ms\n" +
                         $"Unity Version: {Application.unityVersion}\n" +
                         $"Platform: {Application.platform}";
    }
}

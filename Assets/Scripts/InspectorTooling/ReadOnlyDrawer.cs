using UnityEngine;
using UnityEditor; // Only needed in Editor mode

public class Example : MonoBehaviour
{
    [ReadOnly] [SerializeField] private string runtimeString;

    public void UpdateValue(string newValue)
    {
        runtimeString = newValue;
        #if UNITY_EDITOR
        EditorUtility.SetDirty(this); // Force Unity to refresh the Inspector
        #endif
    }

    private void Update()
    {
        if (Time.frameCount % 60 == 0) // Example: Change every second
        {
            UpdateValue("Updated at: " + Time.time.ToString("F2"));
        }
    }
}

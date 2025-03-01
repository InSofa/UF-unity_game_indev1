using UnityEngine;

public class RangeVisual : MonoBehaviour
{
    public float range = 10f;
    public float segmentSize;

    [SerializeField]
    GameObject visualLine;

    [SerializeField]
    bool testing;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(testing) {
            GenerateVisual();
        }
    }

    
    public void GenerateVisual() {
        float circumferance = 2 * Mathf.PI * range;
        int segmentCount = Mathf.RoundToInt(circumferance / segmentSize);
        float segmentAngle = 360 / (float)segmentCount;

        //Debug.Log($"Segment count: {segmentCount}, Circumferance: {circumferance} SegmentSize: {segmentSize} Div: {segmentAngle}");

        LineRenderer lr = visualLine.GetComponent<LineRenderer>();
        lr.positionCount = segmentCount;
        lr.useWorldSpace = false;
        lr.startWidth = segmentSize;
        lr.endWidth = segmentSize;
        for (int i = 0; i < lr.positionCount; i++) {
            float x = Mathf.Sin(Mathf.Deg2Rad * i * segmentAngle) * range;
            float y = Mathf.Cos(Mathf.Deg2Rad * i * segmentAngle) * range;
            lr.SetPosition(i, new Vector3(x, y, 0));
        }
    }
}

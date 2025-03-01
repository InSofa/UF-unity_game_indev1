using UnityEngine;

public class RangeVisual : MonoBehaviour
{
    public float range = 10f;

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
        LineRenderer lr = visualLine.GetComponent<LineRenderer>();
        lr.positionCount = 361;
        lr.useWorldSpace = false;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        for (int i = 0; i < lr.positionCount; i++) {
            float x = Mathf.Sin(Mathf.Deg2Rad * i) * range;
            float y = Mathf.Cos(Mathf.Deg2Rad * i) * range;
            lr.SetPosition(i, new Vector3(x, y, 0));
        }
    }
}

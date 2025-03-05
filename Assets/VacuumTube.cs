using UnityEngine;

public class VacuumTube : MonoBehaviour
{
    [SerializeField]
    LineRenderer line;

    [SerializeField]
    Transform start;

    [SerializeField]
    Transform end;

    // Update is called once per frame
    void Update()
    {
        line.SetPosition(0, start.position);
        line.SetPosition(1, end.position);
    }
}

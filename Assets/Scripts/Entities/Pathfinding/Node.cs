using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public GameObject building = null;

    public Node(bool walkable, Vector3 worldPosition) {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
    }
}

using UnityEngine;

public class pathfindingGrid : MonoBehaviour
{
    [SerializeField]
    private LayerMask unwalkableMask;
    [SerializeField]
    private Vector2 gridWorldSize;
    [SerializeField]
    private float nodeRadius;

    public Node[,] grid;

    [SerializeField]
    Transform player, cursor;

    private float nodeDiameter;
    private int gridSizeX, gridSizeY;

    private void Start() {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    void Update() {
        CreateGrid();
    }

    private void CreateGrid() {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++) {
            for (int y = 0; y < gridSizeY; y++) {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);

                bool walkable = !Physics2D.OverlapCircle(worldPoint, nodeRadius - .2f, unwalkableMask);
                grid[x, y] = new Node(walkable, worldPoint);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition) {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.y + gridWorldSize.y / 2) / gridWorldSize.y;

        int x = Mathf.FloorToInt(Mathf.Clamp((gridSizeX) * percentX, 0, gridSizeX - 1));
        int y = Mathf.FloorToInt(Mathf.Clamp((gridSizeY) * percentY, 0, gridSizeY - 1));
        //int y = Mathf.RoundToInt((gridSizeY) * percentY) + 1;


        return grid[x, y];
    }

    public void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 0));
        if (grid != null) {
            Node playerNode = NodeFromWorldPoint(player.position);
            foreach (Node n in grid) {
                Gizmos.color = n.walkable ? Color.white : Color.red;
                if (playerNode == n) {
                    Gizmos.color = Color.cyan;
                }
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeRadius * 2 * 0.4f));
            }
        }
    }
}

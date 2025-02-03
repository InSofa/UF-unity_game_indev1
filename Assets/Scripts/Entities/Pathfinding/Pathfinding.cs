using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class Pathfinding : MonoBehaviour {

    public static Pathfinding instance;
    PathfindingGrid grid;

    void Awake() {
        grid = PathfindingGrid.instance;
        instance = this;
    }

    void Update() {
        //FindPath(seeker.position, target.position);
    }
    public SimpleNode[] FindPath(Vector3 startPos, Vector3 targetPos) {

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0) {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if (currentNode == targetNode) {
                Node[] nodePath = RetracePath(startNode, targetNode);
                List<SimpleNode> simpleNodePath = new List<SimpleNode>();
                foreach (Node node in nodePath) {
                    simpleNodePath.Add(new SimpleNode(node.worldPosition, node.building));
                }
                return simpleNodePath.ToArray();
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else {
                        openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
        return null;
    }

    Node[] RetracePath(Node startNode, Node endNode) {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        grid.path = path;

        return SimplifyPath(path);
    }

    //IF PATHFINDING NO WORK MIGHT BE THIS
    Node[] SimplifyPath(List<Node> path) {
        List<Node> waypoints = new List<Node>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++) {
            if (path[i].building != null) {
                waypoints.Add(path[i]);
                continue;
            }

            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld) {
                waypoints.Add(path[i]);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB) {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}

public struct SimpleNode {
    public Vector2 worldPosition { get;}
    public GameObject building { get; }

    public SimpleNode(Vector2 worldPosition, GameObject building) {
        this.worldPosition = worldPosition;
        this.building = building;
    }
}
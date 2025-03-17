using UnityEngine;
using System.Collections;
using Unity.Mathematics;

public class Node : IHeapItem<Node> {

    public bool walkable;
    public Vector3 worldPosition;

    public GameObject building;
    public bool isBed;

    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public Node parent;
    int heapIndex;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, bool _isBed) {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        isBed = _isBed;
    }

    public int fCost {
        get {
            if(building != null) {
                int buildingAdditionalValue = Mathf.RoundToInt(building.GetComponent<BuildingHealth>().currentHealth * 100f);
                Debug.Log(buildingAdditionalValue + " " + building.name);
                return Mathf.Abs(gCost + hCost + buildingAdditionalValue);
            }
            return gCost + hCost;
        }
    }

    public int HeapIndex {
        get {
            return heapIndex;
        }
        set {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare) {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0) {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}
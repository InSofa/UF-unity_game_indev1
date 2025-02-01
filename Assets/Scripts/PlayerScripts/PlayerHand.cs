using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHand : MonoBehaviour
{
    pathfindingGrid grid;

    [SerializeField]
    int pillows = 5;

    [SerializeField]
    TextMeshProUGUI pillowText;

    Camera cam;

    [SerializeField]
    BuildingScriptableObject[] buildings;

    int selectedBuilding = 0;

    Vector2 buildPos;

    [SerializeField]
    float buildRange = 1f;

    [SerializeField]
    Transform buildIndicator, placementIndicator;


    [SerializeField]
    float collisionRadius;

    Vector2 mousePos;

    [SerializeField]
    InputActionReference mousePositionInput;

    [SerializeField]
    InputActionReference mouseClick;


    private void Start()
    {
        cam = Camera.main;
        pillowText.text = pillows.ToString();

        grid = GameObject.Find("PathfindingGrid").GetComponent<pathfindingGrid>();
        grid.player = transform;
        grid.buildPlacement = buildIndicator;
    }

    private void Update()
    {
        takeInput();
    }

    private void takeInput()
    {
        mousePos = mousePositionInput.action.ReadValue<Vector2>();
        Vector2 worldMousePos = cam.ScreenToWorldPoint(mousePos);
        Vector2 v2Pos = new Vector2(transform.position.x, transform.position.y);

        Vector2 lookDir =  worldMousePos - v2Pos;
        if (lookDir.magnitude > buildRange)
        {
            lookDir.Normalize();
            lookDir *= buildRange;
        }

        buildPos = v2Pos + lookDir;
        buildIndicator.transform.position = buildPos;

        placementIndicator.position = grid.NodeFromWorldPoint(buildPos).worldPosition;
    }

    private void OnEnable()
    {
        mouseClick.action.started += placeBuilding;
    }

    private void OnDisable()
    {
        mouseClick.action.started -= placeBuilding;
    }

    public void addPillow(int amount)
    {
        pillows += amount;
        pillowText.text = pillows.ToString();
    }

    public void SwitchBuildSelection(int selection)
    {
        if(selection >= buildings.Length)
        {
            return;
        }
        selectedBuilding = selection;
    }

    private void placeBuilding(InputAction.CallbackContext obj)
    {
        if (pillows < buildings[selectedBuilding].buildingCost) {
            Debug.Log("Not enough pillows");
            return;
        }

        bool placed = grid.CreateBuilding(buildings[selectedBuilding].buildingPrefab);

        if (placed) {
            pillows -= buildings[selectedBuilding].buildingCost;
            pillowText.text = pillows.ToString();
            return;
        }
        Debug.Log("Cannot place building here");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, buildRange);
    }
}


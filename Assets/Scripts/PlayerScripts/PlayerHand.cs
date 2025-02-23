using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHand : MonoBehaviour
{
    PathfindingGrid grid;

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

    Vector2 lookDir;
    PlayerInput pi;

    [SerializeField]
    InputActionReference lookInput;


    [SerializeField]
    InputActionReference buildInput, sellBuildingInput;

    private void Start()
    {
        cam = Camera.main;
        pillowText.text = pillows.ToString();

        PathfindingGrid.instance.buildPlacement = buildIndicator;

        pi = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        takeInput();
    }

    private void takeInput()
    {
        if (pi.currentControlScheme == "MnK") {
            Vector2 mousePos = lookInput.action.ReadValue<Vector2>();
            Vector2 worldMousePos = cam.ScreenToWorldPoint(mousePos);

            lookDir = worldMousePos - (Vector2)transform.position;
            if (lookDir.magnitude > buildRange) {
                lookDir.Normalize();
                lookDir *= buildRange;
            }
        } else if(pi.currentControlScheme == "Gamepad") {
            Vector2 joystickInput = lookInput.action.ReadValue<Vector2>();

            if (Settings.rawJoystickInput) {
                lookDir = joystickInput * buildRange;
            } else {
                lookDir += joystickInput * Time.deltaTime * Settings.joystickLookSensitivity;
                if (lookDir.magnitude > buildRange) {
                    lookDir.Normalize();
                    lookDir *= buildRange;
                }
            }
        }


        buildPos = (Vector2)transform.position + lookDir;
        buildIndicator.transform.position = buildPos;

        placementIndicator.position = PathfindingGrid.instance.NodeFromWorldPoint(buildPos).worldPosition;
    }

    private void OnEnable()
    {
        buildInput.action.started += placeBuilding;
        sellBuildingInput.action.started += removeBuilding;
    }

    private void OnDisable()
    {
        buildInput.action.started -= placeBuilding;
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

        bool placed = PathfindingGrid.instance.CreateBuilding(buildings[selectedBuilding].buildingPrefab);

        if (placed) {
            pillows -= buildings[selectedBuilding].buildingCost;
            pillowText.text = pillows.ToString();
            return;
        }
        Debug.Log("Cannot place building here");
    }

    private void removeBuilding(InputAction.CallbackContext obj) {
        int? sellAmount = PathfindingGrid.instance.RemoveBuilding(buildPos);
        if(sellAmount != null) {
            Debug.Log("Sold building for " + sellAmount + " pillows");
            addPillow((int)sellAmount);
            return;
        }
        Debug.Log("No building to sell here");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, buildRange);
    }
}


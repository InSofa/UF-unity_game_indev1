using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHand : MonoBehaviour
{
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

    [Header("Input Actions")]
    [SerializeField]
    InputActionReference buildInput;

    [SerializeField]
    InputActionReference sellBuildingInput;

    [SerializeField]
    InputActionReference meleeSwitch;

    [Header("Melee Attack")]
    [SerializeField]
    float meleeRange;

    [SerializeField]
    float meleeRadius;

    [SerializeField]
    int meleeDamage;

    [SerializeField]
    public bool isMeleeMode = false; // Made public for UIControls system

    [SerializeField]
    LayerMask enemyLayer;

    private void Start()
    {
        cam = Camera.main;
        pillowText.text = pillows.ToString();

        PathfindingGrid.instance.buildPlacement = buildIndicator;

        pi = GetComponent<PlayerInput>();

        buildInput.action.started += placeBuilding;
        sellBuildingInput.action.started += removeBuilding;
        meleeSwitch.action.started += switchMeleeMode;
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
            //Settings.rawJoystickInput
            if (isMeleeMode) {
                lookDir = joystickInput * meleeRange;
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
        sellBuildingInput.action.started -= removeBuilding;
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

    private void meleeAttack(InputAction.CallbackContext obj) {
        Vector2 attackPos = (Vector2)transform.position + lookDir;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackPos, meleeRadius, enemyLayer);

        foreach (Collider2D hitCollider in hitColliders) {
            EnemyHealth eh = hitCollider.GetComponent<EnemyHealth>();
            if(eh == null) {
                continue;
            }
            eh.TakeDamage(meleeDamage);
        }
    }

    private void switchMeleeMode(InputAction.CallbackContext obj) {
        if(isMeleeMode) {
            isMeleeMode = false;
            buildInput.action.started += placeBuilding;
            sellBuildingInput.action.started += removeBuilding;
            buildInput.action.started -= meleeAttack;
            return;
        }

        isMeleeMode = true;
        buildInput.action.started -= placeBuilding;
        sellBuildingInput.action.started -= removeBuilding;
        buildInput.action.started += meleeAttack;
    }

    private void OnDrawGizmos()
    {
        if (isMeleeMode) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere((Vector2)transform.position + lookDir * meleeRange, meleeRadius);
            return;
        }
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, buildRange);
    }
}


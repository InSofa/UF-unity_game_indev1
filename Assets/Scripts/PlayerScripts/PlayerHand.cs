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
    Transform buildIndicator, attackIndicator, placementIndicator, buildingIndicator;


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
    bool isMeleeMode = false;

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
        if(isMeleeMode) {
            attackIndicator.position = buildPos;
            return;
        } else {
            buildIndicator.position = buildPos;
            Node node = PathfindingGrid.instance.NodeFromWorldPoint(buildPos);
            Node playerNode = PathfindingGrid.instance.NodeFromWorldPoint((Vector2)transform.position);
            if(node.building != null) {
                buildingIndicator.position = node.worldPosition;

                //If the hovered node is the same node as the node the player is standing on, dont show the preview
                if(playerNode != node) {
                    buildingIndicator.gameObject.SetActive(true);
                } else {
                    buildingIndicator.gameObject.SetActive(false);
                }
                placementIndicator.gameObject.SetActive(false);
            } else {
                placementIndicator.position = node.worldPosition;
                buildingIndicator.gameObject.SetActive(false);

                //See comment earlier
                if (playerNode != node) {
                    placementIndicator.gameObject.SetActive(true);
                } else {
                    placementIndicator.gameObject.SetActive(false);
                }
            }
        }

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

    //Playernode and placement node check is done in the grid logic
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

            attackIndicator.gameObject.SetActive(false);
            buildIndicator.gameObject.SetActive(true);
            placementIndicator.gameObject.SetActive(true);
            return;
        }

        isMeleeMode = true;
        buildInput.action.started -= placeBuilding;
        sellBuildingInput.action.started -= removeBuilding;
        buildInput.action.started += meleeAttack;

        attackIndicator.gameObject.SetActive(true);
        buildIndicator.gameObject.SetActive(false);
        placementIndicator.gameObject.SetActive(false);
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


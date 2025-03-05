using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHand : MonoBehaviour {
    LocalSoundComposer lsc;

    [SerializeField]
    UIHandler uiHandler;

    // Reference to mainGameTokenIconResolver
    [SerializeField]
    public MainGame_TokenIconResolver mainGameTokenIconResolver;

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

    Vector2 lastLookDir = Vector2.right;
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
    float meleeLookSpeed;

    [SerializeField]
    float meleeCD;

    float meleeTimer;

    //Offsets the attack position from the lookDir
    [SerializeField]
    float attackOffset;

    [SerializeField]
    int meleeDamage;

    [SerializeField]
    public bool isMeleeMode = false; // Made public for UIControls system

    [SerializeField]
    LayerMask enemyLayer;

    [Header("Sound")]
    [SerializeField]
    string buildModeSFX;

    [SerializeField]
    string meleeModeSFX;

    [SerializeField]
    string buildSFX;

    [SerializeField]
    string sellSFX;

    private void Start()
    {
        lsc = GetComponent<LocalSoundComposer>();

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
        if (isMeleeMode) {
            meleeTimer += Time.deltaTime;
        }
        takeInput();
    }

    private void takeInput()
    {
        if (pi.currentControlScheme == "MnK") {
            Vector2 mousePos = lookInput.action.ReadValue<Vector2>();
            Vector2 worldMousePos = cam.ScreenToWorldPoint(mousePos);

            lookDir = worldMousePos - (Vector2)transform.position;
            if (isMeleeMode) {
                /*if (lookDir.magnitude > meleeRange) {*/
                    lookDir.Normalize();
                    lookDir *= meleeRange;
                //}
            } else  if(lookDir.magnitude > buildRange) {
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



        if(isMeleeMode) {
            if(lookDir.magnitude != 0) {
                lastLookDir = lookDir;
            }
            float angle = Mathf.Atan2(lastLookDir.y, lastLookDir.x) * Mathf.Rad2Deg;

            Vector2 attackDir = attackIndicator.position - transform.position;
            float attackAngle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;

            float newAngle = Mathf.LerpAngle(attackAngle, angle, meleeLookSpeed * Time.deltaTime);

            Vector2 newDir = new Vector2(Mathf.Cos(newAngle * Mathf.Deg2Rad), Mathf.Sin(newAngle * Mathf.Deg2Rad));

            //Debug.Log($"Angle: {angle}, newAngle: {newAngle}, newDir: {newDir}");

            attackIndicator.position = (Vector2)transform.position + newDir * meleeRange;

            return;
        } else {
            buildPos = (Vector2)transform.position + lookDir;
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
        uiHandler.highlightBuildingSelected();
        selectedBuilding = selection;
    }

    //Playernode and placement node check is done in the grid logic
    private void placeBuilding(InputAction.CallbackContext obj)
    {
        if (pillows < buildings[selectedBuilding].buildingCost) {
            //Debug.Log("Not enough pillows");
            return;
        }

        bool placed = PathfindingGrid.instance.CreateBuilding(buildings[selectedBuilding].buildingPrefab);

        if (placed) {
            lsc.PlayFx(buildSFX);
            pillows -= buildings[selectedBuilding].buildingCost;
            pillowText.text = pillows.ToString();
            return;
        }
        //Debug.Log("Cannot place building here");
    }

    private void removeBuilding(InputAction.CallbackContext obj) {
        int? sellAmount = PathfindingGrid.instance.RemoveBuilding(buildPos);
        if(sellAmount != null) {
            lsc.PlayFx(sellSFX);
            //Debug.Log("Sold building for " + sellAmount + " pillows");
            addPillow((int)sellAmount);
            return;
        }
        //Debug.Log("No building to sell here");
    }

    private void meleeAttack(InputAction.CallbackContext obj) {
        if(meleeTimer < meleeCD) {
            return;
        }
        meleeTimer = 0;

        Vector2 attackPos = (Vector2)attackIndicator.position - (attackOffset * lookDir.normalized);
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
        if (isMeleeMode) {
            lsc.PlayFx(buildModeSFX);

            isMeleeMode = false;
            buildInput.action.started += placeBuilding;
            sellBuildingInput.action.started += removeBuilding;
            buildInput.action.started -= meleeAttack;

            attackIndicator.gameObject.SetActive(false);
            buildIndicator.gameObject.SetActive(true);
            placementIndicator.gameObject.SetActive(true);
        } else {
            lsc.PlayFx(meleeModeSFX);

            isMeleeMode = true;
            buildInput.action.started -= placeBuilding;
            sellBuildingInput.action.started -= removeBuilding;
            buildInput.action.started += meleeAttack;

            attackIndicator.gameObject.SetActive(true);
            buildIndicator.gameObject.SetActive(false);
            buildingIndicator.gameObject.SetActive(false);
            placementIndicator.gameObject.SetActive(false);
        }

        // Update mainTokenIconResolver's text
        if (mainGameTokenIconResolver) {
            mainGameTokenIconResolver.UpdateMeleeMode(isMeleeMode);
        }

    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);

        //Added unnecessary logic to mimic attack logic
        Gizmos.DrawWireSphere((Vector2)transform.position + lookDir - (attackOffset * lookDir.normalized), meleeRadius);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, buildRange);
    }
}


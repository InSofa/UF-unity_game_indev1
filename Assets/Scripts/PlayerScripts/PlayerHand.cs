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
    GameObject[] buildings;

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

        //+ .5f to center the placement indicator on the grid
        Vector2 placementSpot = new Vector2(Mathf.Round(buildPos.x) + .5f, Mathf.Round(buildPos.y) + .5f);
        placementIndicator.transform.position = placementSpot;

        if(placementIndicator.transform.position == new Vector3(Mathf.Round(transform.position.x) + .5f, Mathf.Round(transform.position.y) + .5f)) {
            placementIndicator.gameObject.SetActive(false);
        } else {
            placementIndicator.gameObject.SetActive(true);
        }
    }

    private void OnEnable()
    {
        mouseClick.action.started += placeTurret;
    }

    private void OnDisable()
    {
        mouseClick.action.started -= placeTurret;
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

    private void placeTurret(InputAction.CallbackContext obj)
    {
        //Vector2 clickPoint = cam.ScreenToWorldPoint(mousePos);
        Vector2 clickPoint = buildPos;

        Vector2 placementSpot = new Vector2(Mathf.Round(clickPoint.x), Mathf.Round(clickPoint.y));

        Collider2D[] colliders = Physics2D.OverlapCircleAll(placementSpot, collisionRadius);

        if(colliders.Length > 0 || pillows <= 0)
        {
            Debug.Log("Can't place here");
            return;
        }

        pillows--;
        pillowText.text = pillows.ToString();

        Instantiate(buildings[selectedBuilding], placementSpot, Quaternion.identity);
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, buildRange);
    }
}


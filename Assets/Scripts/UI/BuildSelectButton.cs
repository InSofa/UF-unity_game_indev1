using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildSelectButton : MonoBehaviour {

    [SerializeField]
    private PlayerHand playerHand;
    [SerializeField]
    private int buildingIndex;
    [SerializeField]
    private GameObject nameDisplay;
    [SerializeField]
    private GameObject costDisplay;

    private BuildingScriptableObject building;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        building = playerHand.buildings[buildingIndex];
    }

    // Update is called once per frame
    void Update() {
        if (building == null) {
            building = playerHand.buildings[buildingIndex];
        }
        nameDisplay.GetComponent<TMP_Text>().text = building.buildingName;
        costDisplay.GetComponent<TMP_Text>().text = (building.buildingCost * playerHand.GlobalBuyInflationMultiplier).ToString();
    }
}

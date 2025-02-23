using UnityEngine;

[CreateAssetMenu(fileName = "BuildingScriptableObject", menuName = "Scriptable Objects/BuildingScriptableObject")]
public class BuildingScriptableObject : ScriptableObject
{
    public string buildingName;

    public int buildingCost;
    public int buildingSellValue;

    public int buildingHealth;
    public int buildingDamage;
    public float buildingRange;

    public string buildingDescription;

    public GameObject buildingPrefab;
}

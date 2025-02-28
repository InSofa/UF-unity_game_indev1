using UnityEngine;

[CreateAssetMenu(fileName = "BuildingScriptableObject", menuName = "Scriptable Objects/BuildingScriptableObject")]
public class BuildingScriptableObject : ScriptableObject
{
    public string buildingName;

    public int buildingCost;
    public int buildingSellValue;

    [Space]
    public int buildingHealth;
    public int buildingDamage;
    public float buildingRange;

    [Space]
    public float shotCD;
    public float projectileSpeed;
    [Space]

    public string buildingDescription;

    public GameObject buildingPrefab;
}

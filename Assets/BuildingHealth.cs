using UnityEngine;

public class BuildingHealth : MonoBehaviour
{
    public BuildingScriptableObject buildingScriptableObject;

    [HideInInspector]
    public int currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = buildingScriptableObject.buildingHealth;
    }

    public bool TakeDamage(int damage) {

        currentHealth -= damage;
        print("Building took damage, currenthp: " + currentHealth);
        if (currentHealth <= 0) {
            print("Building destroyed");
            PathfindingGrid.instance.RemoveBuilding(transform.position);
            return true;
        }
        return false;
    }
}

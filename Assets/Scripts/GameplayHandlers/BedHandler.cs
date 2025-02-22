using UnityEngine;
using UnityEngine.InputSystem;

public class BedHandler : MonoBehaviour
{
    [SerializeField]
    InputActionReference startNewWave;

    [SerializeField]
    float minInteractDistance = 3f;

    [SerializeField]
    GameObject interactVisuals;

    // Update is called once per frame
    void Update()
    {
        Vector2 playerPos = PlayerController.player.transform.position;
        float distance = Vector2.Distance(playerPos, transform.position);
        if (distance <= minInteractDistance) {
            interactVisuals.SetActive(true);
            if (startNewWave.action.triggered) {
                if(EnemySpawner.instance.currentEnemies.Count == 0)
                    EnemySpawner.instance.SpawnWave();
                else {
                    Debug.Log("Enemies still alive");
                }
            }
        } else {
            interactVisuals.SetActive(false);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minInteractDistance);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public enum entityPriority {
    Nearest = 0,
    Player = 1,
    Buildings = 2,
    Bed = 3
}

public class EnemyHandler : MonoBehaviour {
    private static GameObject player;
    private static PlayerHealth ph;

    //Not used yet
    [SerializeField]
    entityPriority priority;

    private Pathfinding pathfinder;

    public List<SimpleNode> path;

    public void Start() {
        if (player == null || ph == null) {
            player = GameObject.Find("Player");
            ph = player.GetComponent<PlayerHealth>();
        }

        PathfindingGrid.instance.enemies.Add(this);
        pathfinder = Pathfinding.instance;
    }
    void GetPath(Vector2 targetPos) {
        Debug.Log("Getting path");
        path = pathfinder.FindPath(transform.position, targetPos).ToList();
        Debug.Log("Path found " + path.Count);
    }

    //A* pathfinding funtionality, finding the right path depending on target priority(buildings, player, and bed)
    //Implement A* pathfinding funtionality, finding the right path depending on target priority(buildings, player, and bed)
    public Vector2 moveDir(Vector2 targetPos) {
        Vector2 currentTarget = targetPos;
        if (path == null) {
            GetPath(targetPos);
        } else if(path.Count >= 1){
            if (Vector2.Distance(transform.position, currentTarget) < .5f) {
                path.Remove(path[0]);
            }
            currentTarget = path[0].worldPosition;
        } else {
            GetPath(targetPos);
        }

        Vector2 dir = currentTarget - new Vector2(transform.position.x, transform.position.y);
        dir.Normalize();

        return dir;

        /*
        switch (priority) {
            case entityPriority.Nearest:
                return (insert A* func for nearest etc etc
        }
         */
    }


    public void damagePlayer(float dmg, GameObject self) {
        ph.TakeDamage(dmg, self);
    }
}

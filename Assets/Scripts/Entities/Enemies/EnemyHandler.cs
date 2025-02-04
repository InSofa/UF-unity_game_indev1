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

    Vector2 currentTarget;

    //Not used yet
    [SerializeField]
    entityPriority priority;

    private Pathfinding pathfinder;

    public List<SimpleNode> path;
    public bool refreshPath = false;

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
        if (path == null || refreshPath) {
            currentTarget = targetPos;
            GetPath(targetPos);
            refreshPath = false;
        } else if(path.Count >= 1){
            //The deltatime is added to make sure that low end devices don't break the pathfinding
            if (Vector2.Distance(transform.position, currentTarget) < .5f * (Time.deltaTime * 60)) {
                path.Remove(path[0]);
            }
            currentTarget = path[0].worldPosition;
        } else {
            GetPath(targetPos);
        }

        Vector2 dir = currentTarget - new Vector2(transform.position.x, transform.position.y);
        dir.Normalize();
        print(dir);

        return dir;

        /*
        switch (priority) {
            case entityPriority.Nearest:
                return (insert A* func for nearest etc etc
        }
         */
    }

    public void OnDrawGizmos() {
        Gizmos.color = Color.red;
        if(currentTarget != null)
            Gizmos.DrawWireSphere(currentTarget, 0.5f);
    }


    public void damagePlayer(float dmg, GameObject self) {
        ph.TakeDamage(dmg, self);
    }

    public void damageBuilding(int dmg) {
        print("Dealing damage to building");
        if (path.Count >= 1 && path[0].building != null) {
            var building = path[0].building;
            bool destroyed = building.GetComponent<BuildingHealth>().TakeDamage(dmg);
            if (destroyed) {
                //For some reason the building is seen as readonly? check later, (nulling the building just in case it causes issues)
                var node = path[0];
                node.building = null;
                path[0] = node;
            }
        }
    }
}

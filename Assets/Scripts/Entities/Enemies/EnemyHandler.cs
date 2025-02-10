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

    public float newNodeDistance = 0.5f;

    [SerializeField]
    private Vector2 dir = Vector2.right;

    [SerializeField]
    private float attackDistance, attackRadius;

    //Buildings & player usually
    private LayerMask attackLayers;

    private float timeSinceMove;
    private Vector2 lastPos = Vector2.zero;

    public void Start() {
        if (player == null || ph == null) {
            player = GameObject.Find("Player");
            ph = player.GetComponent<PlayerHealth>();
        }

        PathfindingGrid.instance.enemies.Add(this);
        pathfinder = Pathfinding.instance;
    }
    IEnumerator GetPath(Vector2 targetPos) {
        Debug.Log("Getting path");
        path = pathfinder.FindPath(transform.position, targetPos).ToList();
        Debug.Log("Path found " + path.Count);
        yield return path;
    }

    //A* pathfinding funtionality, finding the right path depending on target priority(buildings, player, and bed)
    //Implement A* pathfinding funtionality, finding the right path depending on target priority(buildings, player, and bed)
    public Vector2 moveDir(Vector2 targetPos) {
        Vector2 currentPos = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.x));
        if(currentPos == lastPos) {
            timeSinceMove += Time.deltaTime;
            if (timeSinceMove > 1) {
                refreshPath = true;
                timeSinceMove = 0;
            }
        } else {
            lastPos = currentPos;
            timeSinceMove = 0;
        }

        if (path == null || refreshPath || path.Count <= 0) {
            currentTarget = targetPos;
            StartCoroutine(GetPath(targetPos));
            //GetPath(targetPos);
            refreshPath = false;
        } else{
            //The deltatime is added to make sure that low end devices don't break the pathfinding
            float diff = newNodeDistance * (Time.deltaTime * 144);
            if (Vector2.Distance(transform.position, currentTarget) < diff) {
                path.Remove(path[0]);
            }
            currentTarget = path[0].worldPosition;
        } 

        Vector2 _dir = currentTarget - new Vector2(transform.position.x, transform.position.y);
        _dir.Normalize();

        dir = _dir;
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
        Gizmos.DrawWireSphere((Vector2)transform.position + (dir * attackDistance), attackRadius);
    }


    public bool attack(int dmg) {
        bool hit = false;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll((Vector2)transform.position + (dir * attackDistance), attackRadius);
        foreach (var hitCollider in hitColliders) {
            if (hitCollider.gameObject.tag == "Player" || hitCollider.gameObject.tag == "Bed") {
                ph.TakeDamage(dmg, this.gameObject);
                continue;
            }

            BuildingHealth buildingHp = hitCollider.gameObject.GetComponent<BuildingHealth>();
            if(buildingHp != null) {
                buildingHp.TakeDamage(dmg);
            }

            hit = true;
        }
        return hit;
    }


    //These functions might soon be deprecated due to being replaced by the attack function
    /*
    private void damagePlayer(float dmg, GameObject self) {
    Debug.Log("Dealing damage to player");
    ph.TakeDamage(dmg, self);
    }

    public void damageBuilding(int dmg) {
    print("Dealing damage to building");
    if (path.Count >= 1 && path[0].building != null) {
        var building = path[0].building;
        bool destroyed = building.GetComponent<BuildingHealth>().TakeDamage(dmg);
        if (destroyed) {
            print("Building destroyed");
            //For some reason the building is seen as readonly? check later, (nulling the building just in case it causes issues)
            var node = path[0];
            node.building = null;
            path[0] = node;
        }
    }
    }
    */
}

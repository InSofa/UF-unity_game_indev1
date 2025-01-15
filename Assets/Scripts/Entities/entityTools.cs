using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace entityTools
{
    public class Turret
    {
        public GameObject findTarget(Transform firePoint, float range, LayerMask targetable, string[] prioriyTags = null)
        {
            //Create a system to specifically target some enemies with specific tags

            //Finds all Gameobjects with colliders withing the turrets "range" based on the position of the "firepoint"
            //withing the layer/layers of "targetable"
            Collider2D[] collider = Physics2D.OverlapCircleAll(firePoint.position, range, targetable);

            if (collider.Length == 0)
            {
                return null;
            }

            //Checks which gameObject is closest to the firing point
            Collider2D returnTarget = collider[0];
            float returnTargetDistance = Vector2.Distance(firePoint.position, collider[0].transform.position);

            for (int i = 1; i < collider.Length; i++)
            {
                float colliderDistance = Vector2.Distance(firePoint.position, collider[i].transform.position);
                if (colliderDistance < returnTargetDistance)
                {
                    returnTarget = collider[i];
                    returnTargetDistance = colliderDistance;
                }
            }

            return returnTarget.gameObject;
        }
    }
    public enum entityPriority {
        Nearest = 0,
        Player = 1,
        Buildings = 2,
        Bed = 3
    }

    public class Enemy
    {
        private static GameObject player;
        private static PlayerHealth ph;

        //Not used yet
        private entityPriority priority;

        private GameObject parent;

        public Enemy(entityPriority priority, GameObject parent) {
            if (player == null || ph == null) {
                player = GameObject.Find("Player");
                ph = player.GetComponent<PlayerHealth>();
            }

            this.priority = priority;
            this.parent = parent;
        }

        //A* pathfinding funtionality, finding the right path depending on target priority(buildings, player, and bed)
        //Implement A* pathfinding funtionality, finding the right path depending on target priority(buildings, player, and bed)
        public Vector2 moveDir(Vector2 targetPos) {
            Vector2 dir = targetPos - new Vector2(parent.transform.position.x, parent.transform.position.y);
            dir.Normalize();
            return dir;

            /*
            switch (priority) {
                case entityPriority.Nearest:
                    return (insert A* func for nearest etc etc
            }
             */
        }

        public void damagePlayer(float dmg, GameObject self)
        {
            ph.TakeDamage(dmg, self);
        }
    }
}
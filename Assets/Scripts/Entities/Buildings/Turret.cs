using UnityEngine;

public class Turret {
    public GameObject findTarget(Transform firePoint, float range, LayerMask targetable, string[] prioriyTags = null) {
        //Create a system to specifically target some enemies with specific tags

        //Finds all Gameobjects with colliders withing the turrets "range" based on the position of the "firepoint"
        //withing the layer/layers of "targetable"
        Collider2D[] collider = Physics2D.OverlapCircleAll(firePoint.position, range, targetable);

        if (collider.Length == 0) {
            return null;
        }

        //Checks which gameObject is closest to the firing point
        Collider2D returnTarget = collider[0];
        float returnTargetDistance = Vector2.Distance(firePoint.position, collider[0].transform.position);

        for (int i = 1; i < collider.Length; i++) {
            float colliderDistance = Vector2.Distance(firePoint.position, collider[i].transform.position);
            if (colliderDistance < returnTargetDistance) {
                returnTarget = collider[i];
                returnTargetDistance = colliderDistance;
            }
        }

        return returnTarget.gameObject;
    }
}

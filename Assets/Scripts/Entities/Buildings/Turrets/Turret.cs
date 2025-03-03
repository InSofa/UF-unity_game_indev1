using UnityEngine;

public class Turret {
    public GameObject findTarget(Transform originPosition, float range, LayerMask targetable, string[] prioriyTags = null) {
        //Create a system to specifically target some enemies with specific tags

        //Finds all Gameobjects with colliders withing the turrets "range" based on the position of the "originPosition"
        //withing the layer/layers of "targetable"
        Collider2D[] collider = Physics2D.OverlapCircleAll(originPosition.position, range, targetable);

        if (collider.Length == 0) {
            return null;
        }

        //Checks which gameObject is closest to the firing point
        Collider2D returnTarget = collider[0];
        float returnTargetDistance = Vector2.Distance(originPosition.position, collider[0].transform.position);

        for (int i = 1; i < collider.Length; i++) {
            float colliderDistance = Vector2.Distance(originPosition.position, collider[i].transform.position);
            if (colliderDistance < returnTargetDistance) {
                returnTarget = collider[i];
                returnTargetDistance = colliderDistance;
            }
        }

        return returnTarget.gameObject;
    }

    public Vector2 predictMovement(Transform originPosition, GameObject target, float maxPredict, float projectileSpeed) {
        Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
        if (targetRb != null) {
            Vector2 targetPos = target.transform.position;
            Vector2 targetVel = targetRb.linearVelocity;
            float time = Vector2.Distance(originPosition.position, targetPos) / projectileSpeed;

            Vector2 prediction = targetVel * time;
            if(prediction.magnitude > maxPredict) {
                prediction = prediction.normalized * maxPredict;
            }

            return targetPos + prediction;
        }

        return target.transform.position;
    }

    public Vector2 predictMovementWithTime(GameObject target, float travelTime, float maxPredict) {
        Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>();
        if (targetRb != null) {
            Vector2 targetPos = target.transform.position;
            Vector2 targetVel = targetRb.linearVelocity;

            Vector2 prediction = targetVel * travelTime;
            if (prediction.magnitude > maxPredict) {
                prediction = prediction.normalized * maxPredict;
            }


            return targetPos + prediction;
        }

        return target.transform.position;
    }

    public bool lerpPivot(Transform pivot, Vector2 targetDir, float lerpSpeed, float shootLerpDiff, float minDiff) {


        //Normalization might be reduntant but it's here to make sure the values are correct
        float diff = Vector2.Distance(pivot.right.normalized, targetDir.normalized);
        //Debug.Log($"Diff: {diff}  shootLerpDiff: {shootLerpDiff}");

        if(diff > minDiff * Time.deltaTime * 144) {
            pivot.right = Vector2.Lerp(pivot.right, targetDir, lerpSpeed * Time.deltaTime);
            if(diff > shootLerpDiff * Time.deltaTime * 144) {
                return false;
            }
        }
        return true;
    }
}

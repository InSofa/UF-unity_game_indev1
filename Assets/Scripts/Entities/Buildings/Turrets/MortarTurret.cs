using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarTurret : MonoBehaviour {
    Turret turret;
    BuildingScriptableObject bs;

    float range, shotCD, travelTime, maxPredict;
    int damage;

    [SerializeField]
    float lerpSpeed;

    [SerializeField]
    [Range(0f, .5f)]
    [Tooltip("The distance when the turret decides its accurate enough and stops trying to adjust")]
    float minLerpDiff;

    [SerializeField]
    [Range(0f, 2f)]
    [Tooltip("The max distance between pointing towards the target and current angle while still choosing to shoot")]
    float shootLerpDiff;

    float shotCDTime;

    [SerializeField]
    GameObject projectile;

    GameObject target;

    [SerializeField]
    Transform firePoint;

    [SerializeField]
    Transform pivot;

    [SerializeField]
    LayerMask targetable;

    // Start is called before the first frame update
    void Start() {
        turret = new Turret();

        bs = GetComponent<BuildingHealth>().buildingScriptableObject;
        range = bs.buildingRange;
        damage = bs.buildingDamage;
        shotCD = bs.shotCD;
        travelTime = bs.projectileSpeed;
        maxPredict = bs.maxPredict;
    }

    // Update is called once per frame
    void Update() {
        target = turret.findTarget(firePoint, range, targetable);

        shotCDTime += Time.deltaTime;
        if (target != null) {
            //Gets the direction in the form of a Vector2 and lerps it to that direction

            Vector2 prediction = target.transform.position;
            if (maxPredict != 0) {
                prediction = turret.predictMovement(firePoint, target, maxPredict, travelTime);
            }


            Vector2 direction = prediction - (Vector2)firePoint.position;

            bool shoot = turret.lerpPivot(pivot, direction, lerpSpeed, shootLerpDiff, minLerpDiff);

            if (shotCDTime >= shotCD && shoot) {
                shootTarget(target);
                shotCDTime = 0;
            }

        }
    }

    private void shootTarget(GameObject target) {
        if (target == null) {
            return;
        }

        //Creates the projectile and destroys it after 5 seconds to make sure the instance isn't left eventually affecting performance
        GameObject local_projectile = Instantiate(projectile, firePoint.position, firePoint.rotation);
        Destroy(local_projectile, 5);

        MortarProjectile mortarProjectile = local_projectile.GetComponent<MortarProjectile>();
        mortarProjectile.damage = damage;
        mortarProjectile.travelTime = travelTime;
        mortarProjectile.targetPos = turret.predictMovementWithTime(target, travelTime, maxPredict);
        mortarProjectile.hitMask = targetable;

        mortarProjectile.init();
    }
}

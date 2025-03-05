using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HailTurret : MonoBehaviour {
    Turret turret;
    BuildingScriptableObject bs;

    float range, shotCD, projectileSpeed, maxPredict;
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
    GameObject[] projectile;

    GameObject target;

    [SerializeField]
    Transform[] firePoint;

    [SerializeField]
    Transform pivot;

    [SerializeField]
    LayerMask targetable;

    [Header("Effects")]
    [SerializeField]
    GameObject muzzleFlash;

    // Start is called before the first frame update
    void Start() {
        turret = new Turret();

        bs = GetComponent<BuildingHealth>().buildingScriptableObject;
        range = bs.buildingRange;
        damage = bs.buildingDamage;
        shotCD = bs.shotCD;
        projectileSpeed = bs.projectileSpeed;
        maxPredict = bs.maxPredict;
    }

    // Update is called once per frame
    void Update() {
        target = turret.findTarget(transform, range, targetable);

        shotCDTime += Time.deltaTime;
        if (target != null) {
            //Gets the direction in the form of a Vector2 based on a prediction and lerps it to that direction

            Vector2 prediction = target.transform.position;
            if (maxPredict != 0) {
                prediction = turret.predictMovement(firePoint[0], target, maxPredict, projectileSpeed);
            }

            Vector2 direction = prediction - (Vector2)firePoint[0].position;

            bool shoot = turret.lerpPivot(pivot, direction, lerpSpeed, shootLerpDiff, minLerpDiff);

            if (shotCDTime >= shotCD && shoot) {
                shootTarget();
                shotCDTime = 0;
            }
        }
    }

    private void shootTarget() {
        if (muzzleFlash != null) {
            if(muzzleFlash.gameObject.scene.name == null) {
                GameObject flash = Instantiate(muzzleFlash, firePoint[0].position, firePoint[0].rotation);
            } else {
                muzzleFlash.GetComponent<ParticleSystem>().Play();
            }
        }

        foreach (Transform point in firePoint) {
            if(point == null) {
                continue;
            }

            shootProjectile(point);
        }
    }

    private void shootProjectile(Transform point) {
        Quaternion randomAngle = Quaternion.Euler(new Vector3(0,0, Random.Range(0,90)));
        GameObject local_projectile = Instantiate(projectile[Random.Range(0, projectile.Length)], point.position, randomAngle);
        Destroy(local_projectile, 5);

        Rigidbody2D local_projectileRb = local_projectile.GetComponent<Rigidbody2D>();
        local_projectileRb.AddForce(point.right * projectileSpeed, ForceMode2D.Impulse);

        TurretProjectile tp = local_projectile.GetComponent<TurretProjectile>();
        tp.damage = damage;
        tp.init = true;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicTurret : MonoBehaviour
{
    Turret turret;
    BuildingScriptableObject bs;

    float range, shotCD, projectileSpeed, maxPredict;
    int damage;

    [SerializeField]
    float lerpSpeed;

    [SerializeField]
    [Range(0f, 5f)]
    float resetLerpDiff, shootLerpDiff;


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

    [Header("Effects")]
    [SerializeField]
    GameObject muzzleFlash;

    // Start is called before the first frame update
    void Start()
    {
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
            //Gets the direction in the form of a Vector2 and lerps it to that direction

            Vector2 prediction = target.transform.position;
            if (maxPredict != 0) {
                prediction = turret.predictMovement(firePoint, target, maxPredict, projectileSpeed);
            } 

            Vector2 direction = prediction - (Vector2)firePoint.position;
            pivot.right = Vector2.Lerp(pivot.right, direction, lerpSpeed * Time.deltaTime);
            //pivot.right = target.transform.position - firePoint.position;

            float diff = Vector2.Distance(pivot.right, direction);
            //Debug.Log($"Diff: {diff}  shootLerpDiff: {shootLerpDiff}");

            if (shotCDTime >= shotCD && diff <= shootLerpDiff) {
                shootTarget();
                shotCDTime = 0;
            }

        } else {
            if(muzzleFlash != null) {
                if (muzzleFlash.gameObject.scene.name != null) {
                    muzzleFlash.GetComponent<ParticleSystem>().Stop();
                }
            }
        }
    }

    private void shootTarget()
    {
        if (muzzleFlash != null) {
            if (muzzleFlash.gameObject.scene.name == null) {
                GameObject flash = Instantiate(muzzleFlash, firePoint.position, firePoint.rotation);
            } else {
                muzzleFlash.GetComponent<ParticleSystem>().Play();
            }
        }

        //Creates the projectile and destroys it after 5 seconds to make sure the instance isn't left eventually affecting performance
        GameObject local_projectile = Instantiate(projectile, firePoint.position, firePoint.rotation);
        Destroy(local_projectile, 5);

        //Launches the projectile towards the target
        Rigidbody2D local_projectileRb = local_projectile.GetComponent<Rigidbody2D>();
        local_projectileRb.AddForce(firePoint.right * projectileSpeed, ForceMode2D.Impulse);

        //Sets the damage of the projectile
        TurretProjectile tp = local_projectile.GetComponent<TurretProjectile>();
        tp.damage = damage;
        tp.init = true;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}

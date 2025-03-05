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

    [Header("Effects")]
    [SerializeField]
    GameObject muzzleFlash;

    [SerializeField]
    Sprite shotSprite;
    Sprite originalSprite;

    [SerializeField]
    SpriteRenderer sr;

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

        originalSprite = sr.sprite;
    }

    // Update is called once per frame
    void Update() {
        target = turret.findTarget(transform, range, targetable);

        shotCDTime += Time.deltaTime;
        if (shotCDTime > shotCD * .4f) {
            sr.sprite = originalSprite;
        }

        if (target != null) {
            //Gets the direction in the form of a Vector2 and lerps it to that direction

            Vector2 prediction = target.transform.position;
            if (maxPredict != 0) {
                prediction = turret.predictMovement(firePoint, target, maxPredict, projectileSpeed);
            } 


            Vector2 direction = prediction - (Vector2)firePoint.position;

            bool shoot = turret.lerpPivot(pivot, direction, lerpSpeed, shootLerpDiff, minLerpDiff);



            if (shotCDTime >= shotCD && shoot) {
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
        Quaternion projectileAngle = Quaternion.Euler(0, 0, firePoint.eulerAngles.z + projectile.transform.eulerAngles.z);
        GameObject local_projectile = Instantiate(projectile, firePoint.position, projectileAngle);
        Destroy(local_projectile, 5);

        //Launches the projectile towards the target
        Rigidbody2D local_projectileRb = local_projectile.GetComponent<Rigidbody2D>();
        local_projectileRb.AddForce(firePoint.right * projectileSpeed, ForceMode2D.Impulse);

        //Sets the damage of the projectile
        TurretProjectile tp = local_projectile.GetComponent<TurretProjectile>();
        tp.damage = damage;
        tp.init = true;

        if(shotSprite != null) {
            sr.sprite = shotSprite;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}

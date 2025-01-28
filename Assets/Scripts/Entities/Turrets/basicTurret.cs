using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using entityTools;

public class basicTurret : MonoBehaviour
{
    Turret turret;

    [SerializeField]
    float range, projectileSpeed, shotCD, lerpSpeed;

    [SerializeField]
    [Range(0f, .5f)]
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

    // Start is called before the first frame update
    void Start()
    {
        turret = new Turret();
    }

    // Update is called once per frame
    void Update() {
        target = turret.findTarget(firePoint, range, targetable);

        shotCDTime += Time.deltaTime;
        if (target != null) {
            //Gets the direction in the form of a Vector2 and lerps it to that direction
            Vector2 direction = target.transform.position - firePoint.position;
            pivot.right = Vector2.Lerp(pivot.right, direction, lerpSpeed * Time.deltaTime);
            //pivot.right = target.transform.position - firePoint.position;

            float diff = Vector2.Distance(pivot.right, direction);
            Debug.Log(diff);

            if (shotCDTime >= shotCD && diff > shootLerpDiff) {
                shootTarget();
                shotCDTime = 0;
            }

        } else {
            if (Vector2.Distance(pivot.right, Vector2.right) > resetLerpDiff) {
                pivot.right = Vector2.Lerp(pivot.right, Vector2.right, lerpSpeed * Time.deltaTime);
            } else {
                pivot.right = Vector2.right;
            }
        }
    }

    private void shootTarget()
    {
        //Creates the projectile and destroys it after 5 seconds to make sure the instance isn't left eventually affecting performance
        GameObject local_projectile = Instantiate(projectile, firePoint.position, Quaternion.identity);
        Destroy(local_projectile, 5);

        //Launches the projectile towards the target
        Rigidbody2D local_projectileRb = local_projectile.GetComponent<Rigidbody2D>();
        local_projectileRb.AddForce(firePoint.right * projectileSpeed, ForceMode2D.Impulse);
    }
}

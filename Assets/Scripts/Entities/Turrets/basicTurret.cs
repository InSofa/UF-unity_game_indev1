using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using entityTools;

public class basicTurret : MonoBehaviour
{
    Turret turret;

    [SerializeField]
    float range, projectileSpeed, shotCD;

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
        if (shotCDTime >= shotCD) {
            shotCDTime = 0;
            if (target != null) {
                //Gets the direction in the form of a Vector2 and sets the pivot to that direction
                pivot.right = target.transform.position - firePoint.position;
                if (shotCDTime >= shotCD) {
                    shootTarget();
                    shotCDTime = 0;
                }

            } else {
                pivot.right = Vector2.right;
            }

        }
    }

    private void shootTarget()
    {
        if (target != null) {
            pivot.right = target.transform.position - firePoint.position;
        }

        //Creates the projectile and destroys it after 5 seconds to make sure the instance isn't left eventually affecting performance
        GameObject local_projectile = Instantiate(projectile, firePoint.position, Quaternion.identity);
        Destroy(local_projectile, 5);

        //Launches the projectile towards the target
        Rigidbody2D local_projectileRb = local_projectile.GetComponent<Rigidbody2D>();
        local_projectileRb.AddForce(firePoint.right * projectileSpeed, ForceMode2D.Impulse);
    }
}

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
    LayerMask targetable;

    // Start is called before the first frame update
    void Start()
    {
        turret = new Turret();
    }

    // Update is called once per frame
    void Update()
    {
        target = turret.findTarget(firePoint, range, targetable);

        shotCDTime += Time.deltaTime;
        if (target != null && shotCDTime >= shotCD)
        {
            shootTarget();
            shotCDTime = 0;
        }
    }

    private void shootTarget()
    {
        //Gets the direction in the form of a Vector2
        Vector2 targetDirection = target.transform.position - firePoint.position;

        //Normalizes the direction to a magnitude of 1
        targetDirection.Normalize();

        //Creates the projectile and destroys it after 5 seconds to make sure the instance isn't left eventually affecting performance
        GameObject local_projectile = Instantiate(projectile, firePoint.position, Quaternion.identity);
        Destroy(local_projectile, 5);

        //Launches the projectile towards the target
        Rigidbody2D local_projectileRb = local_projectile.GetComponent<Rigidbody2D>();
        local_projectileRb.AddForce(targetDirection * projectileSpeed, ForceMode2D.Impulse);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepDartAbility : Ability
{
    [SerializeField] GameObject projectile;
    [SerializeField] Transform projectileSpawn;
    [SerializeField] float speed;
    [SerializeField] ForceMode mode;

    public override void DeactivateAbility()
    {

    }

    public override void UseAbility()
    {
        GameObject proj = Instantiate(projectile, projectileSpawn.position, projectileSpawn.rotation * Quaternion.AngleAxis(-90, transform.up));
        proj.GetComponent<Rigidbody>().AddForce(proj.transform.right * speed, mode);
    }
}

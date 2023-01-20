using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeBombAbility : Ability
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
        GameObject proj = Instantiate(projectile, projectileSpawn.position, projectileSpawn.rotation);
        proj.GetComponent<Rigidbody>().AddForce(proj.transform.forward * speed, mode);
        proj.GetComponent<SmokeBomb>().startSpeed = speed;
    }
}

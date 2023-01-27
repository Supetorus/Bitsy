using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPAbility : Ability
{
    [SerializeField] GameObject projectile;
    [SerializeField] Transform projectileSpawn;
    [SerializeField] ForceMode mode;
    [SerializeField] float speed;
	[SerializeField] public float maxAmmo;
	[SerializeField] public float currentAmmo { get{ return currentAmmo; } set { currentAmmo = Mathf.Clamp(value, 0, maxAmmo); } }

	public void Start()
	{
		currentAmmo = maxAmmo;
	}

    public override void UseAbility()
    {
		if(currentAmmo > 0)
		{
			GameObject proj = Instantiate(projectile, projectileSpawn.position, projectileSpawn.rotation);
			proj.GetComponent<Rigidbody>().AddForce(proj.transform.forward * speed, mode);
			proj.GetComponent<Bomb>().startSpeed = speed;
			currentAmmo--;
		}
    }
	public override void DeactivateAbility()
    {

    }
}

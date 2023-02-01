using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPAbility : Ability
{
    [SerializeField] GameObject projectile;
    [SerializeField] Transform projectileSpawn;
    [SerializeField] ForceMode mode;
    [SerializeField] float speed;
	[SerializeField] public int maxAmmo;
	[SerializeField] public int currentAmmo;

	public void Start()
	{
		maxAmmo *= PlayerPrefs.GetInt("EMP_AM");
		currentAmmo = maxAmmo;
	}

	

    public override void UseAbility()
    {
		if(currentAmmo > 0)
		{
			GameObject proj = Instantiate(projectile, projectileSpawn.position, projectileSpawn.rotation);
			proj.GetComponent<Rigidbody>().AddForce(proj.transform.forward * speed, mode);
			proj.GetComponent<Bomb>().isEMP = true;
			currentAmmo--;
		}
    }
	public override void DeactivateAbility()
    {

    }
}

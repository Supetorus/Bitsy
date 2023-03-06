using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrojanDartAbility : Ability
{
    [SerializeField] GameObject projectile;
    [SerializeField] Transform projectileSpawn;
    [SerializeField] ForceMode mode;
    [SerializeField] float speed;
	[SerializeField] public int maxAmmo;
	//[SerializeField] public int currentAmmo;
	const int DEFAULT_AMMO = 5;
	private ThirdPersonCameraController myCamera;

	public void Start()
	{
		myCamera = GetComponent<StateData>().camera.GetComponent<ThirdPersonCameraController>();
		UpdateAmmo();
	}

	public void UpdateAmmo()
	{
		maxAmmo = DEFAULT_AMMO * PlayerPrefs.GetInt("TD_AM");
		currentAmmo = maxAmmo;
	}

	public override void UseAbility()
    {
		if(currentAmmo > 0)
		{
			GameObject proj = Instantiate(projectile, projectileSpawn.position, myCamera.transform.rotation * Quaternion.AngleAxis(-90, transform.up));
			proj.GetComponent<Rigidbody>().AddForce(proj.transform.right * speed, mode);
			currentAmmo--;
		}
    }
}

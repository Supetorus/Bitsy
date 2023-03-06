using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPAbility : Ability
{
	[SerializeField] List<GameObject> projectiles;
    [SerializeField] Transform projectileSpawn;
    [SerializeField] ForceMode mode;
    [SerializeField] float speed;
	[SerializeField] public int maxAmmo;
	//[SerializeField] public int currentAmmo;
	const int DEFAULT_AMMO = 5;
	GameObject projectile;
	private ThirdPersonCameraController myCamera;

	public void Start()
	{
		myCamera = GetComponent<StateData>().camera.GetComponent<ThirdPersonCameraController>();
		UpdateAmmo();
	}

	public void UpdateAmmo()
	{
		maxAmmo = DEFAULT_AMMO * PlayerPrefs.GetInt("EMP_AM");
		currentAmmo = maxAmmo;
	}

	public override void UseAbility()
    {
		if (PlayerPrefs.GetString("EMP_DU") == "True" && PlayerPrefs.GetString("EMP_RU") == "True") projectile = projectiles[2];
		else if (PlayerPrefs.GetString("EMP_RU") == "True" || PlayerPrefs.GetString("EMP_DU") == "True") projectile = projectiles[1];
		else projectile = projectiles[0];


		if(currentAmmo > 0)
		{
			GameObject proj = Instantiate(projectile, projectileSpawn.position, myCamera.transform.rotation);
			proj.GetComponent<Rigidbody>().AddForce(proj.transform.forward * speed, mode);
			proj.GetComponent<Bomb>().isEMP = true;
			currentAmmo--;
		}
    }
}

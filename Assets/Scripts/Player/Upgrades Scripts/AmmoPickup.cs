using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
	[SerializeField] int ammoCount;
	public enum AMMO_TYPE
	{
		DART,
		SMOKE,
		EMP,
	}

	[SerializeField] AMMO_TYPE ammoType;

	public void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.TryGetComponent<AbilityController>(out _))
		{
			switch(ammoType)
			{
				case AMMO_TYPE.DART:
					other.gameObject.TryGetComponent<TrojanDartAbility>(out TrojanDartAbility dart);
					if (dart.currentAmmo + ammoCount < dart.maxAmmo) dart.currentAmmo += ammoCount;
					else dart.currentAmmo = dart.maxAmmo;
					break;
				case AMMO_TYPE.SMOKE:
					other.gameObject.TryGetComponent<SmokeBombAbility>(out SmokeBombAbility smoke);
					if (smoke.currentAmmo + ammoCount < smoke.maxAmmo) smoke.currentAmmo += ammoCount;
					else smoke.currentAmmo = smoke.maxAmmo;
					break;
				case AMMO_TYPE.EMP:
					other.gameObject.TryGetComponent<EMPAbility>(out EMPAbility emp);
					if (emp.currentAmmo + ammoCount < emp.maxAmmo) emp.currentAmmo += ammoCount;
					else emp.currentAmmo = emp.maxAmmo;
					break;
			}
			Destroy(gameObject);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
	[SerializeField] float ammoCount;
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
					dart.currentAmmo += ammoCount;
					break;
				case AMMO_TYPE.SMOKE:
					other.gameObject.TryGetComponent<SmokeBombAbility>(out SmokeBombAbility smoke);
					smoke.currentAmmo += ammoCount;
					break;
				case AMMO_TYPE.EMP:
					other.gameObject.TryGetComponent<EMPAbility>(out EMPAbility emp);
					emp.currentAmmo += ammoCount;
					break;
			}

			Destroy(gameObject);
		}
	}
}

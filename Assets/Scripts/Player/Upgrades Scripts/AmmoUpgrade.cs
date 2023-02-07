using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoUpgrade : MonoBehaviour
{
	[SerializeField] AMMO_UPGRADE upgrade;
	[SerializeField] int newValue;
	public enum AMMO_UPGRADE
	{
		SB_AM,
		EMP_AM,
		TD_AM,
	}
	// Start is called before the first frame update
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.TryGetComponent<UpgradeController>(out UpgradeController upgradeController))
		{
			if (PlayerPrefs.HasKey(upgrade.ToString()))
			{
				upgradeController.ChangeAmmoMult(upgrade, newValue);
			}
			TestEffects(other);
			Destroy(gameObject);
		}
	}

	public void TestEffects(Collider other)
	{
		switch (upgrade)
		{
			case AMMO_UPGRADE.SB_AM:
				other.gameObject.TryGetComponent<SmokeBombAbility>(out SmokeBombAbility sba);
				print(sba.maxAmmo);
				print("SB AMMO MULT: " + PlayerPrefs.GetInt("SB_AM"));
				break;
			case AMMO_UPGRADE.EMP_AM:
				other.gameObject.TryGetComponent<EMPAbility>(out EMPAbility empa);
				print(empa.maxAmmo);
				print("EMP AMMO MULT: " + PlayerPrefs.GetInt("EMP_AM"));
				break;
			case AMMO_UPGRADE.TD_AM:
				other.gameObject.TryGetComponent<TrojanDartAbility>(out TrojanDartAbility tda);
				print(tda.maxAmmo);
				print("TD AMMO MULT: " + PlayerPrefs.GetInt("TD_AM"));
				break;
		}
	}
}

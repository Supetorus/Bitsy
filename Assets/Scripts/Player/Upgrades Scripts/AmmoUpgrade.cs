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
			Destroy(gameObject);
		}
	}
}

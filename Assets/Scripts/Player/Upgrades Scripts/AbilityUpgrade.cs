using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityUpgrade : MonoBehaviour
{
	[SerializeField] UPGRADE upgrade;

	public enum UPGRADE
	{
		Lunge,
		WebZip,
		DetectiveMode,
		Hack,
		SB_U,
		EMP_DU,
		EMP_RU,
		TD_U,
		C_U,
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.TryGetComponent<UpgradeController>(out UpgradeController upgradeController))
		{
			if(PlayerPrefs.HasKey(upgrade.ToString()))
			{
				upgradeController.EnableUpgrade(upgrade.ToString());
			}
			Destroy(gameObject);
		}
	}

}

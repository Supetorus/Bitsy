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
		SB_DU,
		EMP_DU,
		EMP_RU,
		TD_U,
		C_DU,
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			if(PlayerPrefs.HasKey(upgrade.ToString()))
			{
				Player.UpgradeController.EnableUpgrade(upgrade.ToString());
			}
			Destroy(gameObject); // todo special effect and sound
		}
	}
}

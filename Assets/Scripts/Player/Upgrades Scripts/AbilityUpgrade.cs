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
		if (other.gameObject.TryGetComponent<UpgradeController>(out UpgradeController upgradeController))
		{
			if(PlayerPrefs.HasKey(upgrade.ToString()))
			{
				upgradeController.EnableUpgrade(upgrade.ToString());
			}
			TestEffects();
			Destroy(gameObject);
		}
	}

	public void TestEffects()
	{
		switch (upgrade)
		{
			case UPGRADE.Lunge:
				print("Lunge On");
				break;
			case UPGRADE.WebZip:
				print("WebZip On");
				break;
			case UPGRADE.DetectiveMode:
				print("Detective Mode On");
				break;
			case UPGRADE.Hack:
				print("Hack On");
				break;
			case UPGRADE.SB_DU:
				print("SB Duration Upgrade ON");
				print("SB DURATION: " + PlayerPrefs.GetInt("SB_DURATION"));
				break;
			case UPGRADE.EMP_DU:
				print("EMP Duration Upgrade ON");
				print("EMP DURATION: " + PlayerPrefs.GetInt("EMP_DURATION"));
				break;
			case UPGRADE.EMP_RU:
				print("EMP Radius Upgrade ON");
				print("EMP RADIUS: " + PlayerPrefs.GetInt("EMP_RADIUS"));
				break;
			case UPGRADE.TD_U:
				print("TD Penetration Upgrade ON");
				print("TD PENETRATION: " + PlayerPrefs.GetString("TD_PENETRATION"));
				break;
			case UPGRADE.C_DU:
				print("Cloak Duration Upgrade ON");
				print("CLOAK DURATION: " + PlayerPrefs.GetInt("C_DURATION"));
				break;
		}
	}

}

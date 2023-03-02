using Michsky.UI.Reach;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetButton : MonoBehaviour
{
   public void ResetPlayerPrefs()
	{
		//Level Lock
		PlayerPrefs.SetInt("LevelLock", 0);
		PlayerPrefs.SetString("TutorialDone", "FALSE");
		PlayerPrefs.SetString("Level1Done", "FALSE");
		PlayerPrefs.SetString("Level2Done", "FALSE");

		//Misc Upgrades
		PlayerPrefs.SetString("Lunge", "False");
		PlayerPrefs.SetString("WebZip", "False");

		//Abiity Upgrades
		PlayerPrefs.SetString("SB_DU", "False");
		PlayerPrefs.SetString("EMP_DU", "False");
		PlayerPrefs.SetString("EMP_RU", "False");
		PlayerPrefs.SetString("TD_U", "False");
		PlayerPrefs.SetString("C_DU", "False");

		PlayerPrefs.SetInt("SB_DURATION", 1);
		PlayerPrefs.SetInt("EMP_DURATION", 1);
		PlayerPrefs.SetInt("EMP_RADIUS", 1);
		PlayerPrefs.SetString("TD_PENETRATION", "FALSE");
		PlayerPrefs.SetInt("C_DURATION", 1);

		//Ammo Upgrades
		PlayerPrefs.SetInt("SB_AM", 1);
		PlayerPrefs.SetInt("EMP_AM", 1);
		PlayerPrefs.SetInt("TD_AM", 1);
	}
}

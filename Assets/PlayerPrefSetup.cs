using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefSetup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		//Level Lock
		if (!PlayerPrefs.HasKey("LevelLock")) PlayerPrefs.SetInt("LevelLock", 1);
		if (!PlayerPrefs.HasKey("TutorialDone")) PlayerPrefs.SetString("TutorialDone", "FALSE");
		if (!PlayerPrefs.HasKey("Level1Done")) PlayerPrefs.SetString("Level1Done", "FALSE");
		if (!PlayerPrefs.HasKey("Level2Done")) PlayerPrefs.SetString("Level2Done", "FALSE");

		//Misc Upgrades
		if(!PlayerPrefs.HasKey("Lunge")) PlayerPrefs.SetString("Lunge", "False");
		if (!PlayerPrefs.HasKey("WebZip")) PlayerPrefs.SetString("WebZip", "False");

		//Abiity Upgrades
		if (!PlayerPrefs.HasKey("SB_DU")) PlayerPrefs.SetString("SB_DU", "False");
		if (!PlayerPrefs.HasKey("EMP_DU")) PlayerPrefs.SetString("EMP_DU", "False");
		if (!PlayerPrefs.HasKey("EMP_RU")) PlayerPrefs.SetString("EMP_RU", "False");
		if (!PlayerPrefs.HasKey("TD_U")) PlayerPrefs.SetString("TD_U", "False");
		if (!PlayerPrefs.HasKey("C_DU")) PlayerPrefs.SetString("C_DU", "False");

		if (!PlayerPrefs.HasKey("SB_DURATION")) PlayerPrefs.SetInt("SB_DURATION", 1);
		if (!PlayerPrefs.HasKey("EMP_DURATION")) PlayerPrefs.SetInt("EMP_DURATION", 1);
		if (!PlayerPrefs.HasKey("EMP_RADIUS")) PlayerPrefs.SetInt("EMP_RADIUS", 1);
		if (!PlayerPrefs.HasKey("TD_PENETRATION")) PlayerPrefs.SetString("TD_PENETRATION", "FALSE");
		if (!PlayerPrefs.HasKey("C_DURATION")) PlayerPrefs.SetInt("C_DURATION", 1);

		//Ammo Upgrades
		if (!PlayerPrefs.HasKey("SB_AM")) PlayerPrefs.SetInt("SB_AM", 1);
		if (!PlayerPrefs.HasKey("EMP_AM")) PlayerPrefs.SetInt("EMP_AM", 1);
		if (!PlayerPrefs.HasKey("TD_AM")) PlayerPrefs.SetInt("TD_AM", 1);
	}
}

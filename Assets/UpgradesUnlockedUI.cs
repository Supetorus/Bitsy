using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradesUnlockedUI : MonoBehaviour
{
	public UpgradeType upgrade;
	[SerializeField] GameObject locked;
	[SerializeField] GameObject unlocked;

	public enum UpgradeType
	{
		SB_AU,
		TD_AU,
		EMP_AU,
		EMP_RU,
		EMP_DU,
		SB_DU,
		C_DU,
		TD_U,
		WZ_O,
		L_O
	}


    // Start is called before the first frame update
    void Start()
    {
		switch (upgrade)
		{
			case UpgradeType.SB_AU:
				if (PlayerPrefs.GetInt("SB_AM") == 1) locked.SetActive(true);
				else
				{
					locked.SetActive(false);
					unlocked.SetActive(true);
				}
				break;
			case UpgradeType.TD_AU:
				if (PlayerPrefs.GetInt("TD_AM") == 1) locked.SetActive(true);
				else
				{
					locked.SetActive(false);
					unlocked.SetActive(true);
				}
				break;
			case UpgradeType.EMP_AU:
				if (PlayerPrefs.GetInt("EMP_AM") == 1) locked.SetActive(true);
				else
				{
					locked.SetActive(false);
					unlocked.SetActive(true);
				}
				break;
			case UpgradeType.EMP_RU:
				if (PlayerPrefs.GetString("EMP_RU") == "False") locked.SetActive(true);
				else
				{
					locked.SetActive(false);
					unlocked.SetActive(true);
				}
				break;
			case UpgradeType.EMP_DU:
				if (PlayerPrefs.GetString("EMP_DU") == "False") locked.SetActive(true);
				else
				{
					locked.SetActive(false);
					unlocked.SetActive(true);
				}
				break;
			case UpgradeType.SB_DU:
				if (PlayerPrefs.GetString("SB_DU") == "False") locked.SetActive(true);
				else
				{
					locked.SetActive(false);
					unlocked.SetActive(true);
				}
				break;
			case UpgradeType.C_DU:
				if (PlayerPrefs.GetString("C_DU") == "False") locked.SetActive(true);
				else
				{
					locked.SetActive(false);
					unlocked.SetActive(true);
				}
				break;
			case UpgradeType.TD_U:
				if (PlayerPrefs.GetString("TD_U") == "False") locked.SetActive(true);
				else
				{
					locked.SetActive(false);
					unlocked.SetActive(true);
				}
				break;
			case UpgradeType.WZ_O:
				if (PlayerPrefs.GetString("WebZip") == "False") locked.SetActive(true);
				else
				{
					locked.SetActive(false);
					unlocked.SetActive(true);
				}
				break;
			case UpgradeType.L_O:
				if (PlayerPrefs.GetString("Lunge") == "False") locked.SetActive(true);
				else
				{
					locked.SetActive(false);
					unlocked.SetActive(true);
				}
				break;
		}
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}

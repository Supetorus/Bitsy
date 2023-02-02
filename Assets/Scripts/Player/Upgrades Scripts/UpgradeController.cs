using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradeController : MonoBehaviour
{
	[Header("Input Actions")]
	//Input Actions
	public InputActionReference lunge;
	public InputActionReference webZip;
	public InputActionReference detectiveMode;
	public InputActionReference hack;

	//MovementController
	[HideInInspector]public MovementController controller;

	[Header("WebZip Settings")]
	//WebZip Variables WZ_ = WebZip
	[SerializeField] float WZ_MaxDist;
	[SerializeField] float WZ_Cooldown;
	[SerializeField] float WZ_Timer;
	[SerializeField] bool WZ_OnCooldown;
	[SerializeField] LayerMask WZ_LayerMask;

	[Header("Lunge Settings")]
	//Lunge Variables L_ = Lunge
	[SerializeField] float L_Force;
	[SerializeField] float L_Cooldown;
	[SerializeField] float L_Timer;
	[HideInInspector] bool L_OnCooldown;

	[Header("Detective Mode Settings")]
	//Detective Mode Variables DM_ = DetectiveMode
	[SerializeField] float range;
	[SerializeField] float DM_Cooldown;
	[SerializeField] float DM_Timer;
	[SerializeField] float DM_Length;
	[HideInInspector] bool DM_OnCooldown;

	[Header("Upgrade Variables")]
	private const int SB_REGULAR_DURATION = 1;
	private const int SB_UPGRADED_DURATION = 2;

	private const int C_REGULAR_DURATION = 1;
	private const int C_UPGRADED_DURATION = 2;

	private const int EMP_REGULAR_RADIUS = 1;
	private const int EMP_UPGRADED_RADIUS = 2;

	private const int EMP_REGULAR_DURATION = 1;
	private const int EMP_UPGRADED_DURATION = 2;


	// Start is called before the first frame update
	void Start()
    {
		//Misc Upgrades
		PlayerPrefs.SetString("Lunge", "False");
		PlayerPrefs.SetString("WebZip", "False");
		PlayerPrefs.SetString("DetectiveMode", "False");
		PlayerPrefs.SetString("Hack", "False");

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

		controller = GetComponent<MovementController>();
		WZ_LayerMask = 3;

		//Setting Timers
		WZ_Timer = WZ_Cooldown;
		L_Timer = L_Cooldown;
		DM_Timer = DM_Cooldown;
    }


	public void Update()
	{ 
		if (lunge.action.ReadValue<float>() > 0 && !L_OnCooldown)
		{
			Lunge();
		}
		else if (webZip.action.ReadValue<float>() > 0 && !WZ_OnCooldown)
		{
			WebZip();
		}
		else if (detectiveMode.action.ReadValue<float>() > 0 && !DM_OnCooldown)
		{
			DetectiveMode();
		} 
		else if (hack.action.ReadValue<float>() > 0 && LookingAtHackable())
		{
			Hack();
		}
		HandleCooldowns();
	}

	public void HandleCooldowns()
	{
		if(L_OnCooldown)
		{
			if (L_Timer >= 0) L_Timer -= Time.deltaTime;
			else
			{
				L_OnCooldown = false;
				L_Timer = L_Cooldown;
			}
		}
		if(WZ_OnCooldown)
		{
			if (WZ_Timer >= 0) WZ_Timer -= Time.deltaTime;
			else
			{
				WZ_OnCooldown = false;
				WZ_Timer = WZ_Cooldown;
			}
		}
		if(DM_OnCooldown)
		{
			if (DM_Timer >= 0) DM_Timer -= Time.deltaTime;
			else
			{
				DM_OnCooldown = false;
				DM_Timer = DM_Cooldown;
			}
		}
	}

	public void WebZip()
	{
		Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
		RaycastHit hit;
		Physics.Raycast(ray, out hit, WZ_MaxDist, WZ_LayerMask);
		if (hit.collider)
		{
			ZipState zip = GetComponent<ZipState>();
			zip.attachedObject = hit;
			controller.CurrentMovementState = zip;
		}
	}

	public void Lunge()
	{
		L_OnCooldown = true;
		print("Lunge");
	}

	public void DetectiveMode()
	{
		DM_OnCooldown = true;
		print("Detective Mode");
	}

	public void Hack()
	{
		print("Elite Hacker: Xx_BlackWillow69_xX@AOL.com");
	}

	public bool LookingAtHackable()
	{
		return false;
	}

	public void EnableUpgrade(string upgrade)
	{
		if(PlayerPrefs.HasKey(upgrade))
		{
			PlayerPrefs.SetString(upgrade, "True");
			switch (upgrade)
			{
				case "Lunge":
					lunge.action.Enable();
					break;
				case "WebZip":
					webZip.action.Enable();
					break;
				case "DetectiveMode":
					detectiveMode.action.Enable();
					break;
				case "Hack":
					hack.action.Enable();
					break;
				case "SB_DU":
					PlayerPrefs.SetInt("SB_DURATION", SB_UPGRADED_DURATION);
					break;
				case "EMP_DU":
					PlayerPrefs.SetInt("EMP_DURATION", EMP_UPGRADED_DURATION);
					break;
				case "EMP_RU":
					PlayerPrefs.SetInt("EMP_RADIUS", EMP_UPGRADED_RADIUS);
					break;
				case "TD_U":
					PlayerPrefs.SetString("TD_PENETRATION", "True");
					break;
				case "C_DU":
					PlayerPrefs.SetInt("C_DURATION", C_UPGRADED_DURATION);
					break;
			}
		}
	}

	public void ChangeAmmoMult(AmmoUpgrade.AMMO_UPGRADE ammoType, int newMult)
	{
		if(PlayerPrefs.HasKey(ammoType.ToString()))
		{
			PlayerPrefs.SetInt(ammoType.ToString(), newMult);
		}
		switch (ammoType)
		{
			case AmmoUpgrade.AMMO_UPGRADE.SB_AM:
				gameObject.TryGetComponent<SmokeBombAbility>(out SmokeBombAbility sba);
				sba.UpdateAmmo();
				break;
			case AmmoUpgrade.AMMO_UPGRADE.EMP_AM:
				gameObject.TryGetComponent<EMPAbility>(out EMPAbility empa);
				empa.UpdateAmmo();
				break;
			case AmmoUpgrade.AMMO_UPGRADE.TD_AM:
				gameObject.TryGetComponent<TrojanDartAbility>(out TrojanDartAbility tda);
				tda.UpdateAmmo();
				break;
		}
	}

	public void DisableUpgrade(string upgrade)
	{
		if (PlayerPrefs.HasKey(upgrade))
		{
			PlayerPrefs.SetString(upgrade, "False");
			switch (upgrade)
			{
				case "Lunge":
					lunge.action.Disable();
					break;
				case "WebZip":
					webZip.action.Disable();
					break;
				case "DetectiveMode":
					detectiveMode.action.Disable();
					break;
				case "Hack":
					hack.action.Disable();
					break;
				case "SB_U":
					PlayerPrefs.SetInt("SB_DURATION", SB_REGULAR_DURATION);
					break;
				case "EMP_DU":
					PlayerPrefs.SetInt("EMP_DURATION", EMP_REGULAR_DURATION);
					break;
				case "EMP_RU":
					PlayerPrefs.SetInt("EMP_RADIUS", EMP_REGULAR_RADIUS);
					break;
				case "TD_U":
					PlayerPrefs.SetString("TD_PENETRATION", "False");
					break;
				case "C_U":
					PlayerPrefs.SetInt("C_DURATION", C_REGULAR_DURATION);
					break;
			}
		}
	}
}

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
	[HideInInspector] bool WZ_OnCooldown;
	[HideInInspector] LayerMask WZ_LayerMask;

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

	[Header("Testing Settings")]
	//Testing Variables
	public bool canLunge;
	public bool canDetectiveMode;
	public bool canWebZip;
	public bool canHack;

	// Start is called before the first frame update
	void Start()
    {
		//Misc Upgrades
		PlayerPrefs.SetString("Lunge", "False");
		PlayerPrefs.SetString("WebZip", "False");
		PlayerPrefs.SetString("DetectiveMode", "False");
		PlayerPrefs.SetString("Hack", "False");
		//Abiity Upgrades
		PlayerPrefs.SetString("SmokeBomb_Upgrade", "False");
		PlayerPrefs.SetString("EMP_Upgrade", "False");
		PlayerPrefs.SetString("TrojanDart_Upgrade", "False");
		PlayerPrefs.SetString("Cloak_Upgrade", "False");
		//Ammo Upgrades
		PlayerPrefs.SetString("SmokeBomb_Ammo_Upgrade", "False");
		PlayerPrefs.SetString("EMP_Ammo_Upgrade", "False");
		PlayerPrefs.SetString("TrojanDart_Ammo_Upgrade", "False");

		
		controller = GetComponent<MovementController>();
		WZ_LayerMask = LayerMask.NameToLayer("Default");

		//Setting Timers
		WZ_Timer = WZ_Cooldown;
		L_Timer = L_Cooldown;
		DM_Timer = DM_Cooldown;
    }


	public void Update()
	{
		if (canLunge && PlayerPrefs.GetString("Lunge") == "False") EnableUpgrade("Lunge");
		if (canWebZip && PlayerPrefs.GetString("WebZip") == "False") EnableUpgrade("WebZip");
		if (canDetectiveMode && PlayerPrefs.GetString("DetectiveMode") == "False") EnableUpgrade("DetectiveMode");
		if (canHack && PlayerPrefs.GetString("Hack") == "False") EnableUpgrade("Hack");

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
			if (L_Timer > 0) L_Timer -= Time.deltaTime;
			else
			{
				L_OnCooldown = false;
				L_Timer = L_Cooldown;
			}
		}
		if(WZ_OnCooldown)
		{
			if (WZ_Timer > 0) WZ_Timer -= Time.deltaTime;
			else
			{
				WZ_OnCooldown = false;
				WZ_Timer = WZ_Cooldown;
			}
		}
		if(DM_OnCooldown)
		{
			if (DM_Timer > 0) DM_Timer -= Time.deltaTime;
			else
			{
				DM_OnCooldown = false;
				DM_Timer = DM_Cooldown;
			}
		}
	}

	public void WebZip()
	{
		WZ_OnCooldown = true;
		Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
		RaycastHit hit;
		Physics.Raycast(ray, out hit, WZ_MaxDist, WZ_LayerMask);
		if (hit.collider)
		{
			print(hit.collider.name);
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
			}
		}
	}

	public void CheckUpgradeEffects(string upgrade)
	{
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
			case "":
				break;
			default:
				break;
		}
	}
}

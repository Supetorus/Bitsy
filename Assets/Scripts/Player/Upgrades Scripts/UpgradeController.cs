using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradeController : MonoBehaviour
{
	//Holds all possible upgrades
	Dictionary<string, bool> upgrades = new Dictionary<string, bool>();

	//Input Actions
	public InputActionReference lunge;
	public InputActionReference webZip;
	public InputActionReference detectiveMode;

	//MovementController
	[HideInInspector]public MovementController controller;

	//WebZip Variables
	[SerializeField] float maxZipDistance;
	[SerializeField] LayerMask myLayerMask;

	//Testing Variables
	public bool canLunge;
	public bool canDetectiveMode;
	public bool canWebZip;

	// Start is called before the first frame update
	void Start()
    {
		controller = GetComponent<MovementController>();
		upgrades.Add("Lunge", false);
		upgrades.Add("WebZip", false);
		upgrades.Add("DetectiveMode", false);
		upgrades.Add("SensorHack", false);
		upgrades.Add("SmokeBomb_Upgrade", false);
		upgrades.Add("SmokeBombAmmo_Upgrade", false);
		upgrades.Add("EMP_Upgrade", false);
		upgrades.Add("EMP_UpgradeAmmo", false);
		upgrades.Add("SleepDart_Upgrade", false);
		upgrades.Add("SleepDartAmmo_Upgrade", false);
		upgrades.Add("Cloak_Upgrade", false);
    }


	public void Update()
	{
		if (canLunge && !upgrades["Lunge"]) EnableUpgrade("Lunge");
		if (canWebZip && !upgrades["WebZip"]) EnableUpgrade("WebZip");
		if (canDetectiveMode && !upgrades["DetectiveMode"]) EnableUpgrade("DetectiveMode");


		if(lunge.action.ReadValue<float>() > 0)
		{
			Lunge();
		}
		else if(webZip.action.ReadValue<float>() > 0)
		{
			WebZip();
		}
		else if(detectiveMode.action.ReadValue<float>() > 0)
		{
			DetectiveMode();
		}
	}

	public void WebZip()
	{
		Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
		RaycastHit hit;
		Physics.Raycast(ray, out hit, maxZipDistance, myLayerMask);
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
		print("Lunge");
	}

	public void DetectiveMode()
	{
		print("Detective Mode");
	}

	public void EnableUpgrade(string upgrade)
	{
		if(upgrades.ContainsKey(upgrade))
		{
			upgrades[upgrade] = true;
		}

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
			default:
				break;
		}
	}

	public void DisableUpgrade(string upgrade)
	{
		if (upgrades.ContainsKey(upgrade))
		{
			upgrades[upgrade] = false;
		}
	}
}

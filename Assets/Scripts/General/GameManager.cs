using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public GameObject player;
	public GameObject hud;
	public GameObject mainMenu;
	public GameObject menuCamera;
	public GameObject playCamera;

	private bool isInstantiated = false;

	public void Register(string registrationName, GameObject o)
	{
		switch (registrationName.ToLower())
		{
			case "player": player = o; break;
			case "hud": hud = o; break;
			case "mainmenu": mainMenu = o; break;
			case "menucamera": menuCamera = o; break;
			case "playcamera": playCamera = o; break;
			default:
				Debug.LogError(o.name + " attempted to register with Game Manager, but failed to do so. Registration name given was: " + registrationName);
				break;
		}
	}

	private void Start()
	{
		if (isInstantiated)
		{
			Debug.LogError("There are two Game Managers in the scene. This should never happen.");
		}
		isInstantiated = true;
		DontDestroyOnLoad(this);
	}
}

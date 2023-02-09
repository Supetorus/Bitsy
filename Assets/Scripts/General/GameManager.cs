using Michsky.UI.Reach;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public GameObject player;
	public GameObject hud;
	public GameObject mainMenu;
	public GameObject menuCamera;
	public GameObject playCamera;

	private PanelManager panelManager;
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

	public void OnFailLevel()
	{
		hud.gameObject.SetActive(false);
		mainMenu.gameObject.SetActive(true);

		panelManager = FindObjectOfType<PanelManager>();
		panelManager.OpenPanel(panelManager.panels[7].panelName);

		//menuManager.ActivateMenu();
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		menuCamera.SetActive(true);
		playCamera.SetActive(false);

		Debug.Log("Level Failed");
	}

	public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (!scene.name.Contains("Menu"))
		{
			SceneManager.SetActiveScene(scene);
			mainMenu.SetActive(false);
			menuCamera.SetActive(false);
			playCamera.SetActive(true);
			hud.SetActive(true);

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	public void ReloadCurrentScene()
	{
		//SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
		Scene scene = SceneManager.GetActiveScene();
		SceneManager.UnloadSceneAsync(scene);
		SceneManager.LoadScene(scene.buildIndex, LoadSceneMode.Additive);

		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	public void UnloadCurrentScene()
	{
		Scene scene = SceneManager.GetActiveScene();
		SceneManager.UnloadSceneAsync(scene);
	}
}

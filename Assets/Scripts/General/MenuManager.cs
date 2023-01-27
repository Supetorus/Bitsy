using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
	GameManager gm;
    //ThirdPersonCameraController playerCameraController;

    private void Start()
    {
		gm = FindObjectOfType<GameManager>();
    }

    public void StartGame()
    {
		SceneManager.LoadScene("Objective Test", LoadSceneMode.Additive);
		SceneManager.sceneLoaded += OnSceneLoaded;
    }

	public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		gm.mainMenu.SetActive(false);
		gm.menuCamera.SetActive(false);
		gm.playCamera.SetActive(true);
		gm.hud.SetActive(true);

		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

    public void ActivateMenu()
    {
        gm.hud.gameObject.SetActive(false);
        gm.mainMenu.gameObject.SetActive(true);
    }
}

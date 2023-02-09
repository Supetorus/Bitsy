using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
	GameManager gm;
	private string selectedLevel = "Tutorial";
	public int objectiveIndex;
	[SerializeField] List<Level> levels = new List<Level>();
    //ThirdPersonCameraController playerCameraController;

    private void Start()
    {
		gm = FindObjectOfType<GameManager>();
    }

    public void StartGame()
    {
		if (SceneManager.GetSceneByName(selectedLevel).IsValid())
		{
			SceneManager.UnloadSceneAsync(selectedLevel);
		}
		SceneManager.LoadScene(selectedLevel, LoadSceneMode.Additive);
		SceneManager.sceneLoaded += OnSceneLoaded;
    }

	public void SetLevel(string value)
	{
		selectedLevel = value;
	}

	public void SetObjective(int value) 
	{
		objectiveIndex = value;
	}

	public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		SceneManager.SetActiveScene(scene);
		gm.mainMenu.SetActive(false);
		gm.menuCamera.SetActive(false);
		gm.playCamera.SetActive(true);
		gm.hud.SetActive(true);

		FindObjectOfType<ObjectiveHandler>().objectives.Clear();
		FindObjectOfType<ObjectiveHandler>().objectives.Add(levels.Find(x => x.name == selectedLevel).objectives[objectiveIndex]);
		FindObjectOfType<ObjectiveHandler>().ResetObjective();

		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

    public void ActivateMenu()
    {
        gm.hud.gameObject.SetActive(false);
        gm.mainMenu.gameObject.SetActive(true);
    }
}

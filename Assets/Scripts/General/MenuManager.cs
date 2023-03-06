using Michsky.UI.Reach;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
	GameManager gm;
	[SerializeField]
	private string selectedLevel = "Tutorial";
	public int objectiveIndex;
	[SerializeField] private List<Level> levels = new List<Level>();
	[SerializeField] private GameObject objectivesLayoutGroup;
	[SerializeField] private GameObject buttonPrefab;
	[SerializeField] private GameObject nextParent;

	[Header("Displayed Information")]
	private List<Objective> objectives;
	[SerializeField] private TextMeshProUGUI objectiveName;
	[SerializeField] private TextMeshProUGUI objectiveDescription;

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
		FindObjectOfType<SceneLoader>().Load(selectedLevel, LoadSceneMode.Additive);
		//SceneManager.sceneLoaded += FindObjectOfType<ScreenFade>().OnSceneLoaded;
		SceneManager.sceneLoaded += OnSceneLoaded;
    }

	public void SetLevel(string value)
	{
		selectedLevel = value;

		foreach (Transform child in objectivesLayoutGroup.transform)
		{
			Destroy(child.gameObject);
		}

		objectives = levels.Find(x => x.name == selectedLevel).objectives;

		foreach (Objective objective in objectives)
		{
			GameObject objectiveButton = Instantiate(buttonPrefab, objectivesLayoutGroup.transform);
			objectiveButton.GetComponent<ButtonManager>().SetText(objective.objectiveLabel);
			objectiveButton.GetComponent<ButtonManager>().onClick.AddListener(() => SetObjective(objective.index));
			objectiveButton.GetComponent<ButtonManager>().onClick.AddListener(() => nextParent.GetComponent<UIPopup>().PlayIn());
		}
	}

	public void SetObjective(int value) 
	{
		objectiveIndex = value;

		objectiveName.text = objectives[value].objectiveLabel;
		objectiveDescription.text = objectives[value].objectiveDescription;
	}

	public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		SceneManager.SetActiveScene(scene);
		gm.mainMenu.SetActive(false);
		gm.menuCamera.SetActive(false);
		gm.playCamera.SetActive(true);
		gm.hud.SetActive(true);

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		FindObjectOfType<ObjectiveHandler>().objective = null;
		FindObjectOfType<ObjectiveHandler>().objective = levels.Find(x => x.name == selectedLevel).objectives[objectiveIndex];
		FindObjectOfType<ObjectiveHandler>().ResetObjective();

		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

    public void ActivateMenu()
    {
        gm.hud.gameObject.SetActive(false);
        gm.mainMenu.gameObject.SetActive(true);
    }
}

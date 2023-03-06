using Michsky.UI.Reach;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialGuideTrigger : MonoBehaviour
{
	[SerializeField] GameObject windowToOpen;
	[SerializeField] GameObject tutorialHUD;
	AbilityController player;

	public void Start()
	{
		player = GameObject.Find("Player3.0").GetComponentInChildren<AbilityController>();
		StartCoroutine(WaitForLoad());
	}

	IEnumerator WaitForLoad()
	{
		GetComponent<BoxCollider>().enabled = false;
		yield return new WaitForSeconds(1.5f);
		GetComponent<BoxCollider>().enabled = true;
	}

	// Start is called before the first frame update
	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			player.enabled = false;
			Time.timeScale = 0;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
			tutorialHUD.SetActive(true);
			//windowToOpen.SetActive(true);
			windowToOpen.GetComponent<ModalWindowManager>().OpenWindow();
			Destroy(gameObject);
		}
	}
}

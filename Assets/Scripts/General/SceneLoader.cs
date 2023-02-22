using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	[SerializeField] GameObject loadingUI;
	[SerializeField] Slider loadingMeterUI;
	[SerializeField] ScreenFade screenFade;

	public void Load(string sceneName, LoadSceneMode mode)
	{
		StartCoroutine(LoadScene(sceneName, mode));
	}

	IEnumerator LoadScene(string sceneName, LoadSceneMode mode)
	{
		Time.timeScale = 1;

		// fade out screen
		screenFade.FadeOut();
		yield return new WaitUntil(() => screenFade.isDone);

		// load scene
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);
		asyncOperation.allowSceneActivation = false;
		//Pause.Instance.paused = false;

		// show loading ui
		loadingUI.SetActive(true);

		// update progress meter
		while (asyncOperation.progress < 0.9f)
		{
			loadingMeterUI.value = asyncOperation.progress;
			yield return null;
		}
		loadingMeterUI.value = 1;
		yield return new WaitForSeconds(1);

		// hide loading ui
		loadingUI.SetActive(false);
		
		// scene loaded / start
		asyncOperation.allowSceneActivation = true;

		// fade in screen
		screenFade.FadeIn();
		yield return new WaitUntil(() => screenFade.isDone);
	}
}

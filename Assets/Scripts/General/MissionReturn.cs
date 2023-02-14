using Michsky.UI.Reach;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionReturn : MonoBehaviour
{
    void Start()
    {
		GetComponent<ButtonManager>().onClick.AddListener(FindObjectOfType<GameManager>().UnloadCurrentScene);
	}
}

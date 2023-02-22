using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CursorManager : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

	public void OnSceneLoaded()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

}

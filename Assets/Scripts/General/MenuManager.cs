using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    GameObject playerHud;
    GameObject mainMenu;
    //ThirdPersonCameraController playerCameraController;

    private void Start()
    {
        playerHud = GameObject.FindGameObjectWithTag("HUD");
        mainMenu = GameObject.FindGameObjectWithTag("MainMenu");
        //playerCameraController = GameObject.FindObjectOfType<ThirdPersonCameraController>();
        //playerCameraController.enabled = false;
        playerHud.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
    }

    public void StartGame()
    {
       //playerCameraController.enabled = true;
        playerHud.gameObject.SetActive(true);
        mainMenu.gameObject.SetActive(false);
    }

    public void ActivateMenu()
    {
        playerHud.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
    }
}

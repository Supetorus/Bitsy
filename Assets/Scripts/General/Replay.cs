using Michsky.UI.Reach;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Replay : MonoBehaviour
{
    void Start()
    {
		GetComponent<ButtonManager>().onClick.AddListener(FindObjectOfType<GameManager>().ReloadCurrentScene);
    }
}

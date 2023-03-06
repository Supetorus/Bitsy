using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class InventoryManager : MonoBehaviour
{
	public GameData gameData;
	//[SerializeField] TextMeshProUGUI items;
	//When proper inventory is implemented, use this to display them

    void Start()
    {
		gameData.stringData.Clear();
    }

    void Update()
    {
		//items.text = gameData.stringData.values.ToString();
    }
}

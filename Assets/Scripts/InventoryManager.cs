using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class InventoryManager : MonoBehaviour
{
	public GameData gameData;
	[SerializeField] TextMeshProUGUI items;

    void Start()
    {
        
    }

    void Update()
    {
		items.text = gameData.stringData.values.ToString();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRegistrar : MonoBehaviour
{
	[SerializeField]
	private string registrationName;

	private void Awake()
	{
		GameManager gm = FindObjectOfType<GameManager>();
		if (gm) gm.Register(registrationName, gameObject);
	}
}

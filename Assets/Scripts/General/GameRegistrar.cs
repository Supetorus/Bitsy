using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRegistrar : MonoBehaviour
{
	[SerializeField]
	private string registrationName;

	private void Awake()
	{
		FindObjectOfType<GameManager>().Register(registrationName, gameObject);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	private static Player instance;

	private static DetectionLevel detection;
	public static DetectionLevel Detection
	{
		get
		{
			if (detection == null) detection = instance.GetComponent<DetectionLevel>();
			return detection;
		}
	}

	private static AbilityController abilityController;
	public static AbilityController AbilityController
	{
		get
		{
			if (abilityController == null) abilityController = instance.GetComponent<AbilityController>();
			return abilityController;
		}
	}

	private static Health health;
	public static Health Health
	{
		get
		{
			if (health == null) health = instance.GetComponent<Health>();
			return health;
		}
	}

	public static Transform Transform
	{
		get
		{
			return instance.transform;
		}
	}

	private void Start()
	{
		if (instance == null) instance = this;
		else
		{
			Debug.LogError("There is more than one player in the scene.");
		}
	}
}

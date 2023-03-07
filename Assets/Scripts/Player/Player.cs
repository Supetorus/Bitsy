using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	[SerializeField] private new Camera camera;
	public static Camera Camera
	{
		get
		{
			if (instance.camera == null) Debug.LogError("Player camera was not assigned in Player component.");
			else return instance.camera;
			return null;
		}
	}

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

	private static ObjectiveHandler objectiveHandler;
	public static ObjectiveHandler ObjectiveHandler
	{
		get
		{
			if (objectiveHandler == null) objectiveHandler = instance.GetComponent<ObjectiveHandler>();
			return objectiveHandler;
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

	private static UpgradeController upgradeController;
	public static UpgradeController UpgradeController
	{
		get
		{
			if (upgradeController == null) upgradeController = instance.GetComponent<UpgradeController>();
			return upgradeController;
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

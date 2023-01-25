using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class MovementController : MonoBehaviour
{
	public TMPro.TMP_Text debugText;
	private MovementState currentMovementState;
	public MovementState CurrentMovementState
	{
		get { return currentMovementState; }
		set
		{
			if (value == null)
			{
				Debug.LogError("You cannot set CurrentMovementState to null. You may have forgotten to add a state component.");
				return;
			}
			if (value != null && value != currentMovementState)
			{
				//print(CurrentMState + " : " + newState);
				currentMovementState?.ExitState();
				currentMovementState = value;
				currentMovementState.EnterState();
			}
		}
	}

	public InputActionReference move;
	public InputActionReference sprint;
	public InputActionReference jump;

	[SerializeField, Tooltip("The maximum lateral speed of the spider."), Min(0)]
	public float maxVelocity = 1;
	[SerializeField, Tooltip("Influences how quickly the spider gets to max velocity.")]
	public float acceleration = 1;
	[SerializeField, Tooltip("Influences how quickly the spider slows down when input stops." +
		" Higher numbers apply less drag."), Range(0, 1)]
	public float drag;

	[HideInInspector] public ClingState clingState;
	[HideInInspector] public FallState fallState;
	[HideInInspector] public JumpState jumpState;

	private void Awake()
	{
		clingState = GetComponent<ClingState>();
		fallState = GetComponent<FallState>();
		jumpState = GetComponent<JumpState>();
	}

	private void Start()
	{
		CurrentMovementState = clingState;
	}

	private void Update()
	{
		CurrentMovementState.UpdateState();
		debugText.text = CurrentMovementState.GetType().Name;
	}

	private void FixedUpdate()
	{
		CurrentMovementState.FixedUpdateState();
	}

}

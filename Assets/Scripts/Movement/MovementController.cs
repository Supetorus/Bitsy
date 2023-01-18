using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class MovementController : MonoBehaviour
{
	private MovementState currentMovementState;
	public MovementState CurrentMovementState
	{
		get { return currentMovementState; }
		set
		{
			if (value != null && value != currentMovementState)
			{
				//print(CurrentMState + " : " + newState);
				currentMovementState?.ExitState();
				currentMovementState = value;
				currentMovementState.EnterState();
			}
		}
	}
	[HideInInspector] public bool isGrounded;
	[HideInInspector] public bool wasGrounded;

	public InputActionReference move;
	public InputActionReference sprint;
	public InputActionReference jump;

	private ClingState clingState;

	private void Awake()
	{
		clingState = GetComponent<ClingState>();
	}

	private void Start()
	{
		CurrentMovementState = clingState;
	}

	private void Update()
	{
		CurrentMovementState.UpdateState();
	}

	private void FixedUpdate()
	{
		CurrentMovementState.FixedUpdateState();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(StateData))]
public class MovementState : MonoBehaviour
{

	protected MovementController c;
	protected new Rigidbody rigidbody;
	protected StateData sd;

	private void Awake()
	{
		c = GetComponent<MovementController>();
		rigidbody = GetComponent<Rigidbody>();
		sd = GetComponent<StateData>();
	}

	public virtual void EnterState() { }
	public virtual void ExitState() { }
	public virtual void FixedUpdateState() { }
	public virtual void UpdateState() { }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementState : MonoBehaviour
{
	protected MovementController c;
	protected new Rigidbody rigidbody;

	private void Awake()
	{
		c = GetComponent<MovementController>();
		rigidbody = GetComponent<Rigidbody>();
	}

	public virtual void EnterState() { }
	public virtual void ExitState() { }
	public virtual void FixedUpdateState() { }
	public virtual void UpdateState() { }

}

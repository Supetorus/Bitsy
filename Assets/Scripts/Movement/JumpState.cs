using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : MovementState
{
	[SerializeField, Tooltip("The amount of force applied to the rigidbody when it jumps.")]
	private float jumpForce = 1;

	public override void EnterState()
	{
		rigidbody.isKinematic = false;
		rigidbody.useGravity = true;
		rigidbody.AddForce(transform.rotation * (Vector3.up * jumpForce), ForceMode.Impulse);
		c.CurrentMovementState = c.fallState;
	}
}

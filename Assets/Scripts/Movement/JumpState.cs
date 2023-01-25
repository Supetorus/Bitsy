using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : MovementState
{
	[SerializeField, Tooltip("The amount of force applied to the rigidbody when it jumps."), Range(0.0001f, 10)]
	private float jumpForce = 1;

	public override void EnterState()
	{
		// This cuts out the up and down velocity of the spider, in case it has any. 
		// This fixes the problem that sometimes the spider can jump higher because it
		// tends to bounce when it lands.
		rigidbody.velocity = Vector3.ProjectOnPlane(rigidbody.velocity, transform.up);
		rigidbody.isKinematic = false;
		rigidbody.useGravity = true;
		rigidbody.AddForce(transform.rotation * (Vector3.up * jumpForce), ForceMode.Impulse);
		c.CurrentMovementState = c.fallState;
	}
}

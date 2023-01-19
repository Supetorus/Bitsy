using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : MovementState
{
	public override void EnterState()
	{
		//c.move.action.Enable();
		rigidbody.isKinematic = false;
		rigidbody.useGravity = true;
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
	}

	public override void FixedUpdateState()
	{
		transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up); // Todo this should be a slerp from the original position to facing upright.

		if (sd.GetClosestPoint(sd.attachmentDistance) != null)
		{
			c.CurrentMovementState = c.clingState;
		}
	}

	public override void ExitState()
	{
		//c.move.action.Disable();
	}
}

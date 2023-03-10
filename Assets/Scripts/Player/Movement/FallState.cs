using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : MovementState
{
	public override void EnterState()
	{
		rigidbody.isKinematic = false;
		rigidbody.useGravity = true;
	}

	public override void UpdateState()
	{
		// Todo this should be a slerp from the original position to facing upright,
		// or upright according to where they will land.
		transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
		Vector3? point = sd.GetClosestPoint(sd.lesserAttachmentDistance);
		if (point != null)
		{
			c.CurrentMovementState = c.clingState;
		}
	}
}

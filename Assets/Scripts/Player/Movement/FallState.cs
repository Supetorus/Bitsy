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
		List<RaycastHit> hits = SphereRaycaster.SphereRaycast(transform.position, sd.lesserAttachmentDistance, sd.walkableLayers);
		Vector3? point = SphereRaycaster.GetClosestPoint(hits, transform.position);
		if (point != null)
		{
			c.CurrentMovementState = c.clingState;
		}
	}
}

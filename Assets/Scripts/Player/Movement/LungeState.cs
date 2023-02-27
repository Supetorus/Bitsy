using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LungeState : MovementState
{
	public const float LUNGE_FORCE = 10f;
	[HideInInspector] public Vector3 lungeDirection;

	public override void EnterState()
	{
		rigidbody.isKinematic = false;
		rigidbody.useGravity = true;

		GetComponent<Rigidbody>().AddForce(lungeDirection * LUNGE_FORCE, ForceMode.Impulse);
	}

	public override void UpdateState()
	{
		// Todo this should be a slerp from the original position to facing upright,
		// or upright according to where they will land.
		var hits = SphereRaycaster.SphereRaycast(transform.position, sd.lesserAttachmentDistance, sd.walkableLayers);
		Vector3? point = SphereRaycaster.GetClosestPoint(hits, transform.position);
		if (point != null)
		{
			c.CurrentMovementState = c.clingState;
		}
	}
}
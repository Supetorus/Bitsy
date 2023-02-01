using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LungeState : MovementState
{
    public const float LUNGE_FORCE = 50f;
	[HideInInspector] public Vector3 lungeDirection;

    public override void EnterState()
    {
        rigidbody.isKinematic = false;
        rigidbody.useGravity = false;

        GetComponent<Rigidbody>().AddForce(lungeDirection * LUNGE_FORCE, ForceMode.Impulse);
    }

	public override void ExitState()
	{
		
	}

	public override void UpdateState()
    {
        // Todo this should be a slerp from the original position to facing upright,
        // or upright according to where they will land.
        Vector3? point = sd.GetClosestPoint(sd.lesserAttachmentDistance);
        if (point != null)
        {
            c.CurrentMovementState = c.clingState;
        }
    }
}

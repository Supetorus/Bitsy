using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : MovementState
{
	public override void EnterState()
	{
		c.move.action.Enable();
	}

	public override void ExitState()
	{
		c.move.action.Disable();
	}
}

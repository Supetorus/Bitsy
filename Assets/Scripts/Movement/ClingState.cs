using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClingState : MovementState
{
	public float forwardSpeed = 1;
	public float backSpeed = 1;
	public float sideSpeed = 1;
	public float height = 0.5f;

	//private float checkDistance = 1.5f;

	public override void EnterState()
	{
		c.jump.action.Enable();
		c.sprint.action.Enable();
		c.move.action.Enable();
		rigidbody.isKinematic = true;
		rigidbody.useGravity = false;
	}

	public override void ExitState()
	{
		c.jump.action.Disable();
		c.sprint.action.Disable();
		c.move.action.Disable();
	}

	public override void FixedUpdateState()
	{
		Vector2 input = c.move.action.ReadValue<Vector2>();
		print(input);

		Vector3? closestPoint = sd.GetClosestPoint(sd.groundCheckDistance);

		if (closestPoint != null)
		{
			// Movement
			float distance = ((Vector3)(transform.position - closestPoint)).magnitude;
			float x = input.x * sideSpeed;
			//todo interpolate y to smooth position change.
			float y = height - distance;
			float z = input.y > 0 ? input.y * forwardSpeed : input.y * backSpeed;
			Vector3 direction = transform.rotation * new Vector3(x * Time.fixedDeltaTime, y, z * Time.fixedDeltaTime);

			rigidbody.MovePosition(direction + transform.position);
			//transform.position = (direction + transform.position);

			// Rotation
			// https://discord.com/channels/489222168727519232/885300730104250418/1063576660051501136
			Vector3 up = transform.position - (Vector3)closestPoint;
			Vector3 forward = Vector3.ProjectOnPlane(sd.camera.forward, up);
			rigidbody.MoveRotation(Quaternion.LookRotation(forward, up));
		}
		else
		{
			c.CurrentMovementState = c.fallState;
		}
	}
}

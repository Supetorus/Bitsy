using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClingState : MovementState
{
	[SerializeField, Tooltip("How fast the spider moves forward.")]
	private float forwardSpeed = 1;
	[SerializeField, Tooltip("How fast the spider moves backward.")]
	private float backSpeed = 1;
	[SerializeField, Tooltip("How fast the spider moves sideways.")]
	private float sideSpeed = 1;
	[SerializeField, Tooltip("This defines how far the spider keeps itself above a surface. Adjust this to make the legs line up with the ground. Must be less than the State Data's attachment distance.")]
	private float height = 0.5f;
	[SerializeField]
	private float maxVelocity = 1;
	[SerializeField]
	private float drag;

	private Vector3 velocity;

	public override void EnterState()
	{
		if (c.jump == null) Debug.LogError("Jump action was not assigned.");
		if (c.sprint == null) Debug.LogError("Sprint action was not assigned.");
		if (c.move == null) Debug.LogError("Move action was not assigned.");
		c.jump.action.Enable();
		c.sprint.action.Enable();
		c.move.action.Enable();
		rigidbody.isKinematic = true;
		rigidbody.useGravity = false;
		velocity = Vector3.zero;
	}

	public override void ExitState()
	{
		c.jump.action.Disable();
		c.sprint.action.Disable();
		c.move.action.Disable();
	}

	public override void FixedUpdateState()
	{
		if (height > sd.attachmentDistance)
		{
			Debug.Log("Cling State height cannot be greater than State Data's attachment distance.");
			return;
		}

		// Jump
		if (c.jump.action.ReadValue<float>() > 0)
		{
			c.CurrentMovementState = c.jumpState;
		}

		Vector2 input = c.move.action.ReadValue<Vector2>();
		Vector3? closestPoint = sd.GetClosestPoint(sd.attachmentDistance);
		velocity *= drag;

		if (closestPoint != null)
		{
			// Movement
			float distance = ((Vector3)(transform.position - closestPoint)).magnitude;
			float x = input.x * sideSpeed;
			float y = height - distance;
			float z;
			if (input.y > 0)
			{
				z = input.y * forwardSpeed;
			}
			else
			{
				z = input.y * backSpeed;
			}
			Vector3 direction = transform.rotation * new Vector3(x * Time.fixedDeltaTime, y, z * Time.fixedDeltaTime);
			velocity = Vector3.ClampMagnitude(direction + velocity, maxVelocity);

			rigidbody.MovePosition(velocity + transform.position);

			// Rotation
			// https://discord.com/channels/489222168727519232/885300730104250418/1063576660051501136
			Vector3 up = transform.position - (Vector3)closestPoint;
			Vector3 forward = Vector3.ProjectOnPlane(sd.camera.forward, up);
			Quaternion targetRotation = Quaternion.LookRotation(forward, up);
			Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, Quaternion.Angle(transform.rotation, targetRotation) / 360);
			rigidbody.MoveRotation(finalRotation);
		}
		else
		{ // Not near any walkable surfaces.
			c.CurrentMovementState = c.fallState;
		}
	}
}

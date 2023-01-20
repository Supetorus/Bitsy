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
	[Tooltip("This must be less than the State Data's attachment distance.")]
	public float height = 0.5f;
	public float maxVelocity = 1;
	public float drag;

	private bool isActive = false;
	private Vector3 targetPosition;
	private Vector3 velocity;

	public override void EnterState()
	{
		c.jump.action.Enable();
		c.sprint.action.Enable();
		c.move.action.Enable();
		rigidbody.isKinematic = true;
		rigidbody.useGravity = false;
		isActive = true;
		velocity = Vector3.zero;
	}

	public override void ExitState()
	{
		c.jump.action.Disable();
		c.sprint.action.Disable();
		c.move.action.Disable();
		isActive = false;
	}

	public override void FixedUpdateState()
	{
		if (height > sd.attachmentDistance)
		{
			Debug.Log("Cling State height cannot be greater than State Data's attachment distance.");
			return;
		}

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
			//todo interpolate y to smooth position change.
			float y = height - distance;
			float z = input.y > 0 ? input.y * forwardSpeed : input.y * backSpeed;
			Vector3 direction = transform.rotation * new Vector3(x * Time.fixedDeltaTime, y, z * Time.fixedDeltaTime);
			velocity = Vector3.ClampMagnitude(direction + velocity, maxVelocity);

			targetPosition = velocity + transform.position;
			rigidbody.MovePosition(targetPosition);

			// Rotation
			// https://discord.com/channels/489222168727519232/885300730104250418/1063576660051501136
			Vector3 up = transform.position - (Vector3)closestPoint;
			Vector3 forward = Vector3.ProjectOnPlane(sd.camera.forward, up);
			Quaternion targetRotation = Quaternion.LookRotation(forward, up);
			Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, Quaternion.Angle(transform.rotation, targetRotation) / 360);
			rigidbody.MoveRotation(finalRotation);
		}
		else
		{
			c.CurrentMovementState = c.fallState;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		//Gizmos.DrawWireSphere(transform.position, height);

		//if (isActive)
		//{
		//	Gizmos.color = Color.red;
		//	Gizmos.DrawSphere(targetPosition, 0.01f);
		//	Gizmos.DrawLine(transform.position, targetPosition);
		//}
		//Gizmos.color = Color.red;

	}
}

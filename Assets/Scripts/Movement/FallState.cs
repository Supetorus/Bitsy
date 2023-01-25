using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : MovementState
{
	[SerializeField, Tooltip("The amount of force applied to move the player.")]
	private float airForce = 500;

	public override void EnterState()
	{
		rigidbody.isKinematic = false;
		rigidbody.useGravity = true;
		c.move.action.Enable();
	}

	public override void UpdateState()
	{
		Vector3? point = sd.GetClosestPoint(sd.lesserAttachmentDistance);
		if (point != null)
		{
			c.CurrentMovementState = c.clingState;
		}
	}

	public override void FixedUpdateState()
	{
		Vector2 input = c.move.action.ReadValue<Vector2>();
		// Drag is only applied if there is no input.
		if (input == Vector2.zero) sd.velocity *= c.drag;
		Vector3 direction = transform.rotation * new Vector3(
				input.x * Time.fixedDeltaTime * c.acceleration,
				0,
				input.y * Time.fixedDeltaTime * c.acceleration
		);

		rigidbody.AddForce(direction.normalized * airForce * Time.deltaTime);

		Vector3 cameraForward = Vector3.ProjectOnPlane(sd.camera.forward, Vector3.up);
		Quaternion targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
		rigidbody.MoveRotation(
			Quaternion.Slerp(transform.rotation, targetRotation,
				Quaternion.Angle(transform.rotation, targetRotation) / 360)
		);
	}
}

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClingState : MovementState
{
	[SerializeField, Tooltip("This defines how far the spider keeps itself above a surface. " +
		"Adjust this to make the legs line up with the ground. Must be less than the State " +
		"Data's attachment distance."), Min(0)]
	private float height = 0.5f;
	[SerializeField, Tooltip("Multiplies with the player acceleration and maxVelocity")]
	private float sprintMultiplier = 1.5f;

	private float movementMultiplier;

	//[SerializeField] private AudioSource walking;

	public override void EnterState()
	{
		if (c.jump == null) Debug.LogError("Jump action was not assigned.");
		if (c.sprint == null) Debug.LogError("Sprint action was not assigned.");
		if (c.move == null) Debug.LogError("Move action was not assigned.");
		c.jump.action.Enable();
		c.sprint.action.Enable();
		c.move.action.Enable();
		rigidbody.isKinematic = false;
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
		// Drag is only applied if there is no input.
		if (input == Vector2.zero) sd.velocity *= c.drag;

		// Sprint
		if (c.sprint.action.ReadValue<float>() > 0) movementMultiplier = sprintMultiplier;
		else movementMultiplier = Mathf.Clamp(movementMultiplier * c.drag, 1, sprintMultiplier);

		Vector3? closestPoint = sd.GetClosestPoint(sd.attachmentDistance);
		// Near a walkable surface
		if (closestPoint != null)
		{
			// Movement
			// This is intentionally not normalized.
			Vector3 up = transform.position - (Vector3)closestPoint;
			// distance from the center of object to the ground.
			float distance = up.magnitude;
			input = Vector3.ClampMagnitude(input, 1);
			Vector3 direction = transform.rotation * new Vector3(
				input.x * Time.fixedDeltaTime * c.acceleration * movementMultiplier,
				height - distance,
				input.y * Time.fixedDeltaTime * c.acceleration * movementMultiplier
			);
			sd.velocity = Vector3.ClampMagnitude(sd.velocity + direction, c.maxVelocity * movementMultiplier);
			rigidbody.MovePosition(sd.velocity * Time.fixedDeltaTime + transform.position);

			sd.velocity = Vector3.ClampMagnitude(sd.velocity + direction, c.maxVelocity * movementMultiplier);
			rigidbody.MovePosition(sd.velocity * Time.fixedDeltaTime + transform.position);

			//TODO: This should be implemented when multiple surface materials are used
			/*if (rigidbody.velocity.sqrMagnitude >= 0.01f && !walking.isPlaying) 
			{
				walking.Play();
			}
			else if (rigidbody.velocity.sqrMagnitude < 0.01f)
			{
				walking.Stop();
			}*/

			// Rotation
			// https://discord.com/channels/489222168727519232/885300730104250418/1063576660051501136
			// Sets the forward direction of the spider based on camera.
			Vector3 upDirection = sd.CalculateAverageUp(sd.attachmentDistance);
			//print(upDirection +""+ transform.position +""+ transform.position + upDirection);
			Debug.DrawLine(transform.position, transform.position + upDirection, Color.magenta);
			Vector3 forward = Vector3.ProjectOnPlane(sd.camera.forward, upDirection);
			Quaternion targetRotation = Quaternion.LookRotation(forward, upDirection);
			// Slerp is used to make the rotation more gradual so it doesn't instantly snap.
			rigidbody.MoveRotation(
				Quaternion.Slerp(
					transform.rotation,
					targetRotation,
					Quaternion.Angle(transform.rotation, targetRotation) / 360
					)
				);
		}
		// Not near any walkable surfaces.
		else
		{
			c.CurrentMovementState = c.fallState;
		}
	}
}

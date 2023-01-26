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
	[SerializeField, Tooltip("Influences how quickly the spider slows down when input stops." +
		" Higher numbers apply less drag."), Range(0, 1)]
	private float drag;
	[SerializeField, Tooltip("Multiplies with the player acceleration and maxVelocity")]
	private float sprintMultiplier = 1.5f;

	private float movementMultiplier;

	[SerializeField] private AudioSource walking;

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
		else movementMultiplier = Mathf.Clamp(movementMultiplier, 1, sprintMultiplier);

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
			rigidbody.MovePosition(sd.velocity + transform.position);

			//if (Vector2.Dot(input, Vector2.up) > 0) { input *= movementMultiplier; }
			sd.velocity = Vector3.ClampMagnitude(sd.velocity + direction, c.maxVelocity * movementMultiplier);
			//float  scale = Vector3.Dot(transform.forward, direction) + 1;
			rigidbody.MovePosition(sd.velocity + transform.position);

			if (rigidbody.velocity.sqrMagnitude >= 0.01f && !walking.isPlaying) 
			{
				walking.Play();
			}
			else if (rigidbody.velocity.sqrMagnitude < 0.01f)
			{
				walking.Stop();
			}

			// Rotation
			// https://discord.com/channels/489222168727519232/885300730104250418/1063576660051501136
			// Sets the forward direction of the spider based on camera.
			Vector3 forward = Vector3.ProjectOnPlane(sd.camera.forward, up);
			Quaternion targetRotation = Quaternion.LookRotation(forward, up);
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

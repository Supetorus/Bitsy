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
	[Tooltip("Drag in the 'Look' action here.")]
	public InputActionReference cameraInput;

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
		rigidbody.isKinematic = true;
		rigidbody.useGravity = false;
	}

	public override void ExitState()
	{
		c.jump.action.Disable();
		c.sprint.action.Disable();
		c.move.action.Disable();
	}

	float yaw = 0;
	public float sensitivity = 50;
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

		Vector3? closestPoint = SphereRaycaster.GetClosestPoint(transform.position, sd.attachmentDistance, sd.walkableLayers);
		// Near a walkable surface
		if (closestPoint != null)
		{

			Vector2 camInput = cameraInput.action.ReadValue<Vector2>();
			//if (invertX) input *= Vector2.right * -1;
			camInput *= sensitivity;
			yaw = (camInput.x) % 360;
			//print(yaw);
			Quaternion rotationDelta = Quaternion.Euler(0, yaw, 0);

			// Rotation
			Vector3 upDirection = SphereRaycaster.CalculateAverageUp(transform.position, sd.attachmentDistance, sd.walkableLayers, transform.up);

			Vector3 forwardFromCamera = Vector3.ProjectOnPlane(sd.camera.forward, upDirection);
			Quaternion targetRotation = Quaternion.LookRotation(Vector3.Cross(transform.right, upDirection), upDirection);
			if (camInput != Vector2.zero) targetRotation *= rotationDelta;
			rigidbody.MoveRotation(
				Quaternion.Slerp(
					transform.rotation,
					targetRotation,
					Quaternion.Angle(transform.rotation, targetRotation) / 360
				)
			);

			// Movement
			// distance from the center of object to the nearest walkable object.
			float distance = (transform.position - (Vector3)closestPoint).magnitude;
			input = Vector3.ClampMagnitude(input, 1);
			Vector3 acceleration = new Vector3(
				input.x * Time.fixedDeltaTime * c.acceleration * movementMultiplier,
				height - distance,
				input.y * Time.fixedDeltaTime * c.acceleration * movementMultiplier
			);
			sd.velocity = Vector3.ClampMagnitude(sd.velocity + acceleration, c.maxVelocity * movementMultiplier);
			Quaternion movementRotationBasis = Quaternion.LookRotation(forwardFromCamera, transform.position - (Vector3)closestPoint);
			Vector3 movement = movementRotationBasis * (sd.velocity * Time.fixedDeltaTime + (Vector3.up * (height - distance)));
			rigidbody.MovePosition(movement + transform.position);

			//TODO: This should be implemented when multiple surface materials are used
			/*if (rigidbody.velocity.sqrMagnitude >= 0.01f && !walking.isPlaying) 
			{
				walking.Play();
			}
			else if (rigidbody.velocity.sqrMagnitude < 0.01f)
			{
				walking.Stop();
			}*/
		}
		// Not near any walkable surfaces.
		else
		{
			c.CurrentMovementState = c.fallState;
		}
	}
}
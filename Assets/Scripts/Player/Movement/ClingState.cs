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

	[Header("Debug tools")]
	[SerializeField, Tooltip("Makes the player continuously walk forwards, overrides any other input.")]
	private bool walkForwards = false;
	[SerializeField, Tooltip("Makes the player continuously turn to the right, overrides other input")]
	private bool turnRight = false;

	private float movementMultiplier = 1;

	//[SerializeField] private AudioSource walking;
	private ThirdPersonCameraController cameraController;
	private void Start()
	{
		cameraController = sd.camera.GetComponent<ThirdPersonCameraController>();
	}

	public override void EnterState()
	{
		cameraController.canZoom = true;
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

	private List<RaycastHit> hits;
	private float yaw = 0;
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
		if (walkForwards) input = Vector2.up;
		// Drag is only applied if there is no input.
		if (input == Vector2.zero) sd.velocity *= c.drag;

#if UNITY_EDITOR
		// Sprint
		if (c.sprint.action.ReadValue<float>() > 0) movementMultiplier = sprintMultiplier;
		else movementMultiplier = Mathf.Clamp(movementMultiplier * c.drag, 1, sprintMultiplier);
#else
		movementMultiplier = 1;
#endif

		hits = SphereRaycaster.SphereRaycast(transform.position, sd.attachmentDistance, sd.walkableLayers);
		Vector3? closestPoint = SphereRaycaster.GetClosestPoint(hits, transform.position);
		// Near a walkable surface
		if (closestPoint != null)
		{
			Vector2 camInput = cameraInput.action.ReadValue<Vector2>();
			if (turnRight) camInput = Vector2.right;
			float sensitivity = PlayerPrefs.GetFloat("Slider_CameraHorizontalSensitivity");
			camInput *= sensitivity;
			yaw = (camInput.x) % 360;
			Quaternion rotationDelta;
			if (cameraController.zooming) rotationDelta = Quaternion.identity;
			else rotationDelta = Quaternion.Euler(0, yaw, 0);

			// Rotation
			Vector3 upDirection = SphereRaycaster.CalculateAverageUp(hits, transform.position, transform.up);
			if ((transform.position - (Vector3)closestPoint).magnitude < sd.lesserAttachmentDistance)
			{
				upDirection = transform.position - (Vector3)closestPoint;
			}

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
			if (distance < sd.lesserAttachmentDistance)
			{
				rigidbody.MovePosition(rigidbody.position + upDirection * (height - distance));
			}
			else
			{
				input = Vector3.ClampMagnitude(input, 1);
				Vector3 acceleration = new Vector3(
					input.x * Time.fixedDeltaTime * c.acceleration * movementMultiplier,
					height - distance,
					input.y * Time.fixedDeltaTime * c.acceleration * movementMultiplier
				);
				sd.velocity = Vector3.ClampMagnitude(sd.velocity + acceleration, c.maxVelocity * movementMultiplier);
				//Vector3 forwardFromCamera = Vector3.ProjectOnPlane(sd.camera.forward, upDirection);
				Quaternion movementRotationBasis = Quaternion.LookRotation(transform.forward, transform.position - (Vector3)closestPoint);
				Vector3 movement = movementRotationBasis * (sd.velocity * Time.fixedDeltaTime + (Vector3.up * (height - distance)));
				rigidbody.MovePosition(movement + transform.position);
			}

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

	private void OnDrawGizmosSelected()
	{
		//if (!Application.isPlaying || hits == null) return;
		//Gizmos.color = Color.green;
		//foreach (var hit in hits)
		//{
		//	Gizmos.DrawSphere(hit.point, 0.01f);
		//}
		//Gizmos.color = Color.red;
		//Vector3 position = (Vector3)SphereRaycaster.GetClosestPoint(hits, transform.position);
		//Gizmos.DrawSphere(position, 0.02f);
		//Gizmos.DrawLine(transform.position, position);
	}
}
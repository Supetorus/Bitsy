using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[ExecuteInEditMode]
public class ThirdPersonCameraController : MonoBehaviour
{
	[Header("Camera Settings")]
	public Transform aimTarget;
	[Min(0.01f)]
	public float distance = 5;
	[Range(-360, 360)]
	public float yaw = 0;
	[Range(-90, 90)]
	public float pitch = 45;
	[Range(-90, 90)]
	public float maxPitch = 89.9f;
	[Range(-90, 90)]
	public float minPitch = -89.9f;
	[Tooltip("Camera won't pass through objects on these layers.")]
	public LayerMask hitLayers;

	[Header("Controls Settings")]
	[Tooltip("Drag in the 'Look' action here.")]
	public InputActionReference cameraInput;
	[Tooltip("How quickly the camera moves when you move mouse or stick.")]
	public float sensitivity = 1;
	[Tooltip("Whether or not to invert the horizontal camera movement.")]
	public bool invertX = false;
	[Tooltip("Whether or not to invert the vertical camera movement.")]
	public bool invertY = false;

	private new Camera camera;
	private Vector2 input = Vector2.zero;

	private void Start()
	{
		if (cameraInput == null)
		{
			Debug.LogError("You haven't set the input action for camera control.");
		}
		camera = GetComponent<Camera>();
		cameraInput.action.Enable();

		// Capture and lock the cursor
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void Update()
	{
		// up down controlled by mouse. left right controlled by player rotation.
		input = cameraInput.action.ReadValue<Vector2>();
		if (invertX) input *= Vector2.right * -1;
		if (invertY) input *= Vector2.up * -1;
		input *= sensitivity;
		//input = new Vector2(1, 1);
		//yaw = (yaw+input.x) % 360;
		pitch = Mathf.Clamp(pitch-input.y, minPitch, maxPitch);
		Quaternion rotationDelta = Quaternion.Euler(pitch, 0, 0);
		Quaternion rotation = aimTarget.rotation * rotationDelta;

		bool doQueryHitBackfaces = Physics.queriesHitBackfaces;
		Physics.queriesHitBackfaces = true;
		Physics.SphereCast(aimTarget.position, camera.nearClipPlane, rotation * Vector3.back, out RaycastHit hit, distance, hitLayers);
		Physics.queriesHitBackfaces = doQueryHitBackfaces;
		if (hit.collider)
		{
			transform.position = hit.point + (hit.normal * camera.nearClipPlane);
		}
		else
		{
			Vector3 offset = rotation * Vector3.back * distance;
			transform.position = aimTarget.position + offset;
		}
		transform.rotation = Quaternion.LookRotation(aimTarget.position - transform.position, aimTarget.up);
	}
}

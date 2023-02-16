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
	[SerializeField, Tooltip("The FOV when zoomed in.")]
	private float zoomedFOV = 20;
	[SerializeField, Tooltip("How fast the camera zooms in."), Range(0.0001f, 10f)]
	private float zoomSpeed = 4f;

	[SerializeField, Tooltip("How quickly the camera adjusts to new positions. Lower values are faster.")]
	private float cameraSpeed = 10;
	[SerializeField, Range(0.01f, 0.5f), Tooltip("How quickly the camera rotates to look at the player. Higher values are faster.")]
	private float cameraRotationSpeed;

	[Header("Controls Settings")]
	[Tooltip("Drag in the 'Look' action here.")]
	public InputActionReference cameraInput;
	[Tooltip("Drag in the 'Aim' action here.")]
	public InputActionReference aimInput;
	[Tooltip("How quickly the camera moves when you move mouse or stick.")]
	public float verticalSensitivity = 1;
	[Range(0, 1)]
	public float zoomSensivityMultiplier = 0.3f;

	[Tooltip("Whether or not to invert the horizontal camera movement.")]
	public bool invertX = false;
	[Tooltip("Whether or not to invert the vertical camera movement.")]
	public bool invertY = false;

	private new Camera camera;
	private Vector2 input = Vector2.zero;
	private float defaultFOV = 60;
	private float zoomLerpValue = 0;

	private Vector3 velocity = Vector3.zero;

	private void Start()
	{
		if (cameraInput == null || aimInput == null)
		{
			Debug.LogWarning("You haven't set an input action for camera controls.");
		}
		camera = GetComponent<Camera>();
		cameraInput.action.Enable();
		aimInput.action.Enable();

		// Capture and lock the cursor
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void Update()
	{
		// Zoom
		float sensitivity = verticalSensitivity;
		// Do zoom
		if (aimInput.action.ReadValue<float>() > 0)
		{
			zoomLerpValue = Mathf.Clamp(zoomLerpValue + zoomSpeed * Time.deltaTime, 0, 1);
			sensitivity = verticalSensitivity * zoomSensivityMultiplier;
		}
		// No zoom
		else
		{
			zoomLerpValue = Mathf.Clamp(zoomLerpValue - zoomSpeed * Time.deltaTime, 0, 1);
		}
		camera.fieldOfView = Mathf.Lerp(defaultFOV, zoomedFOV, zoomLerpValue);

		// up down controlled by mouse. left right controlled by player rotation.
		input = cameraInput.action.ReadValue<Vector2>();
		if (invertX) input *= -Vector2.right;
		if (invertY) input *= -Vector2.up;
		input *= verticalSensitivity;
		pitch = Mathf.Clamp(pitch - input.y, minPitch, maxPitch);
		Quaternion rotationDelta = Quaternion.Euler(pitch, 0, 0);
		Quaternion rotation = aimTarget.rotation * rotationDelta;

		Vector3 targetPosition = aimTarget.position + rotation * (Vector3.back * distance);
		bool hitBackfaces = Physics.queriesHitBackfaces;
		Physics.queriesHitBackfaces = true;
		Physics.SphereCast(aimTarget.position, camera.nearClipPlane, targetPosition - aimTarget.position, out RaycastHit hit, distance, hitLayers);
		Physics.queriesHitBackfaces = hitBackfaces;
		if (hit.collider)
		{
			targetPosition = hit.point + (hit.normal * camera.nearClipPlane);
			targetPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, cameraSpeed * Time.deltaTime);
		}
		else
		{
			//Vector3 offset = rotation * Vector3.back * distance;
			//targetPosition = aimTarget.position + offset;
			targetPosition = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, cameraSpeed * Time.deltaTime);
			Vector3 direction = (targetPosition - aimTarget.position).normalized;
			targetPosition = aimTarget.position + (direction * distance);
		}

		transform.position = Vector3.Lerp(targetPosition, aimTarget.position, zoomLerpValue);
		//transform.position = targetPosition;

		Quaternion targetRotation;
			print(rotation);
		if (zoomLerpValue > 0)
		{
			targetRotation = rotation;
		}
		else
		{
			targetRotation = Quaternion.LookRotation(aimTarget.position - transform.position, aimTarget.up);
		}
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, cameraRotationSpeed);
	}
}

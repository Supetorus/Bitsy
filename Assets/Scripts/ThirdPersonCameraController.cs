using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

[ExecuteInEditMode]
public class ThirdPersonCameraController : MonoBehaviour
{
	[Header("Camera Settings")]
	public Transform target;
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

	[Header("Controls Settings")]
	[Tooltip("Drag in the 'Look' action here.")]
	public InputActionReference look;
	[Tooltip("Drag in the 'Zoom' action here.")]
	public InputActionReference zoom;
	[Tooltip("How quickly the camera moves when you move mouse or stick.")]
	public float defaultSensitivity = 1;
	[Range(0, 1)]
	public float zoomSensivityMultiplier = 0.3f;
	[Tooltip("Whether or not to invert the horizontal camera movement.")]
	public bool invertX = false;
	[Tooltip("Whether or not to invert the vertical camera movement.")]
	public bool invertY = false;

	private new Camera camera;
	private float defaultFOV = 60;
	private float fovLerpValue = 0;

	private void Start()
	{
		if (look == null)
		{
			Debug.LogError("You haven't set the 'look' input action for camera control.");
		}
		if (zoom == null)
		{
			Debug.LogError("You haven't set the 'zoom' input action for camera control.");
		}
		camera = GetComponent<Camera>();
		look.action.Enable();
		zoom.action.Enable();

		// Capture and lock the cursor
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void Update()
	{
		// Zoom
		float sensitivity = defaultSensitivity;
		// Do zoom
		if (zoom.action.ReadValue<float>() > 0)
		{
			fovLerpValue = Mathf.Clamp(fovLerpValue + zoomSpeed * Time.deltaTime, 0, 1);
			sensitivity = defaultSensitivity * zoomSensivityMultiplier;
		}
		// No zoom
		else
		{
			fovLerpValue = Mathf.Clamp(fovLerpValue - zoomSpeed * Time.deltaTime, 0, 1);
		}
		camera.fieldOfView = Mathf.Lerp(defaultFOV, zoomedFOV, fovLerpValue);

		// Rotation / position
		Vector2 input = look.action.ReadValue<Vector2>();
		if (invertX) input *= Vector2.right * -1;
		if (invertY) input *= Vector2.up * -1;
		input *= sensitivity;
		yaw = (yaw + input.x) % 360;
		pitch = Mathf.Clamp(pitch - input.y, minPitch, maxPitch);
		Quaternion qYaw = Quaternion.AngleAxis(yaw, Vector3.up);
		Quaternion qPitch = Quaternion.AngleAxis(pitch, Vector3.right);
		Quaternion rotation = qYaw * qPitch;

		// Wall collision detection
		Physics.SphereCast(target.position, camera.nearClipPlane, rotation * Vector3.back, out RaycastHit hit, distance, hitLayers);
		if (hit.collider)
		{
			transform.position = hit.point + (hit.normal * camera.nearClipPlane);
		}
		else
		{
			Vector3 offset = rotation * Vector3.back * distance;
			transform.position = target.position + offset;
		}
		transform.rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
	}
}

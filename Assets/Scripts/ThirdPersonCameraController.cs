using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

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

	[Header("Controls Settings")]
	public InputActionReference inputActions;
	public float mouseSensitivity = 1;
	public float stickSensitivity = 0.5f;
	public bool invertMouseX = false;
	public bool invertMouseY = false;
	public bool invertStickX = false;
	public bool invertStickY = false;

	private new Camera camera;
	private Vector2 input = Vector2.zero;

	private void Start()
	{
		camera = GetComponent<Camera>();
		// Capture and lock the cursor
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void Update()
	{
		inputActions.action.Enable();
		input = inputActions.action.ReadValue<Vector2>();
		print(input);
		yaw = (yaw + input.x) % 360;
		pitch = Mathf.Clamp(pitch - input.y, minPitch, maxPitch);
		Quaternion qYaw = Quaternion.AngleAxis(yaw, Vector3.up);
		Quaternion qPitch = Quaternion.AngleAxis(pitch, Vector3.right);
		Quaternion rotation = qYaw * qPitch;

		RaycastHit hit;
		Physics.SphereCast(target.position, camera.nearClipPlane, rotation * Vector3.back, out hit, distance, hitLayers);
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

	private void OnDrawGizmosSelected()
	{
		if (!Application.isPlaying) return;
		Gizmos.DrawSphere(target.position, 0.1f);
		Gizmos.DrawLine(target.position, transform.position);
		Gizmos.DrawSphere(transform.position, camera.nearClipPlane);
	}
}

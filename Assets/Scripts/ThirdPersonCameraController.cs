using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

//[ExecuteInEditMode]
public class ThirdPersonCameraController : MonoBehaviour
{
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
	public float mouseSensitivity = 1;
	public float stickSensitivity = 0.5f;
	public Vector3 startDirection;
	[Tooltip("Camera won't pass through objects on these layers.")]
	public LayerMask ignoredLayers;
	public bool invertMouseX = false;
	public bool invertMouseY = false;
	public bool invertStickX = false;
	public bool invertStickY = false;

	private new Camera camera;
	private Vector2 input = Vector2.zero;
	private float lastMouseTime = 0;
	private float lastStickTime = 0;

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		camera = GetComponent<Camera>();

		CameraControls cc = new CameraControls();
		cc.Default.Enable();
		cc.Default.LookMouse.performed += LookMouse;
		cc.Default.LookStick.performed += LookStick;
		cc.Default.LookStick.canceled += CanceledStick;
	}
	private void LookMouse(InputAction.CallbackContext context)
	{
		input = context.ReadValue<Vector2>() * new Vector2(invertMouseX ? -1 : 1, invertMouseY ? -1 : 1) * mouseSensitivity;
		lastMouseTime = Time.unscaledTime;
	}

	private void LookStick(InputAction.CallbackContext context)
	{
		input = context.ReadValue<Vector2>() * new Vector2(invertStickX ? -1 : 1, invertStickY ? -1 : 1) * stickSensitivity;
		lastStickTime = Time.unscaledTime;
	}

	private void CanceledStick(InputAction.CallbackContext context)
	{
		input = Vector2.zero;
	}

	void Update()
	{
		yaw = (yaw + input.x) % 360;
		pitch -= input.y;
		pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
		Quaternion qYaw = Quaternion.AngleAxis(yaw, Vector3.up);
		Quaternion qPitch = Quaternion.AngleAxis(pitch, Vector3.right);
		Quaternion rotation = qYaw * qPitch;

		RaycastHit hit;
		Physics.SphereCast(target.position, camera.nearClipPlane, rotation * Vector3.back, out hit, distance, ignoredLayers);
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

		if (lastMouseTime > lastStickTime) // player is using stick controls.
		{
			input = Vector2.zero;
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (!Application.isPlaying) return;
		Gizmos.DrawSphere(target.position, 0.1f);
		Gizmos.DrawLine(target.position, transform.position);
		Gizmos.DrawSphere(transform.position, camera.nearClipPlane);
	}
}

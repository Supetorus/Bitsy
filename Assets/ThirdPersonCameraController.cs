using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
	public Transform target;
	public float distance = 5;
	public float pitch = 45;
	public float maxPitch = 89.9f;
	public float minPitch = -89.9f;
	public float sensitivity = 1;
	[Tooltip("Camera won't pass through objects on these layers.")]
	public LayerMask ignoredLayers;

	private float yaw = 0;
	private new Camera camera;

	private void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		camera = GetComponent<Camera>();
	}

	// Update is called once per frame
	void Update()
	{
		yaw += Input.GetAxis("Mouse X") * sensitivity;
		pitch -= Input.GetAxis("Mouse Y") * sensitivity;
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

	}

	private void OnDrawGizmosSelected()
	{
		if (!Application.isPlaying) return;
		Gizmos.DrawSphere(target.position, 0.1f);
		Gizmos.DrawLine(target.position, transform.position);
		Gizmos.DrawSphere(transform.position, camera.nearClipPlane);
	}
}

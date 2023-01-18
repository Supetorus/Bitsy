using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using static MovementController;

public class ClingState : MovementState
{
	public float forwardSpeed = 1;
	public float backSpeed = 1;
	public float sideSpeed = 1;
	public float height = 0.5f;
	public LayerMask layerMask;

	public override void EnterState()
	{
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
		Vector2 input = c.move.action.ReadValue<Vector2>();
		print(input);

		Ray ray = new Ray(transform.position, transform.rotation * Vector3.down);
		Physics.Raycast(ray, out RaycastHit hit, height * 1.5f, layerMask);

		if (hit.collider)
		{
			// Movement
			rigidbody.isKinematic = true;
			float distance = (transform.position - hit.point).magnitude;
			float x = input.x * sideSpeed;
			float y = height - distance; // currently this just teleports the player to the y position but it should interpolate.
			float z = input.y > 0 ? input.y * forwardSpeed : input.y * backSpeed;
			Vector3 direction = transform.rotation * new Vector3(x * Time.fixedDeltaTime, y, z * Time.fixedDeltaTime);

			rigidbody.MovePosition(direction + transform.position);
			//transform.position = (direction + transform.position);

			// Rotation
			// https://discord.com/channels/489222168727519232/885300730104250418/1063576660051501136
			//rigidBody.MoveRotation(Quaternion.LookRotation(camera.forward, transform.up)); // Why is this getting ignored?
			Vector3 forward = Vector3.Cross(transform.right, hit.normal);
			rigidbody.MoveRotation(Quaternion.LookRotation(forward, hit.normal));
			//transform.rotation = (Quaternion.LookRotation(camera.forward, transform.up));
			//transform.rotation = (Quaternion.LookRotation(Vector3.Cross(transform.right, hit.normal), hit.normal));
		}
		else
		{ // Nothing is detected beneath the spider so it just orients straight up and falls down.
			rigidbody.isKinematic = false;
			transform.rotation = Quaternion.identity;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawLine(transform.position, transform.position - transform.up * height * 1.5f);
		Gizmos.color = Color.green;
		Gizmos.DrawLine(transform.position, transform.position + transform.up * 0.1f);
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position + transform.right * 0.1f);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.1f);
	}
}

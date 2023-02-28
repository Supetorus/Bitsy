using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPath : MonoBehaviour
{
	[SerializeField, Tooltip("The list of points where the camera should look.")]
	private Transform[] lookPoints;
	[SerializeField, Tooltip("The rotation per second in degrees.")]
	private float rotationSpeed = 10;

	private int currentPoint = 0;

	private void Start()
	{
		transform.rotation = Quaternion.LookRotation(lookPoints[0].position - transform.position);
	}

	private void Update()
	{
		if (lookPoints.Length == 0) return;
		Quaternion nextRotation = Quaternion.LookRotation(lookPoints[(currentPoint + 1) % lookPoints.Length].position - transform.position);

		transform.rotation = Quaternion.RotateTowards(transform.rotation, nextRotation, rotationSpeed * Time.deltaTime);

		if (Quaternion.Angle(transform.rotation, nextRotation) < 5)
		{
			currentPoint = (currentPoint + 1) % lookPoints.Length;
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		for (int i = 0; i < lookPoints.Length; i++)
		{
			Gizmos.DrawSphere(lookPoints[i].position, 0.1f);
			Gizmos.DrawLine(lookPoints[i].position, lookPoints[(i + 1) % lookPoints.Length].position);
		}
	}
}

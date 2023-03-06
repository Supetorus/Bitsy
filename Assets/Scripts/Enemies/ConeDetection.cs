using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class ConeDetection : DetectionEnemy
{
	[SerializeField, Tooltip("Distance in degrees from the center of the direction the detection cone should extend."), Range(0, 180)]
	private float maxAngle;
	[SerializeField, Tooltip("The number of lines coming out of the camera around the cone"), Range(0, 200)]
	private int lineCount = 5;
	[SerializeField, Tooltip("How many degrees the camera's beams rotate per second")]
	private float beamRotationSpeed = 90;
	[SerializeField, Tooltip("The prefab containing the LineRenderer used to display the cone.")]
	private LineRenderer lineRenderer;
	[SerializeField, Tooltip("The maximum distance this object can spot the player from")]
	float maxDistance = 20f;

	private Light cameraLight;
	private List<LineRenderer> lines = new List<LineRenderer>();

	public override bool CheckSightlines()
	{
		return CanSeePlayer;
	}

	// Start is called before the first frame update
	void Start()
	{
		cameraLight = GetComponent<Light>();
	}

	// Update is called once per frame
	void Update()
	{
		// Detect for player.
		float detection = DetectCone(maxAngle, Player.Transform, layerMask, maxDistance);
		CanSeePlayer = detection != 0;
		if (cameraLight != null) cameraLight.color = CanSeePlayer ? Color.red : Color.white;
		if (CanSeePlayer) Player.Detection.ChangeDetection(detection * 100 * Time.deltaTime);
		print("Detection: " + detection);
		if (cameraLight != null) cameraLight.spotAngle = maxAngle * 2;

		// Draw the rays around the area of the cone.
		DrawCone(lineCount, lines, lineRenderer, beamRotationSpeed, maxAngle, maxDistance);
	}

	private float DetectCone(float maxAngle, Transform player, LayerMask layerMask, float maxDistance, Vector3? detectionOrigin = null)
	{
		Vector3 origin = detectionOrigin != null ? detectionOrigin.Value : transform.position;
		Vector3 directionToPlayer = player.position - origin;
		float angle = Vector3.Angle(transform.forward, directionToPlayer);
		if (angle < maxAngle &&
			Physics.Raycast(origin, directionToPlayer, out RaycastHit hit, float.PositiveInfinity, layerMask) &&
			hit.collider.CompareTag("Player") &&
			Player.AbilityController.isVisible)
		{
			return (angle / maxAngle) * Mathf.Clamp(1 - (Vector3.Distance(origin, Player.Transform.position) / maxDistance), 0, 1);
		}
		else return 0;
	}

	private float lineRotation = 0;
	private void DrawCone(int lineCount, List<LineRenderer> lines, LineRenderer lineRenderer, float beamRotationSpeed, float maxAngle, float maxDistance, Vector3? beamOrigin = null)
	{
		if (lineCount == 0) return;
		while (lines.Count < lineCount) lines.Add(Instantiate(lineRenderer, transform));
		while (lines.Count > lineCount) { Destroy(lines[lines.Count - 1].gameObject); lines.RemoveAt(lines.Count - 1); }

		Vector3 origin = beamOrigin == null ? transform.position : beamOrigin.Value;
		lineRotation += Time.deltaTime * beamRotationSpeed;
		Quaternion toEdge = Quaternion.AngleAxis(maxAngle, transform.up);
		Quaternion aroundCircumference = Quaternion.AngleAxis(lineRotation, transform.forward);
		Quaternion rotationIncrement = Quaternion.AngleAxis(360f / lineCount, transform.forward);
		for (int i = 0; i < lines.Count; i++)
		{
			lines[i].SetPosition(0, origin);
			Vector3 direction = aroundCircumference * toEdge * transform.forward;
			Physics.Raycast(transform.position, direction, out RaycastHit hit, maxDistance);
			if (hit.collider != null) lines[i].SetPosition(1, hit.point);
			else lines[i].SetPosition(1, origin + direction * maxDistance);
			aroundCircumference *= rotationIncrement;
		}
	}

	public override void DartRespond()
	{
	}

	public override void EMPRespond(float stunDuration, GameObject stunEffect)
	{
		throw new System.NotImplementedException();
	}

	private void OnDrawGizmosSelected()
	{
		Quaternion toEdge = Quaternion.AngleAxis(maxAngle, transform.up);
		Quaternion aroundCircumference = Quaternion.identity;
		Quaternion rotationIncrement = Quaternion.AngleAxis(360f / lineCount, transform.forward);
		for (int i = 0; i < lineCount; i++)
		{
			Vector3 direction = aroundCircumference * toEdge * transform.forward;
			Gizmos.DrawLine(transform.position, transform.position + direction);
			aroundCircumference *= rotationIncrement;
		}
	}
}

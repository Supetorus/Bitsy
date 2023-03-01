using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDetection : DetectionEnemy
{
	[SerializeField, Tooltip("Distance in degrees from the center of the direction the detection cone should extend."), Range(0, 180)]
	private float maxAngle;
	[SerializeField, Tooltip("The number of lines coming out of the camera around the cone"), Range(0, 200)]
	private int lineCount = 5;
	[SerializeField, Tooltip("How many degrees the camera's beams rotate per second")]
	private float beamRotationSpeed = 90;
	[SerializeField, Tooltip("The prefab containing the LineRenderer used to display the cone.")]
	private LineRenderer lineRenderer;

	GameObject player;
	private bool canSeePlayer;
	private Light cameraLight;
	private List<LineRenderer> lines = new List<LineRenderer>();
	private float lineRotation = 0;

	public override bool CheckSightlines()
	{
		return canSeePlayer;
	}

	// Start is called before the first frame update
	void Start()
	{
		cameraLight = GetComponent<Light>();
		player = GameObject.FindGameObjectWithTag("Player");
	}

	// Update is called once per frame
	void Update()
	{
		// Detect for player.
		Vector3 directionToPlayer = player.transform.position - transform.position;
		float angle = Vector3.Angle(transform.forward, directionToPlayer);
		if (angle < maxAngle)
		{
			Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit);
			if (hit.collider.CompareTag("Player") && player.GetComponent<AbilityController>().isVisible)
			{
				canSeePlayer = true;
				cameraLight.color = Color.red;
				player.GetComponent<GlobalPlayerDetection>().ChangeDetection(100 * Time.deltaTime);
			}
			else
			{
				canSeePlayer = false;
				cameraLight.color = Color.white;
			}
		}
		else
		{
			canSeePlayer = false;
			cameraLight.color = Color.white;
		}
		cameraLight.spotAngle = maxAngle * 2;
		if (lineCount == 0) return;

		// Draw the rays around the area of the cone.
		while (lines.Count < lineCount) lines.Add(Instantiate(lineRenderer, transform));
		while (lines.Count > lineCount) { Destroy(lines[lines.Count - 1].gameObject); lines.RemoveAt(lines.Count - 1); }

		lineRotation += Time.deltaTime * beamRotationSpeed;
		Quaternion toEdge = Quaternion.AngleAxis(maxAngle, transform.up);
		Quaternion aroundCircumference = Quaternion.AngleAxis(lineRotation, transform.forward);
		Quaternion rotationIncrement = Quaternion.AngleAxis(360f / lineCount, transform.forward);
		for (int i = 0; i < lines.Count; i++)
		{
			lines[i].SetPosition(0, transform.position);
			Vector3 direction = aroundCircumference * toEdge * transform.forward;
			Physics.Raycast(transform.position, direction, out RaycastHit hit);
			if (hit.collider != null) lines[i].SetPosition(1, hit.point);
			else lines[i].SetPosition(1, transform.position + direction * 200);//200 is just a big number so it's not super noticable that it ends.
			aroundCircumference *= rotationIncrement;
		}

		//Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, sightDist);
		//Collider[] collisions = Physics.OverlapSphere(hit.point, 1);

		//foreach(var collision in collisions)
		//{
		//	if (collision.gameObject.TryGetComponent<Smoke>(out _)) return;
		//}

		//foreach(var collision in collisions)
		//{
		//	if (collision.gameObject == player && player.GetComponent<AbilityController>().isVisible)
		//	{
		//		canSeePlayer = true;
		//		player.GetComponent<GlobalPlayerDetection>().ChangeDetection(0.25f, true);
		//		cameraLight.color = Color.red;
		//		break;
		//	}
		//	else
		//	{
		//		cameraLight.color = Color.white;
		//		canSeePlayer = false;
		//	}
		//}
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
			Gizmos.DrawRay(transform.position, direction * 10);
			aroundCircumference *= rotationIncrement;
		}
	}
}

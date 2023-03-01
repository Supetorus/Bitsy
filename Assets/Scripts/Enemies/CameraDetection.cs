using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDetection : DetectionEnemy
{
	[SerializeField, Tooltip("Distance in degrees from the center of the direction the detection cone should extend."), Range(0, 180)]
	private float maxAngle;
	[SerializeField, Tooltip("The layers that should interrupt raycasts searching for the player, including the player")]
	private LayerMask layerMask;
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
		canSeePlayer = ConeDetection(maxAngle, player.transform, layerMask);
		if (cameraLight != null) cameraLight.color = canSeePlayer ? Color.red : Color.white;
		if (canSeePlayer) player.GetComponent<GlobalPlayerDetection>().ChangeDetection(100 * Time.deltaTime);
		if (cameraLight != null) cameraLight.spotAngle = maxAngle * 2;

		// Draw the rays around the area of the cone.
		DrawCone(lineCount, lines, lineRenderer, beamRotationSpeed, maxAngle);
	}

	public override void DartRespond()
	{
	}

	public override void EMPRespond(float stunDuration, GameObject stunEffect)
	{
		throw new System.NotImplementedException();
	}
}

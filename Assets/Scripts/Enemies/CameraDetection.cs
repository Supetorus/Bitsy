using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDetection : DetectionEnemy
{
	[SerializeField] float sightDist;
	[SerializeField] float detectionSize;
	GameObject player;
	private bool canSeePlayer;
	private Light cameraLight;

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
		Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, sightDist);
		Collider[] collisions = Physics.OverlapSphere(hit.point, detectionSize);
		Debug.DrawRay(hit.point, Vector3.up * detectionSize);
		Debug.DrawRay(hit.point, Vector3.left * detectionSize);
		Debug.DrawRay(hit.point, Vector3.right * detectionSize);
		Debug.DrawRay(hit.point, Vector3.forward * detectionSize);
		Debug.DrawRay(hit.point, Vector3.back * detectionSize);

		foreach(var collision in collisions)
		{
			if (collision.gameObject.TryGetComponent<Smoke>(out _)) return;
		}

		foreach(var collision in collisions)
		{
			if (collision.gameObject == player && player.GetComponent<AbilityController>().isVisible)
			{
				canSeePlayer = true;
				player.GetComponent<GlobalPlayerDetection>().ChangeDetection(0.25f, true);
				cameraLight.color = Color.red;
				break;
			}
			else
			{
				cameraLight.color = Color.white;
				canSeePlayer = false;
			}
		}
	}

	public override void DartRespond()
	{
	}

	public override void EMPRespond(float stunDuration, GameObject stunEffect)
	{
		throw new System.NotImplementedException();
	}
}

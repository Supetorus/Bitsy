using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDetection : MonoBehaviour
{
	[SerializeField] float sightDist;
	[SerializeField] Light cameraLight;
	[SerializeField] LayerMask myMask;
	GameObject player;


	// Start is called before the first frame update
	void Start()
    {
		cameraLight = GetComponent<Light>();
		player = GameObject.FindGameObjectWithTag("Player");
	}

    // Update is called once per frame
    void Update()
    {
		Vector3 direction = (player.transform.position - transform.position).normalized;
		Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, sightDist);
		Collider[] collisions = Physics.OverlapSphere(hit.point, 1);

		foreach(var collision in collisions)
		{
			if(collision.gameObject == player && player.GetComponent<AbilityController>().isVisible)
			{
				player.GetComponent<GlobalPlayerDetection>().ChangeDetection(0.25f, true);
				cameraLight.color = Color.red;
				break;
			} else cameraLight.color = Color.white;
		}
		print(player.GetComponent<GlobalPlayerDetection>().currentDetectionLevel);
	}
}

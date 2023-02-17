using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrojanDart : Projectile
{
	float itemsHit = 0;
    public void OnCollisionEnter(Collision collision)
    {
		if (collision.gameObject.TryGetComponent<DetectionEnemy>(out DetectionEnemy hit)) hit.DartRespond();
		if (PlayerPrefs.GetString("TD_PENETRATION") == "FALSE" || itemsHit == 1)
		{
			gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
			gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
			gameObject.GetComponent<DestroyDelay>().hasHitSometing = true;
			gameObject.GetComponent<DestroyDelay>().destroyTimer = d_Time;
			gameObject.GetComponent<BoxCollider>().enabled = false;
		} 
		else
		{
			itemsHit = 1;
		}
    }
}

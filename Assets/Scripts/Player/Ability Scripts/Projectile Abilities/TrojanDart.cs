using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrojanDart : Projectile
{
	float itemsHit = 0;
    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Enemy>(out Enemy hit)) hit.KnockOut();
		if (PlayerPrefs.GetString("TD_PENETRATION") == "FALSE" || itemsHit == 1)
		{
			gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
			gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
			gameObject.GetComponent<DestroyDelay>().hasHitSometing = true;
			gameObject.GetComponent<DestroyDelay>().destroyTimer = d_Time;
		} 
		else
		{
			itemsHit = 1;
		}
    }
}

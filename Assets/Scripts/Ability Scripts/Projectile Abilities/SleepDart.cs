using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepDart : Projectile
{
    public override void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name);
        if (collision.gameObject.TryGetComponent<Enemy>(out Enemy hit)) hit.KnockOut();
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.GetComponent<DestroyDelay>().hasHitSometing = true;
        gameObject.GetComponent<DestroyDelay>().destroyTimer = d_Time;
    }
}

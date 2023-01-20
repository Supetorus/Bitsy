using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeBomb : Projectile
{
    [SerializeField] GameObject explosion;
    [SerializeField] float explosionRad;

    bool hasExploded = false;
    public float startSpeed = 0f;


    public override void OnCollisionEnter(Collision collision)
    {
        if (!hasExploded && collision.gameObject.tag == "Ground" && gameObject.GetComponent<Rigidbody>().velocity.sqrMagnitude < startSpeed * 0.25f)
        {
            hasExploded = true;
            print(collision.gameObject.name);
            SpawnExplosion(explosionRad);
            gameObject.GetComponent<DestroyDelay>().hasHitSometing = true;
            gameObject.GetComponent<DestroyDelay>().destroyTimer = d_Time;
        }
    }

    public void SpawnExplosion(float radius)
    {
        GameObject explode = Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation);
        explode.transform.localScale = Vector3.one * radius;
        Destroy(gameObject, explode.GetComponent<ParticleSystem>().main.duration);
    }
}

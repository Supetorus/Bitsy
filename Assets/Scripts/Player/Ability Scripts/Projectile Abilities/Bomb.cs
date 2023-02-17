using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Projectile
{
    [SerializeField] GameObject explosion;
    [SerializeField] float explosionRad;

    bool hasExploded = false;
	[HideInInspector] public bool isEMP = false;


    public override void OnCollisionEnter(Collision collision)
    {
        if (!hasExploded && collision.gameObject.tag == "Ground")
        {
            hasExploded = true;
			if(isEMP) SpawnExplosion(explosionRad * PlayerPrefs.GetInt("EMP_RADIUS"));
            else SpawnExplosion(explosionRad);
            gameObject.GetComponent<DestroyDelay>().hasHitSometing = true;
            gameObject.GetComponent<DestroyDelay>().destroyTimer = d_Time;
        }
    }

    public void SpawnExplosion(float radius)
    {
        GameObject explode = Instantiate(explosion, gameObject.transform.position, Quaternion.identity);
        explode.transform.localScale = Vector3.one * radius;
		ParticleSystem explosionSystem = explode.GetComponent<ParticleSystem>();
		explosionSystem.Stop();
		var explosionDuration = explosionSystem.main;
		if (!isEMP) explosionDuration.duration = explosion.GetComponent<ParticleSystem>().main.duration * PlayerPrefs.GetInt("SB_DURATION");
		explosionSystem.Play();
        Destroy(gameObject, explode.GetComponent<ParticleSystem>().main.duration);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPBlast : MonoBehaviour
{
	private float stunDuration = 5.0f;
	[SerializeField] GameObject stunEffect;
    // Start is called before the first frame update
    void Start()
    {
        Collider[] collisions = Physics.OverlapSphere(transform.position, GetComponent<ParticleSystem>().shape.radius);

        foreach(Collider collider in collisions)
        {
            if(collider.gameObject.TryGetComponent<DetectionEnemy>(out DetectionEnemy enemy))
            {
				enemy.EMPRespond(stunDuration * PlayerPrefs.GetInt("EMP_DURATION"), stunEffect);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPBlast : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Collider[] collisions = Physics.OverlapSphere(transform.position, GetComponent<ParticleSystem>().shape.radius);

        foreach(Collider collider in collisions)
        {
            if(collider.gameObject.TryGetComponent<Enemy>(out Enemy enemy))
            {
                enemy.Stun();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderListener : MonoBehaviour
{
    [SerializeField] Enemy me;
    private void Awake()
    {

        MeshCollider collider = GetComponentInChildren<MeshCollider>();
        if (collider.gameObject != gameObject)
        {
            ColliderBridge cb = collider.gameObject.AddComponent<ColliderBridge>();
            cb.Initialize(this);
            print(collider.gameObject.name);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            print("player seen");
            GameObject player = other.gameObject;
            Vector3 playerDirection = (player.transform.position - transform.position).normalized;
            
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            print("player seen");
            GameObject player = other.gameObject;
            Vector3 playerDirection = (player.transform.position - transform.position).normalized;
            
        }
    }
}

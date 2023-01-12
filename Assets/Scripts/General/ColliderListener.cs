using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderListener : MonoBehaviour
{
    [SerializeField] Enemy me;
    private void Awake()
    {
        Collider collider = GetComponentInChildren<Collider>();
        if (collider.gameObject != gameObject)
        {
            ColliderBridge cb = collider.gameObject.AddComponent<ColliderBridge>();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            GameObject player = other.gameObject;
            Vector3 playerDirection = (player.transform.position - transform.position).normalized;
            me.CheckSightlines(playerDirection);
        }
    }
}

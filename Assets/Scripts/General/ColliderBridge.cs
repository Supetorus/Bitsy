using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderBridge : MonoBehaviour
{
    ColliderListener _listener;
    public void Initialize(ColliderListener l)
    {
        _listener = l;
        print(l.gameObject.name);
    }

    void OnTriggerEnter(Collider other)
    {
        print("Collide");
        Debug.DrawLine(transform.position, other.transform.position);
        _listener.OnTriggerEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        print("Collide");
        Debug.DrawLine(transform.position, other.transform.position);
        _listener.OnTriggerStay(other);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTesting : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        print(other.gameObject.name);
    }
}

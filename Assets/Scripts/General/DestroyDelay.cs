using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDelay : MonoBehaviour
{
    [HideInInspector]public float destroyTimer;
    [HideInInspector]public bool hasHitSometing = false;
    private void Update()
    {
        if (hasHitSometing) {
            destroyTimer -= Time.deltaTime;
            if(destroyTimer <= 0) Destroy(gameObject);
        }
    }
}

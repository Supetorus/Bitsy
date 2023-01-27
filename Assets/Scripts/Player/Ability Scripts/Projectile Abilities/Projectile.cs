using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected float d_Time;

    public abstract void OnCollisionEnter(Collision collision);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eyesight : MonoBehaviour
{
    public Enemy me;
    private void Start()
    {
        me = GetComponentInParent<Enemy>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SpiderController player = other.gameObject.GetComponent<SpiderController>();
            me.player = player;
            Vector3 playerDirection = (player.spiderCenter.transform.position - transform.position).normalized;
            me.CheckSightlines(playerDirection);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            SpiderController player = other.gameObject.GetComponent<SpiderController>();
            me.player = player;
            Vector3 playerDirection = (player.spiderCenter.transform.position - transform.position).normalized;
            me.CheckSightlines(playerDirection);
        }
    }
}

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
            AbilityController player = other.gameObject.GetComponent<AbilityController>();
            me.player = player;
            if(me.CheckSightlines()) me.awareness += 0.33f;

        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            AbilityController player = other.gameObject.GetComponent<AbilityController>();
            me.player = player;
			me.playerInVision = true;
            if(me.CheckSightlines()) me.awareness += 0.33f;
        }
    }

    public void OnTriggerExit(Collider other)
    {
		me.playerInVision = false;
        //if(other.tag == "Player") me.player = null;
    }
}

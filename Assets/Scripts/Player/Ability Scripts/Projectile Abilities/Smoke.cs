using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : MonoBehaviour
{
    GameObject player;
    bool playerHasEntered;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<AbilityController>(out AbilityController controller))
        {
            controller.isVisible = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<AbilityController>(out AbilityController controller))
        {
            controller.isVisible = true;
        }
    }

    private void OnDestroy()
    {
        if (playerHasEntered) player.GetComponent<AbilityController>().isVisible = true;
    }
}

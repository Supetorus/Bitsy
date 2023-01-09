using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BuildingMakerToolset.Demo
{
    public class TriggerZone : MonoBehaviour
    {
        BoxCollider trigger;
        // Start is called before the first frame update
        void Start()
        {
            trigger = gameObject.GetComponent<BoxCollider>();
            trigger.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerMovement player = other.gameObject.GetComponent<PlayerMovement>();
            if (player == null)
                return;
            OnEnter(player);
        }
        private void OnTriggerExit(Collider other)
        {
            PlayerMovement player = other.gameObject.GetComponent<PlayerMovement>();
            if (player == null)
                return;
            OnExit(player);

        }

        protected virtual void OnEnter(PlayerMovement player)
        {
        }
        protected virtual void OnExit(PlayerMovement player)
        {
        }
    }
}

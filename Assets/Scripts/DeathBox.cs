using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathBox : MonoBehaviour
{
  private void OnTriggerEnter(Collider other) {
    if(other.gameObject.tag == "Player") {
      Debug.Log("Player has hit death box and fell out of the map");
    }
  }
}

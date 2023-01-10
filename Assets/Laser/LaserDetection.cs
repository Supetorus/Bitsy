using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDetection : MonoBehaviour
{
  public bool doesDamage;

    private void OnCollisionEnter(Collision collision) {
    if (collision.gameObject.tag == "Player") {
      if (doesDamage) {
        Debug.Log("You have taken damage");
      } else {
        Debug.Log("You Have Been Spotted");
      }
    } else {
      Debug.Log("Nothing to see here");
    }
    }
}

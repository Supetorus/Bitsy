using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDetection : MonoBehaviour
{
  public bool doesDamage;

  private void OnCollisionEnter(Collision collision) {
    if (collision.gameObject.tag == "Player") {
      if (doesDamage) {
        //Lower the players Health
        Debug.Log("You have taken damage");
      } else {
        //Increase the detection meter
        Debug.Log("You Have Been Spotted");
      }
    }
  }
}

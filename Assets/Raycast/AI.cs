using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour {
  public Transform playerPos;
  public float viewDistance;
  public float viewAngle;
  public GameObject spotted;
  public AudioClip spottedSound;

  private void Update() {
    findThePlayer();
  }
  bool findThePlayer() {
    if (Vector3.Distance(transform.position, playerPos.position) < viewDistance) {
      Vector3 directionToPlayer = (playerPos.position - transform.position).normalized;
      float angleBetweenGuardAndPlayer = Vector3.Angle(transform.forward, directionToPlayer);
      if (angleBetweenGuardAndPlayer < viewAngle / 2) {
        Debug.Log("viewAngle " + viewAngle);
        Debug.Log(Physics.Linecast(transform.position, playerPos.position));
        if (Physics.Linecast(transform.position, playerPos.position)) {
          Debug.Log("Player Has been seen");
          if (!spotted.activeSelf) {
            AudioSource audio = GetComponent<AudioSource>();
            audio.clip = spottedSound;
            audio.Play();
          }
          spotted.SetActive(true);
          return true;
        }
      }
    }
    Debug.Log("Player Has not been seen");
    spotted.SetActive(false);
    return false;
  }
}

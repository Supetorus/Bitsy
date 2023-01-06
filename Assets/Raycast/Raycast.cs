using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour
{
  Vector3 FrontPos;
  public float DistanceDetection;
  public GameObject vision;
  // Start is called before the first frame update

  private void Update() {
    FrontPos = new Vector3(0, 0, vision.transform.localPosition.z + DistanceDetection);
    Debug.Log("Object Postion: ");
    Debug.Log(vision.transform.position.x);
    Debug.Log(vision.transform.position.y);
    Debug.Log(vision.transform.position.z);
    Debug.Log("Looking Postion: ");
    Debug.Log(FrontPos.x);
    Debug.Log(FrontPos.y);
    Debug.Log(FrontPos.z);
    Debug.DrawRay(vision.transform.position, FrontPos, Color.white, 1.0f);
  }
}

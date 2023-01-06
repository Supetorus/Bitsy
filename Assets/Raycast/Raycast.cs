using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycast : MonoBehaviour
{
  Vector3 FrontPos;
  Vector3 RightPos;
  Vector3 LeftPos;
  public float DistanceDetection;
  public GameObject vision;
  // Start is called before the first frame update

  private void Update() {
    FrontPos = new Vector3(vision.transform.forward.x * 4.8f, vision.transform.forward.y, vision.transform.forward.z * 4.8f);
    //vision.transform.rotation = Quaternion.AngleAxis(10, Vector3.up);
    RightPos = new Vector3(vision.transform.right.x * 5, vision.transform.right.y, vision.transform.right.z * 5);
    LeftPos = new Vector3(vision.transform.right.x * -5, vision.transform.right.y, vision.transform.right.z * -5);
    Debug.DrawRay(vision.transform.position, FrontPos, Color.white, 1.0f);
    Debug.DrawRay(vision.transform.position, RightPos, Color.red, 1.0f);
    Debug.DrawRay(vision.transform.position, LeftPos, Color.blue, 1.0f);
  }
}

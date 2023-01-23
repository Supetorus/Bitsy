using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningDirection : MonoBehaviour
{
  public GameObject target;
  public new Transform transform;

    // Update is called once per frame
    void Update()
    {
        if(target != null) {
      transform.LookAt(target.transform);
    }
    }
}

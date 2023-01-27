using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningPipe : MonoBehaviour
{
  public GameObject pipe;

    // Update is called once per frame
    void FixedUpdate()
    {
      pipe.transform.eulerAngles = new Vector3(0, pipe.transform.eulerAngles.y + 179, 0);
    }
}

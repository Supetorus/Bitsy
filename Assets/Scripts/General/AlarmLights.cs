using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmLights : MonoBehaviour {
    void Update()
    {
		transform.eulerAngles = new Vector3(180, transform.eulerAngles.y + 179, 0);
    }
}

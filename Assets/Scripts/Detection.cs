using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detection : MonoBehaviour
{
	private bool detected;
	private float detection;
	public float DetectionValue { get { return detection; } set { detection = Mathf.Clamp(Mathf.Max(detection, value), 0, 100); if (detection >= 100) detected = true; } }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

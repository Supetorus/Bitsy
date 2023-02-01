using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergencyLights : MonoBehaviour
{
	private void Update() {
		transform.eulerAngles = new Vector3(180, transform.eulerAngles.y + 177, 0);
	}
}

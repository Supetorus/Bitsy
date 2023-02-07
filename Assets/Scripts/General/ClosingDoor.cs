using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosingDoor : MonoBehaviour
{
	public GameObject Door1;
	public GameObject Door2;
	public Transform location1;
	public Transform location2;

	private void OnTriggerEnter(Collider other) {
		if(other.tag == "Player") {
			Door1.transform.position = location1.position;
			Door2.transform.position = location2.position;
		}
	}
}

using System;
using moveen.utils;
using UnityEngine;

namespace moveen.example {
	public class SphericBridle : MonoBehaviour {
		public float radius;
		public float mixSpeed = 2;

		public Transform guided;

		// Use this for initialization
		void Start () {
		
		}
	
		// Update is called once per frame
		void FixedUpdate () {
			if (guided != null) {
				RaycastHit hit;
				if (Physics.SphereCast(transform.position + Vector3.up * 10, radius, Vector3.down, out hit, 100f, 1 << 8)) {
				} else {
					hit.point = Vector3.zero;
				}
				guided.transform.position = guided.transform.position.mix(new Vector3(transform.position.x, Math.Max(hit.point.y, 0) + radius, transform.position.z), mixSpeed * Time.deltaTime);
			}

		}

		public void OnDrawGizmos() {
			Gizmos.color = new Color(0.7f, 0.5f, 0.5f);
			Gizmos.DrawWireSphere(transform.position, radius);
			if (guided != null) {
				Gizmos.DrawWireSphere(guided.transform.position, radius);
			}
		}

	}
}
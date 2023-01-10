using System;
using UnityEngine;

namespace moveen.example {
    public class SimpleRotate : MonoBehaviour {

        public float angleStep = 5;
        [NonSerialized]private Quaternion initialRot;
        [NonSerialized]private float angle = 0;

        private void OnEnable() {
            initialRot = transform.rotation;
        }

        private void FixedUpdate() {
            if (Input.GetKey(KeyCode.LeftArrow)) angle -= angleStep;
            if (Input.GetKey(KeyCode.RightArrow)) angle += angleStep;
            transform.rotation = initialRot * Quaternion.AngleAxis(angle, Vector3.up);
        }
    }
}
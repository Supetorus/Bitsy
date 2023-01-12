using System;
using moveen.core;
using UnityEngine;

namespace moveen.example {
    [Serializable]
    public class SurfaceDetector1 : ISurfaceDetector {
        [Tooltip("The layer which will be used to detect surface. For example, create a layer for objects you want to walk on and name it as \"ground\" or \"terrain\"")]
        public LayerMask layer = 0;
        public override Vector3 detect(Vector3 input, Vector3 normal) {
            RaycastHit rh;
            found = Physics.Raycast(input + normal * 5, -normal, out rh, 10, layer.value);
            if (found) {
                pos = rh.point;
                this.normal = rh.normal;
            } else {
                pos = new Vector3(input.x, 0, input.z);
                this.normal = Vector3.up;
            }
            return pos;
        }
    }
}
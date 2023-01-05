using moveen.utils;
using UnityEngine;

namespace moveen.core {
    public class SurfaceDetectorStub : ISurfaceDetector {

        public override Vector3 detect(Vector3 input, Vector3 normal) {
            found = true;
            pos = input.withSetY(0);
            this.normal = new Vector3(0, 1, 0);
            return pos;
        }
    }
}
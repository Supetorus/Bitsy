using moveen.utils;
using UnityEngine;

namespace moveen.core {
    public class Bone {
        public float r;
        public OriginBase origin;

        public Transform attachedGeometry;

        public Vector3 deltaPos;
        public Quaternion deltaRot;

        public void copy() {
//            origin.tick();
            if (attachedGeometry != null) {
                attachedGeometry.SetPositionAndRotation(
                    origin.getPos() + origin.getRot().rotate(deltaPos),
                    origin.getRot() * deltaRot);
            }
        }

        public void copy(Transform other, float progress) {
//            origin.tick();
            if (attachedGeometry != null) {
                Vector3 newPos = origin.getPos() + origin.getRot().rotate(deltaPos);
                Quaternion newRot = origin.getRot() * deltaRot;
                attachedGeometry.SetPositionAndRotation(newPos.mix(other.position, progress), newRot.nlerp(other.rotation, progress));
            }
        }

        public Bone(OriginBase origin, float r) {
            this.origin = origin;
            this.r = r;
        }
    }
}
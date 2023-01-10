using UnityEngine;

namespace moveen.core {
    abstract public class OriginBase {
        public string name;
        public bool isStatic;

        public OriginBase() {
        }

        public virtual void tick() {
        }

        abstract public Matrix4x4 getMatrix();
        abstract public Vector3 getPos();
        abstract public Quaternion getRot();

    }
}
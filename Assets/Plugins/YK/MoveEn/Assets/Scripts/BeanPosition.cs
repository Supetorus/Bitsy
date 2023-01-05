using UnityEngine;

namespace moveen.descs {
    public class BeanPosition {
        public Transform transform;
        public Vector3 storedPosition;
        public Quaternion storedRotation;

        public BeanPosition(Transform transform) {
            this.transform = transform;
        }
    }
}
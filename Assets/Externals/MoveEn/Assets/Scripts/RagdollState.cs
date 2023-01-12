using UnityEngine;

namespace moveen.descs {
    public class RagdollState {
        public Transform transform;
        public Rigidbody rigidbody;
        public Collider collider;
        public Vector3 storedPosition;
        public Quaternion storedRotation;

        public RagdollState(Transform transform, Rigidbody rigidbody) {
            this.transform = transform;
            this.rigidbody = rigidbody;
        }

        public RagdollState(Transform transform, Rigidbody rigidbody, Collider collider) {
            this.transform = transform;
            this.rigidbody = rigidbody;
            this.collider = collider;
        }
    }
}
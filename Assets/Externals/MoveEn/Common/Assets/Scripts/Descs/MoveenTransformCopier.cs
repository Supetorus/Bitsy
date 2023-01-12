using moveen.utils;
using UnityEngine;

namespace moveen.descs {
    [ExecuteInEditMode]
    public class MoveenTransformCopier : OrderedMonoBehaviour {
        [HideInInspector] public Vector3 deltaPos;
        [HideInInspector] public Quaternion deltaRot = Quaternion.identity;
        public Transform target;
        public float transition;

        public MoveenTransformCopier() {
            executionOrder = 10;
        }

        public override void tick(float dt) {
            if (target == null) return;
            if (Application.isPlaying) {
                copyTransforms();
            } else {
                updateDeltas();
            }
        }

        private void copyTransforms() {
            if (transition == 1) return;
            Vector3 newPos = transform.position + transform.rotation.rotate(deltaPos);
            Quaternion newRot = transform.rotation * deltaRot;
            if (transition == 0) {
                target.SetPositionAndRotation(newPos, newRot);
            } else {
                target.SetPositionAndRotation(newPos.mix(transform.position, transition), newRot.nlerp(transform.rotation, transition));
            }
        }

        private void updateDeltas() {
            if (target == null) return;
            Quaternion geomR = target.rotation;
            Quaternion skelR = transform.rotation;

            deltaPos = skelR.conjug().rotate(target.position - transform.position);
            deltaRot = geomR.rotSub(skelR);
        }
    }
}
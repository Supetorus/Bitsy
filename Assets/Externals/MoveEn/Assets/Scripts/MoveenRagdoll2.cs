using System;
using System.Collections.Generic;
using moveen.example;
using moveen.utils;
using UnityEngine;

namespace moveen.descs {
    /// <summary>
    /// Switch one limb to the ragdoll state AND detach it from the skeleton.
    /// </summary>
    public class MoveenRagdoll2 : MonoBehaviour, Startable {
        [BindWarning]
        public MoveenStepper5 stepper;
        [Tooltip("This joint can't be active when Moveen is working. So it will be enabled when the leg is going ragdoll")]
        [BindWarning]
        public Joint bodyJoint;
        [Tooltip("Skel, which should be disabled when going to ragdoll")]
        [BindOrLocalWarning]
        public MoveenSkelWithBones bones;
        [Tooltip("Switcher, which does the actual work switching to the ragdoll")]
        [BindOrLocalWarning]
        public MoveenRagdollSwitch switcher;
        [Tooltip("Repulsion force to affect both leg and body")]
        public float repulsion = 10;
        
        public bool isRagdoll;
        [NonSerialized] private bool isRagdollOld;

        public void Start() {
            if (switcher == null) switcher = GetComponent<MoveenRagdollSwitch>();
            if (bones == null) bones = GetComponent<MoveenSkelWithBones>();
        }

        private void Update() {
            if (isRagdollOld != isRagdoll) {
                isRagdollOld = isRagdoll;
                if (isRagdoll) {
                    switcher.startRagdoll();
                    transform.SetParent(null);
                    stepper.needsUpdate = true;
                    Destroy(bodyJoint);
                    bones.enabled = false;
                    
                    Vector3 force = stepper.transform.position.sub(bodyJoint.transform.position).normalized(repulsion);
                    stepper.bodyRigid.AddForceAtPosition(force, bodyJoint.transform.position, ForceMode.Impulse);
                    bodyJoint.transform.GetComponent<Rigidbody>().AddForceAtPosition(-force, bodyJoint.transform.position, ForceMode.Impulse);
                }
            }
        }

        public void start() {
            isRagdoll = !isRagdoll;
        }
    }
}
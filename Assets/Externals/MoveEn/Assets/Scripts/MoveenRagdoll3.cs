using System;
using moveen.example;
using moveen.utils;
using UnityEngine;

namespace moveen.descs {
    /// <summary>
    /// Switch one limb to the ragdoll state AND possibly turn it on back.
    /// </summary>
    public class MoveenRagdoll3 : MonoBehaviour, Startable {
        [BindWarning]
        public MoveenStepper5 stepper;
        [Tooltip("This joint can't be active when Moveen is working. So it will be enabled when the leg is going ragdoll")]
        [BindWarning]
        public Joint bodyJoint;
        [BindOrLocalWarning]
        public MoveenStep2 step;
        [Tooltip("Skel, which should be disabled when going to ragdoll")]
        [BindOrLocalWarning]
        public MoveenSkelWithBones bones;
        [Tooltip("Switcher, which does the actual work switching to the ragdoll")]
        [BindOrLocalWarning]
        public MoveenRagdollSwitch switcher;
        [Tooltip("Mixer script mixes last ragdolled animation with newly active one")]
        [BindOrLocalWarning]
        public MoveenMix mixer;
        
        public bool isRagdoll;
        [NonSerialized] private bool isRagdollOld;

        public float activationTime = 1;
        [NonSerialized] public float currentActivationTime = -1;

        public void Start() {
            if (switcher == null) switcher = GetComponent<MoveenRagdollSwitch>();
            if (bones == null) bones = GetComponent<MoveenSkelWithBones>();
            if (mixer == null) mixer = GetComponent<MoveenMix>();
            if (step == null) step = GetComponent<MoveenStep2>();
        }

        private void Update() {
            if (currentActivationTime != -1 && currentActivationTime < activationTime) {
                currentActivationTime = MyMath.min(activationTime, currentActivationTime + Time.deltaTime);
                
                //to keep the leg in "reset" position while making raise from ragdoll
                step.step.reset(stepper.transform.position, stepper.transform.rotation);
                step.step.undockCurTime = 0;
                bones.setTarget(step.step.posAbs, step.step.footOrientation);

                if (currentActivationTime == activationTime) {
                    mixer.enabled = false;
                    switcher.canRiseFromRagdoll = false;
                    stepper.needsUpdate = true;//finally acquire new layout
                } else {
                    mixer.progress = currentActivationTime / activationTime;
                }
            }

            if (isRagdollOld != isRagdoll) {
                isRagdollOld = isRagdoll;
                if (isRagdoll) {
                    bodyJoint.connectedBody = stepper.transform.GetComponent<Rigidbody>();
                    mixer.enabled = false;
                    switcher.startRagdoll();
                    switcher.canRiseFromRagdoll = false;
                    step.enabled = false;
                    bones.enabled = false;
                    stepper.needsUpdate = true;
                } else {
                    bodyJoint.connectedBody = null;
                    switcher.stopRagdoll();
                    switcher.canRiseFromRagdoll = true;

                    currentActivationTime = 0;
                    mixer.progress = 0;

                    bones.enabled = true;
                    //stepper.needsUpdate = true;//to activate new layout immediately, but then invisible leg will step while rasing from ragdoll
                    step.enabled = true;

                    mixer.enabled = true;
                }
            }
        }

        public void start() {
            isRagdoll = !isRagdoll;
        }
    }
}
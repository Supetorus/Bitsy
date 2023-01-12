using System;
using System.Collections.Generic;
using moveen.example;
using moveen.utils;
using UnityEngine;

namespace moveen.descs {
    /// <summary>
    /// A simple switch to ragdoll. Turns on and turns off what is needed - stepper, copiers, etc, and switches Rigidbody's isKinematic. Its editor contains buttons for ragdoll generation and manual (editor only) ragdoll switch.
    /// </summary>
    public class MoveenRagdoll1 : MonoBehaviour, Startable {
        public MoveenStepper5 stepper;
        [ReadOnly] public bool isRagdoll;
        [NonSerialized] private List<RagdollState> bodyParts = new List<RagdollState>();
        [NonSerialized] private MoveenSkelWithBones[] cache1;
        [NonSerialized] private MoveenTransformCopier[] cache2;
        [NonSerialized] private Rigidbody[] cache3;

        //if RigidBody is shared between ragdoll and stepper:
        [HideInInspector] public Rigidbody usedByStepper;//not switch kinematic 
        [HideInInspector] public List<Joint> jointsToConnect;//connect/disconnect (because kinematic legs will prevent body from moving) 

        public void Start() {
            if (stepper == null) stepper = GetComponent<MoveenStepper5>();
            if (stepper != null) {
                cache1 = stepper.gameObject.GetComponentsInChildren<MoveenSkelWithBones>();
                cache2 = stepper.gameObject.GetComponentsInChildren<MoveenTransformCopier>();
                cache3 = stepper.gameObject.GetComponentsInChildren<Rigidbody>();
            }
            Rigidbody[] rigidBodies = GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < rigidBodies.Length; i++) {
                bodyParts.Add(new RagdollState(rigidBodies[i].transform, rigidBodies[i], rigidBodies[i].GetComponent<Collider>()));
            }
            setKinematic(true);
            if (usedByStepper != null) {
                for (int i = 0; i < jointsToConnect.Count; i++) {
                    jointsToConnect[i].connectedBody = null;
                }
            }
        }

        public void startRagdoll() {
            if (usedByStepper != null) {
                for (int i = 0; i < jointsToConnect.Count; i++) {
                    jointsToConnect[i].connectedBody = usedByStepper;
                }
            }
            
            
            setKinematic(false);
            isRagdoll = true;
            if (stepper != null) {
                stepper.enabled = false;
                for (int i = 0; i < cache1.Length; i++) cache1[i].enabled = false;
                for (int i = 0; i < cache2.Length; i++) cache2[i].enabled = false;
                for (int i = 0; i < cache2.Length; i++) {
                    if (cache3[i] != usedByStepper) cache3[i].isKinematic = true;
                }
            }
        }

        private void setKinematic(bool newValue) {
            for (int i = 0; i < bodyParts.Count; i++) {
                if (bodyParts[i].rigidbody != usedByStepper) {
                    bodyParts[i].rigidbody.isKinematic = newValue;
                    if (bodyParts[i].collider != null) bodyParts[i].collider.enabled = !newValue;
                }
            }
        }

        public void start() {
            isRagdoll = !isRagdoll;
        }
    }
}
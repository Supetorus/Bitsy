using System;
using System.Collections.Generic;
using moveen.utils;
using UnityEngine;

namespace moveen.descs {
    public class MoveenRagdollSwitch : OrderedMonoBehaviour {
//        [ReadOnly] public bool isRagdoll;
        
        public bool isRagdoll;
        [NonSerialized] private bool isRagdollOld;

        [Tooltip("Must be used in conjuction with MoveenMix")] 
        public bool canRiseFromRagdoll;
        private List<RagdollState> bodyParts = new List<RagdollState>();

        public void Start() {
            foreach (var s in GetComponentsInChildren<Rigidbody>()) {
                bodyParts.Add(new RagdollState(s.transform, s, s.transform.GetComponent<Collider>()));
            }
            setKinematic(true);
        }

        public void startRagdoll() {
            setKinematic(false);
            isRagdoll = true;
            isRagdollOld = true;
        }

        public void stopRagdoll() {
            setKinematic(true);
            isRagdoll = false;
            isRagdollOld = false;
            foreach (RagdollState b in bodyParts) {
                b.storedRotation = b.transform.localRotation;
                b.storedPosition = b.transform.localPosition;
            }
        }

        private void setKinematic(bool newValue) {
            foreach (RagdollState b in bodyParts) {
                b.rigidbody.isKinematic = newValue;
                if (b.collider != null) b.collider.enabled = !newValue;
            }
        }

        public override void tick(float dt) {
            if (isRagdollOld != isRagdoll) {
                if (isRagdoll) {
                    startRagdoll();
                } else {
                    stopRagdoll();
                }
            }

            if (canRiseFromRagdoll && !isRagdoll) {
                foreach (RagdollState b in bodyParts) {
                    b.transform.localPosition = b.storedPosition;
                    b.transform.localRotation = b.storedRotation;
                }
            }
        }
    }
}
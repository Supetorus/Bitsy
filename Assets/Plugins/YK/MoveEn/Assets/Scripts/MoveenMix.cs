using System.Collections.Generic;
using moveen.utils;
using UnityEngine;

namespace moveen.descs {
    /**
     * Mix geometry pos/rot from 1-state to 2-state.
     *
     * Example: mix from one script result A to script result B
     * Config:
     * * Script A order : 10
     * * Script B order : 20
     * * MoveenMix.state1Order : 15
     * * MoveenMix.executionOrder : 25
     * Execution:
     * 1. Script A (order 10)
     * 2. it is state-1 now
     * 3. we remember its result in ProceduralMixHelper (order 15)
     * 4. Script B executes (order 20)
     * 5. it is state-2 now
     * 6. MoveenMix will mix its results with the stored (order 25) 
     * 7. it is state-1 + state-2 now
     *
     * Example: mix from animation to Script
     * Config:
     * Script order : 10
     * MoveenMix.state1Order : -100500
     * MoveenMix.executionOrder : 20
     * Execution:
     * 1. it is state-1 now (animated)
     * 2. at the beginning of a LateUpdate (and after an animation worked out) ProceduralMixHelper is storing the new positions (order -100500)
     * 3. the Script changes pos/rot (order 10)
     * 4. it is state-2 now
     * 5. MoveenMix mixes it with stored (order 20) 
     * 6. it is state-1 + state-2 now
     * 
     */ 
    public class MoveenMix : OrderedMonoBehaviour {
        [Tooltip("Progress from 1-state to 2-state")]
        [Range(0, 1)] public float progress = 1;

        [Tooltip("Order, at which gometry contains 1-state")]
        public int state1Order = -100500;

        [Tooltip("Restore 1-state in Update for the case animation doesn't update some of pos/rot")]
        public bool restore1StateInUpdate = true;

        public bool skipSelf = true;
        
        public List<BeanPosition> bodyParts = new List<BeanPosition>();

        public MoveenMix() {
            //order, at which geometry contains 2-state 
            executionOrder = 100;
        }

        public void Start() {
            ProceduralMixHelper helper = gameObject.AddComponent<ProceduralMixHelper>();
            helper.executionOrder = state1Order;
            helper.mixer = this;

            foreach (var s in GetComponentsInChildren<Rigidbody>()) if (!skipSelf || s.gameObject != gameObject) {
                bodyParts.Add(new BeanPosition(s.transform));
            }
        }

        public Vector3 displaceState1;
        
        /*
         * At this tick, geometry is in 2-state 
         */
        public override void tick(float dt) {
            bool first = true;
            if (progress == 1) return;
            foreach (BeanPosition b in bodyParts) {
                Vector3 state1Pos = b.storedPosition;
                if (first) state1Pos += displaceState1;
                first = false;
                b.transform.localPosition = state1Pos.mix(b.transform.localPosition, progress);
                b.transform.localRotation = Quaternion.Slerp(b.storedRotation, b.transform.localRotation, progress);
            }
        }

        //events order: Update -> animation engine -> LateUpdate (ordered ticks)
        //  so here we can prepare for the animation (it doesn't always needed though)
        public override void Update() {
            base.Update();
            if (restore1StateInUpdate) {
                foreach (BeanPosition b in bodyParts) {
                    b.transform.localPosition = b.storedPosition;
                    b.transform.localRotation = b.storedRotation;
                }
            }
        }
    }

    public class ProceduralMixHelper : OrderedMonoBehaviour {
        public MoveenMix mixer;

        /*
         * At this tick, geometry contains 1-state, which we will later mix with 2-state 
         */
        public override void tick(float dt) {
            foreach (BeanPosition b in mixer.bodyParts) {
                b.storedPosition = b.transform.localPosition;
                b.storedRotation = b.transform.localRotation;
            }
        }
    }
}
using System;
using moveen.utils;
using UnityEngine;

namespace moveen.descs {
    [ExecuteInEditMode] //needed to detect position move
    public class MoveenSkelBase : OrderedMonoBehaviour {//TODO rename
        public Transform target;
        public Vector3 targetPosRel = new Vector3(1, 0, 0);
        public Quaternion targetRotRel = Quaternion.identity;
        [ReadOnly] public Vector3 targetPos;
        [ReadOnly] public Quaternion targetRot = Quaternion.identity;
        public bool useLimits = true;
        public float minLen = 0;
        public float maxLen = 1;
        [ReadOnly] public float comfort;
        [ReadOnly] public Vector3 limitedResultTarget;

        [HideInInspector] [NonSerialized] public bool needsUpdate = true;
        [HideInInspector] [NonSerialized] public bool needsReset = true;

        [HideInInspector] public Vector3 footLocal;

        public MoveenSkelBase() {
            MUtil.logEvent(this, "constructor", true);
            executionOrder = 1;
        }

        //TODO try to get rid
        public void checkNeeds() {
            if (needsUpdate) updateData();
            if (needsReset) reset();
        }
        
        

        public override void tick(float dt) {
            //TODO do we rly need it?
            if (!Application.isPlaying) {
                if (target == null) {
                    var t = transform;
                    setTarget(t.TransformPoint(targetPosRel), t.rotation * targetRotRel);
//                } else {
//                    setTarget(transform.InverseTransformPoint(target.transform.position), target.transform.rotation.rotSub(transform.rotation));
                }
            }
            
            checkNeeds();
            
            if (target == null) {
//                setForNoTarget();
            } else {
                //TODO use local pos/rot ?
                setTarget(target.position, target.rotation);
            }
        }

        private void setForNoTarget() {
            if (transform.parent != null) {
                setTarget(transform.parent.TransformPoint(targetPosRel), transform.parent.rotation * targetRotRel);
            } else {
                setTarget(targetPosRel, targetRotRel);
            }
        }

        public virtual void updateLimitedResult() {
            limitedResultTarget = useLimits ? targetPos.clampAround(transform.position, 0, maxLen) : targetPos;
        }

        public virtual void setTarget(Vector3 targetPos, Quaternion targetRot) {
//            MUtil.log(this, "setTarget " + targetPos.ToString("F4"));
//            Debug.Log("setTarget " + targetPos.ToString("F4"));
            this.targetPos = targetPos;
            this.targetRot = targetRot;
            limitedResultTarget = targetPos;//MUST BE UPDATED IN CORRESPONDING SUBCLASS
        }

        //update inner structures in response to GameObject params change
        public virtual void updateData() {
            MUtil.logEvent(this, "updateData");
            needsUpdate = false;
            //if (!Application.isPlaying) reset();
        }

        public virtual void reset() {
            needsReset = false;
        }


        public override void OnValidate() {
            base.OnValidate();
            needsUpdate = true;
        }
    }
}
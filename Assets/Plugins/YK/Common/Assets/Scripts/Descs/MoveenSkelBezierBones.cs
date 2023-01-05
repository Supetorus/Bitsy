
using System.Collections.Generic;
using moveen.utils;
using UnityEngine;

namespace moveen.descs {
    public class MoveenSkelBezierBones : MoveenSkelBezier {
        
        public List<Vector3> bonesDeltaPos = new List<Vector3>();
        public List<Quaternion> bonesDeltaRot = new List<Quaternion>();
        public List<int> stepNumbers = new List<int>();
        
        
        public override void updateData() {
            base.updateData();

            int newCount = transform.childCount;
            MUtil.madeCount(bonesDeltaPos, newCount);
            MUtil.madeCount(bonesDeltaRot, newCount);
            MUtil.madeCount(stepNumbers, newCount);
        }

        public override void tick(float dt) {
            base.tick(dt);
            if (Application.isPlaying) {
                useDeltas();
            } else {
                updateDeltas();
            }
            
        }

        public void OnTransformChildrenChanged() {
            MUtil.logEvent(this, "OnTransformChildrenChanged");
            needsUpdate = true;
        }

        public void updateDeltas() {
            //TODO calc only needed steps
//            List<Vector3> poss = new List<Vector3>();
//            List<Quaternion> rots = new List<Quaternion>();
//            MUtil.madeCount(poss, 100);
//            MUtil.madeCount(rots, 100);
//            fillOrigins(
//                transform.position, 
//                transform.rotation.rotate(new Vector3(radius1, 0, 0)) + transform.position, 
//                transform.rotation, 
//                targetPos,
//                targetRot.rotate(new Vector3(-radius2, 0, 0)) + targetPos,
//                target.rotation, 
//                poss, 
//                rots);

            int childCount = transform.childCount;
            for (int i = 0; i < childCount; i++) {
                Transform child = transform.GetChild(i);
                int closest = 0;
                for (int j = 1; j < poss.Count; j++) {
                    if (poss[closest].dist(child.position) > poss[j].dist(child.position)) closest = j;
                }
                stepNumbers[i] = closest;
            }
            
            for (int i = 0; i < bonesDeltaPos.Count; i++) {
                Transform child = transform.GetChild(i);

                int stepNumber = stepNumbers[i];
                bonesDeltaRot[i] = rots[stepNumber].conjug().mul(child.rotation);
//                bonesDeltaRot[i] = child.rotation.rotSub(rots[stepNumber]);
                bonesDeltaPos[i] = rots[stepNumber].conjug().rotate(child.position - poss[stepNumber]);

//                bonesDeltaRot[i] = bonesDeltaRot[i].rotSub(transform.rotation);
//                bonesDeltaPos[i] = transform.InverseTransformPoint(bonesDeltaPos[i]);
            }
        }

        public void useDeltas() {
            for (int i = 0; i < bonesDeltaPos.Count; i++) {
                Transform child = transform.GetChild(i);
                int stepNumber = stepNumbers[i];
                child.transform.rotation = rots[stepNumber] * bonesDeltaRot[i];
                child.transform.position = rots[stepNumber].rotate(bonesDeltaPos[i]) + poss[stepNumber];
//                child.transform.position = bonesDeltaRot[i].rotate(bonesDeltaPos[i]) + poss[stepNumber];
            }
        }

    }
}
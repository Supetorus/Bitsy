using System;
using moveen.core;
using moveen.utils;
using UnityEngine;

namespace moveen.descs {
    public class MoveenSkelRotElbow : MoveenSkelWithBones {
        [CustomSkelControl]
        public Vector3 normalRel = new Vector3(0, 1, 0);
        [CustomSkelControl]
        public float r1 = 1;
        [CustomSkelControl]
        public float r2 = 1;

        [CustomSkelResult]
        public Vector3 elbowAbs;
        
        private JunctionSimpleSpider rotationJoint = new JunctionSimpleSpider();
        private JunctionElbow elbowJoint = new JunctionElbow();
        
        public MoveenSkelRotElbow() {
            rotationJoint.basisAbs = new P<Vector3>();
            rotationJoint.targetAbs = new P<Vector3>();

            elbowJoint.basisAbs = rotationJoint.basisAbs; 
            elbowJoint.targetAbs = rotationJoint.targetAbs; 
            elbowJoint.normalAbs = rotationJoint.resultAbs;

            bones.Add(new Bone(new OriginGlobalVectorP(elbowJoint.basisAbs, elbowJoint.resultAbs, false, elbowJoint.normalAbs, true).setXz(), r1));
            bones.Add(new Bone(new OriginGlobalVectorP(elbowJoint.resultAbs, elbowJoint.targetAbs, false, elbowJoint.normalAbs, true).setXz(), r2));
            bonesGeometry = MUtil2.al<Transform>(null, null); //two bones + foot
        }

        //TODO call when needed only
        public override void updateData() {
            MUtil.logEvent(this, "updateData");
            normalRel = normalRel.normalized;
            rotationJoint.axisRel = normalRel;
            elbowJoint.radius1 = r1;
            elbowJoint.radius2 = r2;
            bones[0].r = r1;
            bones[1].r = r2;
            maxLen = (r1 + r2) * 0.99f;//*0.99 because elbow junction "sticks" when it is close to the limit

            base.updateData();
        }

        public override void tickStructure(float dt) {
            rotationJoint.targetAbs.v = useLimits ? targetPos.clampAround(basePos, Math.Abs(r1 - r2), maxLen) : targetPos;
            limitedResultTarget = rotationJoint.targetAbs.v;
            
            rotationJoint.basisAbs.v = basePos;
            rotationJoint.calcAbs(dt, basePos, baseRot);

            rotationJoint.tick(dt);
            elbowJoint.tick(dt);
            elbowAbs = elbowJoint.resultAbs.v;

            comfort = rotationJoint.targetAbs.v.dist(basePos) / maxLen;
        }
    }
}
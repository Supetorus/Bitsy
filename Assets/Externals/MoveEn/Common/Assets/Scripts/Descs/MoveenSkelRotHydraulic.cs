using System;
using moveen.core;
using moveen.utils;
using UnityEngine;

namespace moveen.descs {
    public class MoveenSkelRotHydraulic : MoveenSkelWithBones {

        public float r1 = 1;
        public float r2 = 1;

        [CustomSkelControl]
        public Vector3 normalRel = new Vector3(0, 1, 0);

        public JunctionSimpleSpider rotationJoint = new JunctionSimpleSpider();
        
        public MoveenSkelRotHydraulic() {
            rotationJoint.basisAbs = new P<Vector3>();
            rotationJoint.targetAbs = new P<Vector3>();

            bones.Add(new Bone(new OriginGlobalVectorP(rotationJoint.basisAbs, rotationJoint.targetAbs, false, rotationJoint.resultAbs, true).setXz(), 1));
            bones.Add(new Bone(new OriginGlobalVectorP(rotationJoint.targetAbs, rotationJoint.basisAbs, false, rotationJoint.resultAbs, true).setXz(), 1));
            bonesGeometry = MUtil2.al<Transform>(null, null);
            
        }

        public override void updateData() {
            normalRel = normalRel.normalized;
            rotationJoint.axisRel = normalRel;

            bones[0].r = r1;
            bones[1].r = r2;
            maxLen = r1 + r2;
            
            base.updateData();//AFTER self ticks, because we want new info in bones
        }

        public override void tickStructure(float dt) {
            rotationJoint.targetAbs.v = useLimits ? targetPos.clampAround(basePos, Math.Max(r1, r2), maxLen) : targetPos;
            limitedResultTarget = rotationJoint.targetAbs.v;

            rotationJoint.basisAbs.v = basePos;
            rotationJoint.calcAbs(dt, basePos, baseRot);

            rotationJoint.tick(dt);
            
            comfort = rotationJoint.targetAbs.v.dist(basePos) / maxLen;
        }
    }
}
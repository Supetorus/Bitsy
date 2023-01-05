using System;
using moveen.core;
using moveen.utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace moveen.descs {
    public class MoveenSkelLimb1 : MoveenSkelWithBones {
        [CustomSkelControl]
        public float r1 = 1;
        [CustomSkelControl]
        public float r2 = 1;

        [CustomSkelResult] 
        [NonSerialized]
        public Vector3 elbow;
        
        
        
        [FormerlySerializedAs("rotationJoint")]//31.08.17
        [CustomSkelControlAttribute]
        public JunctionSimpleSpider rotJoint = new JunctionSimpleSpider();
        private readonly JunctionElbow elbowJoint = new JunctionElbow();
        
        public float footPlatformHeight = 0.4f;

        public MoveenSkelLimb1() {
            rotJoint.basisAbs = new P<Vector3>();
            rotJoint.targetAbs = new P<Vector3>();

            elbowJoint.basisAbs = rotJoint.basisAbs;
            elbowJoint.targetAbs = rotJoint.targetAbs;
            elbowJoint.normalAbs = rotJoint.resultAbs;

            bones.Clear();
            bones.Add(new Bone(new OriginGlobalVectorP(elbowJoint.basisAbs, elbowJoint.resultAbs, false, elbowJoint.normalAbs, true).setXz(), r1));
            bones.Add(new Bone(new OriginGlobalVectorP(elbowJoint.resultAbs, elbowJoint.targetAbs, false, elbowJoint.normalAbs, true).setXz(), r2));
            bones.Add(new Bone(new OriginRotPosP(), footPlatformHeight));
            bonesGeometry = MUtil2.al<Transform>(null, null, null); //two bones + foot
        }

        public override void updateData() {
            MUtil.logEvent(this, "updateData");
            
//            rotJoint.axisRel = axis;
            rotJoint.axisRel = rotJoint.axisRel.normalized;
            rotJoint.secondaryAxisRel = rotJoint.secondaryAxisRel.normalized;
            elbowJoint.radius1 = r1;
            elbowJoint.radius2 = r2;
            bones[0].r = r1;
            bones[1].r = r2;
            bones[2].r = footPlatformHeight;
            footLocal = new Vector3(0, footPlatformHeight, 0);
            minLen = Math.Abs(r1 - r2) * 1.1f;
            maxLen = r1 + r2;


            base.updateData();
        }

        public override void tickStructure(float dt) {
            Vector3 platform = targetRot.rotate(new Vector3(0, footPlatformHeight, 0));
            Vector3 target2 = targetPos + platform;
            
            rotJoint.targetAbs.v = useLimits ? target2.clampAround(basePos, minLen, maxLen) : target2;
            limitedResultTarget = rotJoint.targetAbs.v - platform;
            
            rotJoint.basisAbs.v = basePos;
            rotJoint.calcAbs(dt, basePos, baseRot);

            rotJoint.tick(dt);
            elbowJoint.tick(dt);
            
            elbow = elbowJoint.resultAbs.v;
            
            comfort = rotJoint.targetAbs.v.dist(basePos) / maxLen;

            ((OriginRotPosP) bones[2].origin).pos.v = rotJoint.targetAbs.v + targetRot.rotate(new Vector3(0, -footPlatformHeight, 0));
            ((OriginRotPosP) bones[2].origin).rot.v = targetRot;

            isInError |= elbowJoint.isInError;
        }

        public override bool canBeSolved() {
            return true;
        }
    }
}
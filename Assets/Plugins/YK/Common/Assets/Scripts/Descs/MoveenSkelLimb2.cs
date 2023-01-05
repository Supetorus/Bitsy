using System;
using moveen.core;
using moveen.utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace moveen.descs {
    public class MoveenSkelLimb2 : MoveenSkelWithBones {
        [CustomSkelControl]
        public float rA = 1;
        [CustomSkelControl]
        public float rB = 1;
        [CustomSkelControl]
        public float rC = 1;
        [ReadOnly]public float r1 = 1;
        [ReadOnly]public float r2 = 1;

        [Range(0, 1)]
        [CustomSkelControl(min=0f, max=1f)]
        public float styleRatio = 0.5f;
//        [CustomSkelControl]
        public bool style = true; 
//        [CustomSkelControl]
        public bool z = true;

        public P<Vector3> elbowAbs1 = new P<Vector3>();
        public P<Vector3> elbowAbs2 = new P<Vector3>();
        
        [CustomSkelResult]
        [NonSerialized]
        public Vector3 elbow1;
        [CustomSkelResult]
        [NonSerialized]
        public Vector3 elbow2;

        
        public float footPlatformHeight = 0.4f;

        [FormerlySerializedAs("rotationJoint")]//31.08.17
        [CustomSkelControlAttribute]
        public JunctionSimpleSpider rotJoint = new JunctionSimpleSpider();
        public readonly JunctionElbow invisibleElbowJoint = new JunctionElbow();
        public readonly JunctionElbow visibleElbowJoint = new JunctionElbow();

        public MoveenSkelLimb2() {
            rotJoint.basisAbs = new P<Vector3>();
            rotJoint.targetAbs = new P<Vector3>();

            invisibleElbowJoint.basisAbs = rotJoint.basisAbs;
            invisibleElbowJoint.targetAbs = rotJoint.targetAbs;
            invisibleElbowJoint.normalAbs = rotJoint.resultAbs;

            visibleElbowJoint.basisAbs = new P<Vector3>();
            visibleElbowJoint.targetAbs = new P<Vector3>();
            //visibleElbowJoint.normalAbs = rotJoint.resultAbs;
            visibleElbowJoint.normalAbs = new P<Vector3>(new Vector3());//because we need to use 'z' at runtime

            bones.Clear();
            bones.Add(new Bone(new OriginGlobalVectorP(invisibleElbowJoint.basisAbs, elbowAbs1, false, rotJoint.resultAbs, true).setXz(), rA));
            bones.Add(new Bone(new OriginGlobalVectorP(elbowAbs1, elbowAbs2, false, rotJoint.resultAbs, true).setXz(), rB));
            bones.Add(new Bone(new OriginGlobalVectorP(elbowAbs2, invisibleElbowJoint.targetAbs, false, rotJoint.resultAbs, true).setXz(), rC));
            bones.Add(new Bone(new OriginRotPosP(), footPlatformHeight));
            bonesGeometry = MUtil2.al<Transform>(null, null, null, null); //three bones + foot
        }


        public override void updateData() {
            MUtil.logEvent(this, "updateData");
            rotJoint.axisRel = rotJoint.axisRel.normalized;
            rotJoint.secondaryAxisRel = rotJoint.secondaryAxisRel.normalized;
            
            //((SkelLeg2) legSkel).updateRadiuses(step.footPlatformHeight, rA + rC * style, rB + rC * (1 - style), rA, rB, rC);

            
            if (style) {
                r1 = rA + styleRatio * rB;
                r2 = maxLen - r1;
                visibleElbowJoint.radius1 = rB;
                visibleElbowJoint.radius2 = rC;
            } else {
                r2 = rC + styleRatio * rB;
                r1 = maxLen - r2;
                visibleElbowJoint.radius1 = rA;
                visibleElbowJoint.radius2 = rB;
            }

            maxLen = rA + rB + rC;
//            minLen = 0;
            minLen = Math.Abs(r1 - r2) * 1.1f;
//            minLen = Math.Max(rA, Math.Max(rB, Math.Max(rC, Math.Abs(r1 - r2))));
            
            
            invisibleElbowJoint.radius1 = r1;
            invisibleElbowJoint.radius2 = r2;
            
            bones[0].r = rA;
            bones[1].r = rB;
            bones[2].r = rC;
            bones[3].r = footPlatformHeight;
            footLocal = new Vector3(0, footPlatformHeight, 0);

            base.updateData();
        }

        public override void tickStructure(float dt) {
            Vector3 platform = targetRot.rotate(new Vector3(0, footPlatformHeight, 0));
            Vector3 target2 = targetPos + platform;
            
            rotJoint.targetAbs.v = useLimits ? target2.clampAround(basePos, minLen, maxLen) : target2;
            limitedResultTarget = rotJoint.targetAbs.v - platform;
            comfort = rotJoint.targetAbs.v.dist(basePos) / maxLen;
//            MUtil.log(this, "limitedResultTarget: " + limitedResultTarget.ToString("R"));
//            Debug.Log("limitedResultTarget: " + limitedResultTarget.ToString("F4"));
            rotJoint.basisAbs.v = basePos;
            rotJoint.calcAbs(dt, basePos, baseRot);

            rotJoint.tick(dt);
            invisibleElbowJoint.tick(dt);
            visibleElbowJoint.normalAbs.v = rotJoint.resultAbs.v.mul(z ? -1 : 1);

            if (style) {

                elbowAbs1.v = invisibleElbowJoint.basisAbs.v.mix(invisibleElbowJoint.resultAbs.v, rA / r1);
                

                visibleElbowJoint.basisAbs.v = elbowAbs1.v;
                visibleElbowJoint.targetAbs.v = invisibleElbowJoint.targetAbs.v;
                visibleElbowJoint.tick(dt);

                elbowAbs2.v = visibleElbowJoint.resultAbs.v;
                
            } else {

                elbowAbs2.v = invisibleElbowJoint.targetAbs.v.mix(invisibleElbowJoint.resultAbs.v, rC / r2);

                visibleElbowJoint.basisAbs.v = invisibleElbowJoint.basisAbs.v;
                visibleElbowJoint.targetAbs.v = elbowAbs2.v;
                visibleElbowJoint.tick(dt);

                elbowAbs1.v = visibleElbowJoint.resultAbs.v;
                
            }

            elbow1 = elbowAbs1.v;
            elbow2 = elbowAbs2.v;
//            if (!Application.isPlaying) {
//                elbow1 = transform.InverseTransformPoint(elbowAbs1.v);
//                elbow2 = transform.InverseTransformPoint(elbowAbs2.v);
//            }

            ((OriginRotPosP) bones[3].origin).pos.v = rotJoint.targetAbs.v + targetRot.rotate(new Vector3(0, -footPlatformHeight, 0));
            ((OriginRotPosP) bones[3].origin).rot.v = targetRot;

            isInError |= invisibleElbowJoint.isInError | visibleElbowJoint.isInError;
        }

//        public override void createRagdoll() {
//            for (int i = 0; i < bones.Count; i++) {
//                Bone bone = bones[i];
//                createCapsule(bone.attachedGeometry, bone.deltaRot, bone.r);
//            }
//            for (int i = 0; i < bones.Count - 1; i++) connectToParent2(bones[i + 1].attachedGeometry, bones[i].attachedGeometry);
//        }

        public override bool canBeSolved() {
            return true;
        }

    }
}
using moveen.core;
using moveen.utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace moveen.descs {
    public class MoveenSkelRotSushi : MoveenSkelWithBones {

        [CustomSkelControl]
        public float r1 = 1;
        public float r2 = 1;

        [CustomSkelResult]
        public Vector3 elbowAbs;

        [FormerlySerializedAs("rotationJoint")]//31.08.17
        [CustomSkelControlAttribute]
        public JunctionSimpleSpider rotJoint = new JunctionSimpleSpider();
        private JunctionSushi elbowJoint = new JunctionSushi();
        
        public MoveenSkelRotSushi() {
            rotJoint.basisAbs = new P<Vector3>();
            rotJoint.targetAbs = new P<Vector3>();

            elbowJoint.basisAbs = rotJoint.basisAbs; 
            elbowJoint.targetAbs = rotJoint.targetAbs; 
            elbowJoint.normalAbs = rotJoint.resultAbs;


            bones.Add(new Bone(new OriginGlobalVectorP(elbowJoint.basisAbs, elbowJoint.resultAbs, false, elbowJoint.normalAbs, true), r1));
            bones.Add(new Bone(new OriginGlobalVectorP(elbowJoint.targetAbs, elbowJoint.resultAbs, false, elbowJoint.normalAbs, true), r1));
            bonesGeometry = MUtil2.al<Transform>(null, null); //two bones + foot
        }

        //TODO call when needed only
        public override void updateData() {
            rotJoint.axisRel = rotJoint.axisRel.normalized;
            rotJoint.secondaryAxisRel = rotJoint.secondaryAxisRel.normalized;
            elbowJoint.radius1 = r1;
            bones[0].r = r1;
            bones[1].r = r2;//r2 is too large usually
            maxLen = MyMath.sqrt(r1 * r1 + r2 * r2);

            base.updateData();//AFTER self ticks, because we want new info in bones
        }

        public override void tickStructure(float dt) {
            rotJoint.targetAbs.v = useLimits ? targetPos.clampAround(basePos, r1 * 1.1f, maxLen) : targetPos;
            limitedResultTarget = rotJoint.targetAbs.v;

            rotJoint.basisAbs.v = basePos;
            rotJoint.calcAbs(dt, basePos, baseRot);

            rotJoint.tick(dt);
            
            elbowJoint.tick(dt);
            elbowAbs = elbowJoint.resultAbs.v;

            comfort = rotJoint.targetAbs.v.dist(basePos) / maxLen;

        }

    }
}
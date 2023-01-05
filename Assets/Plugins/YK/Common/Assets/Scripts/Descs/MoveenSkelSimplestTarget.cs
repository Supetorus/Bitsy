using System.Collections.Generic;
using moveen.core;
using moveen.utils;
using UnityEngine;

namespace moveen.descs {
    public class MoveenSkelSimplestTarget : MoveenSkelWithBones {
        OriginRotPosP origin = new OriginRotPosP();
        
        public MoveenSkelSimplestTarget() {
            bones.Add(new Bone(origin, 1));
            bonesGeometry = new List<Transform> {null};
        }

        public override void tickStructure(float dt) {
            origin.pos.v = useLimits ? targetPos.clampAround(basePos, minLen, maxLen) : targetPos;
            origin.rot.v = targetRot;
            comfort = origin.pos.v.dist(basePos) / maxLen;
            limitedResultTarget = origin.pos.v;
        }
    }
}
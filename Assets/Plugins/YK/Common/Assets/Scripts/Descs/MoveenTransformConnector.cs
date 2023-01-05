using System;
using System.Collections.Generic;
using moveen.core;
using moveen.utils;
using UnityEngine;

namespace moveen.descs {
    //TODO get rid
    public class MoveenTransformConnector : MoveenSkelWithBones {
        [NonSerialized] private OriginRotPosP origin = new OriginRotPosP();

        public MoveenTransformConnector() {
            executionOrder = 10;
            bones.Add(new Bone(origin, 0));
            bonesGeometry = new List<Transform> {null};
        }

        public override void tickStructure(float dt) {
            origin.pos.v = transform.position;
            origin.rot.v = transform.rotation;
        }
    }
}
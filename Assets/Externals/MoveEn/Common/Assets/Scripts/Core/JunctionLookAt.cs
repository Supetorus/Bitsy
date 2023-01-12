using moveen.utils;
using UnityEngine;

namespace moveen.core {
    public class JunctionLookAt : JunctionBase {
        public Vector3 YRel = new Vector3(0, 1, 0);
        public P<Vector3> YAbs = new P<Vector3>();

        public Quaternion resultRot;

        //TODO use rot somehow (in editor only?)
        public override void calcAbs(float dt, Vector3 pos, Quaternion rot) {
            basisAbs.v = pos;
            YAbs.v = rot.rotate(YRel);
        }

        public override void tick(float dt) {
            resultRot = MUtil.qToAxes(targetAbs.v - basisAbs.v, YAbs.v);
        }

    }
}
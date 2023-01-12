using System;
using moveen.utils;
using UnityEngine;

namespace moveen.core {
    public class JunctionDirToAngle : JunctionBase {
        public float lenFrom = 10;
        public float lenTo = 1;
        public float angleFrom = 0;
        public float angleTo = (float) Math.PI;

        public override void tick(float dt) {

            float l = MyMath.to01(MyMath.clamp((targetAbs.v - basisAbs.v).length(), lenFrom, lenTo), lenFrom, lenTo);
            float angle = MyMath.mix(angleFrom, angleTo, l);

            Vector3 X = (targetAbs.v - basisAbs.v).normalized;
            Vector3 Y = normalAbs.v.crossProduct(X);
            resultAbs.v = basisAbs.v + X * (float) Math.Cos(angle) + Y * (float) Math.Cos(angle);
        }

    }
}
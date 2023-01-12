using moveen.utils;
using UnityEngine;

namespace moveen.core {
    public class JunctionElbow : JunctionBase {

        public float radius1;
        public float radius2;
        public bool isInError;

        public override void tick(float dt) {
            isInError = false;
            float len = (targetAbs.v - basisAbs.v).length();
//            len = Math.Max(len, Math.Abs(radius1 - radius2)*1.01f);
//            len = Math.Min(len, Math.Abs(radius1 + radius2)*0.99f); - must not happen
            if (SimpleIK.intersection(radius1, len, radius2)) {
//            if (SimpleIK.intersection(0, 0, radius1, len, 0, radius2)) {
                if (SimpleIK.yi > 0) {
                    resultAbs.v = calc(new Vector2(SimpleIK.xi, SimpleIK.yi));
                } else {
                    resultAbs.v = calc(new Vector2(SimpleIK.xi_prime, SimpleIK.yi_prime));
                }
            } else {
                isInError = true;
                float ratio = radius1 / (radius1 + radius2);
                if (!float.IsInfinity(ratio)) resultAbs.v = basisAbs.v.mix(targetAbs.v, ratio);
            }

            if (float.IsNaN(resultAbs.v.x) || float.IsNaN(resultAbs.v.y) || float.IsNaN(resultAbs.v.z)) {
                isInError = true;
                Debug.Log("!" + resultAbs.v + " " + radius1 + " " + radius2);
                Debug.Log("!!" + targetAbs.v);
            }
        }
        public virtual Vector3 calc(Vector2 res1) {
            Vector3 x = (targetAbs.v - basisAbs.v).normalized().mul(res1.x);
            Vector3 y = normalAbs.v.crossProduct(targetAbs.v - basisAbs.v).normalized().mul(res1.y);
            return x + y + basisAbs.v;
        }
    }
}

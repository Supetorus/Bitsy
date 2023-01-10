

//фактически - три сустава + три круга для расчёта (а не как в EblowJoint2)

using moveen.utils;
using UnityEngine;
using SimpleIK = moveen.utils.SimpleIK;

namespace moveen.core {
    public class JunctionElbow3 : JunctionBase {
        public float radius1;
        public float radius2;
        public float radius3;

        public Vector3 smthng1;
        public Vector3 smthng2;

        public Vector3 midCirclePos;

        public override void tick(float dt) {
            // input: basisAbs, targetAbs, normal
            //  output: resultAbs

            Vector3 b2t = targetAbs.v - basisAbs.v;
            midCirclePos = basisAbs.v + (targetAbs.v - basisAbs.v) * ((radius1 + radius2) / (radius1 + radius2 + radius2 + radius3) );
//        midCirclePos = (targetAbs.v + basisAbs.v) / 2;



            if (SimpleIK.intersection(0, 0, radius1, (midCirclePos - basisAbs.v).length(), 0, radius2))  {
                if (SimpleIK.yi > 0) smthng1 = calc(new Vector2(SimpleIK.xi, SimpleIK.yi), basisAbs.v, midCirclePos, normalAbs.v);
                else smthng1 = calc(new Vector2(SimpleIK.xi_prime, SimpleIK.yi_prime), basisAbs.v, midCirclePos, normalAbs.v);
            }

            if (SimpleIK.intersection(0, 0, radius3, (midCirclePos - targetAbs.v).length(), 0, radius2))  {
                if (SimpleIK.yi > 0) smthng2 = calc(new Vector2(SimpleIK.xi, SimpleIK.yi), targetAbs.v, midCirclePos, normalAbs.v);
                else smthng2 = calc(new Vector2(SimpleIK.xi_prime, SimpleIK.yi_prime), targetAbs.v, midCirclePos, normalAbs.v);
            }
//        if (SimpleIK.intersection(0, 0, radius2, (smthng1 - targetAbs.v).length(), 0, radius3))  {
//            if (SimpleIK.yi > 0) smthng2 = calc(new Vector2(SimpleIK.xi, SimpleIK.yi), smthng1, targetAbs.v, normal.v);
//            else smthng2 = calc(new Vector2(SimpleIK.xi_prime, SimpleIK.yi_prime), smthng1, targetAbs.v, normal.v);
//        }
        }

        public static Vector3 calc(Vector2 res1, Vector3 basis, Vector3 target, Vector3 normal) {
            Vector3 x = (target - basis).normalized().mul(res1.x);
            Vector3 y = ExtensionMethods.mul(normal.crossProduct(target - basis).normalized(), res1.y);
            return x + y + basis;
        }

    }
}
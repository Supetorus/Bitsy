using System;
using moveen.utils;
using UnityEngine;

namespace moveen.core {
    public class JunctionSushi : JunctionBase {

        public float radius1;
        public float resultRadius2;

        public override void tick(float dt) {
            Vector3 ab = targetAbs.v.sub(basisAbs.v);

            Vector3 n = normalAbs.v.crossProduct(ab).normalized();

            float c = ab.length();//гипотенуза
            float a = radius1;//катет
            float b = (float) Math.Sqrt(c * c - a * a);//катет
            if (c < a) return;
            float h = a * b / c;//высота
            float c1 = (float) Math.Sqrt(a * a - h * h);//расстояние от basis, до высоты
            resultAbs.v = basisAbs.v.add(ab.normalized(c1).add(n.mul(h)));
            resultRadius2 = b;
        }
        
    }
}
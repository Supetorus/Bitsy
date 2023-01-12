using System.Collections.Generic;
using moveen.utils;
using UnityEngine;

namespace moveen.core {
    public class OriginGlobalVectorP : OriginBase {
        //or make interface "Origin" ?
        public bool forcedRotationEnabled;

        public Quaternion forcedRotation;

        public P<Vector3> O;
        public P<Vector3> X;
        public P<Vector3> Y;

        //TODO refactor (rename OXY?)
        public bool xz;

        //true - axis is a normalized RAY, false - axis is |X - O| (ie stated through two points with any distance) 
        public bool xIsNormalizedRay;
        public bool yIsNormalizedRay;

        public Vector3 x;
        public Vector3 y;
        public Vector3 z;

        public bool invertY;

        private Vector3 pos;
        private Quaternion rot;

        public OriginGlobalVectorP() {
        }

        public OriginGlobalVectorP(P<Vector3> o, P<Vector3> x, P<Vector3> y) {
            O = o;
            X = x;
            Y = y;
        }

        public OriginGlobalVectorP(P<Vector3> o, P<Vector3> x, bool xIsNormalizedRay, P<Vector3> y, bool yIsNormalizedRay) {
            O = o;
            X = x;
            Y = y;
            this.xIsNormalizedRay = xIsNormalizedRay;
            this.yIsNormalizedRay = yIsNormalizedRay;
        }

        public OriginGlobalVectorP setXz() {
            xz = true;
            return this;
        }

        public OriginGlobalVectorP setInvertY(bool invertY) {
            this.invertY = invertY;
            return this;
        }

        public OriginGlobalVectorP(List<P<Vector3>> oxy) {
            O = oxy[0];
            X = oxy[1];
            Y = oxy[2];
        }

        public Vector3 toLocal(Vector3 abs) {
            abs = abs.sub(O.v);
            return new Vector3(x.scalarProduct(abs), y.scalarProduct(abs), z.scalarProduct(abs));
        }

        public Vector3 toGlobal(Vector3 local) {
            return x.mul(local.x)
                .add(y.mul(local.y))
                .add(z.mul(local.z))
                .add(O.v);
        }

        private Quaternion getRotation() {
//            Debug.Log("getRotation: " + O.v + " " + X.v + " " + Y.v);
            if (forcedRotationEnabled) return forcedRotation;

            if (xz) {
                x = xIsNormalizedRay ? X.v : (X.v - O.v).normalized;
                y = (yIsNormalizedRay ? Y.v : (Y.v - O.v).normalized).crossProduct(x);
                z = x.crossProduct(y);
            } else {
                x = xIsNormalizedRay ? X.v : (X.v - O.v).normalized;
                z = x.crossProduct(yIsNormalizedRay ? Y.v : (Y.v - O.v).normalized);
                y = z.crossProduct(x);
            }

            return MUtil.qToAxes(x, y, z);
//            return MUtil.matrix2quaternion(x, y, z);
        }

        public Vector3 getMidPosition() {
            return (O.v + X.v) / 2;
        }

        public override void tick() {
            rot = getRotation();
        }

        public override Matrix4x4 getMatrix() {
            throw new System.NotImplementedException();
        }

        public override Vector3 getPos() {
            return O.v;
        }

        public override Quaternion getRot() {
            return rot;
        }
    }
}
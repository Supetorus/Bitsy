using System;
using moveen.core;
using moveen.utils;
using UnityEngine;

namespace moveen.example {
    [Serializable]
    public class SurfaceDetector2 : ISurfaceDetector {
        [Tooltip("The layer which will be used to detect surface. For example, create a layer for objects you want to walk on and name it as \"ground\" or \"terrain\"")]
        public LayerMask layer = 0;
        [Tooltip("Don't consider back surface as walkable")]
        public bool ignoreBackSurface = true;
        [Tooltip("A surface can be detected -beyond- origin point. This number states how far beyond detector have to look. Important to consider model size and ceilings height")]
        public float detectOffset = 5;//TODO make it small by default
        [Tooltip("How far detector should look. Important to consider step size")]
        public float maxDetectDistance = 30;//TODO make it small by default
        [Tooltip("Magic for low ceilings and crossed floors")]
        public float preferLowerThanHigher = 2;
        [Tooltip("Maximum number of crosses detected. Consider simplification of your scene instead of increasing of this number")]
        public int bufferSize = 10;

        [NonSerialized] private RaycastHit[] buffer;
        [NonSerialized] public bool resetMaxFoundHits;
        [NonSerialized] public int maxFoundHits;

        public override Vector3 detect(Vector3 input, Vector3 normal) {
            if (resetMaxFoundHits) {
                resetMaxFoundHits = false;
                maxFoundHits = 0;
            }
            if (buffer == null || buffer.Length != bufferSize) buffer = new RaycastHit[bufferSize];
            Vector3 origin = input + normal * detectOffset;
            int foundHits = Physics.RaycastNonAlloc(origin, -normal, buffer, maxDetectDistance, layer.value, QueryTriggerInteraction.Ignore);
            maxFoundHits = MyMath.max(foundHits, maxFoundHits);

            RaycastHit closest = new RaycastHit();
            float minDist = float.MaxValue;
            found = false;
            if (foundHits > 0) {
                for (int i = 0; i < foundHits; i++) {
                    RaycastHit curHit = buffer[i];
                    if (ignoreBackSurface && curHit.normal.scalarProduct(normal) < 0) continue;

//                    if (curHit.point.sub(input).scalarProduct(normal) > 0) continue;

                    Vector3 relative = curHit.point.sub(input);
                    float curDist = relative.scalarProduct(normal);
                    if (curDist > 0) curDist *= preferLowerThanHigher;//we want to make upper intersections less attractive than lower ones 
                    else curDist = -curDist;
                    curDist = Math.Abs(curDist);
                    if (curDist < minDist) {
                        minDist = curDist;
                        closest = curHit;
                        found = true;
                    }
                }
            }


            if (found) {
                pos = closest.point;
                this.normal = closest.normal;
            } else {
//                Debug.Log("unexpected 100500 for " + input.ToString("F4") + " " + normal.ToString("F4"));
                pos = pos + normal.mul(-100500);
                this.normal = normal;
            }
            return pos;
        }
    }
}
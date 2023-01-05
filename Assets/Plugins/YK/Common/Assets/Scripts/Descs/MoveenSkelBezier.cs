using System;
using System.Collections.Generic;
using moveen.utils;
using UnityEngine;

namespace moveen.descs {
    public class MoveenSkelBezier : MoveenSkelBase {
        public float radius1 = 1;
        public float radius2 = 1;

        public int stepsCount = 10;

        [Tooltip("Orientation not connected to origins. Can be faster")]
        public bool simpleOrientation = false;

        [NonSerialized] public List<Vector3> poss = new List<Vector3>();
        [NonSerialized] public List<Quaternion> rots = new List<Quaternion>();

        public override void updateData() {
            base.updateData();
            MUtil.madeCount(poss, stepsCount + 2);
            MUtil.madeCount(rots, stepsCount + 2);
        }

        public override void tick(float dt) {
            base.tick(dt);


//            fromRot = Quaternion.identity;
//            toRot = target.rotation.rotSub(fromRot);
//            a1 = transform.InverseTransformPoint(a1);
//            b1 = transform.InverseTransformPoint(a1);
//            a2 = transform.InverseTransformPoint(a2);
//            b2 = transform.InverseTransformPoint(a2);

            fillOrigins(
                transform.position,
                radius1,
                transform.rotation, 
                targetPos,
                radius2,
                targetRot, 
                poss, 
                rots);

        }

        public void fillOrigins(Vector3 xA1, float ar, Quaternion fromRot, Vector3 xB1, float br, Quaternion toRot, List<Vector3> poss, List<Quaternion> rots) {
            Vector3 vA = fromRot.rotate(new Vector3(ar, 0, 0));
            Vector3 vB = toRot.rotate(new Vector3(-br, 0, 0));
            
            Vector3 xA2 = xA1 + vA;
            Vector3 xB2 = xB1 + vB;
            
            poss[0] = xA1;

            Vector3 Z = fromRot.rotate(new Vector3(0, 0 ,1));
//            Vector3 Z = -vA.crossProduct(xB1 - xA1);
            Quaternion startRotDif = Quaternion.identity;
            Quaternion endRotDif = Quaternion.identity;
            if (simpleOrientation) {
                rots[0] = MUtil.qToAxesXZ(vA, Z);
            } else {
                startRotDif = fromRot.rotSub(MUtil.qToAxesXZ(vA, Z));
                endRotDif = toRot.rotSub(MUtil.qToAxesXZ(-vB, Z));
                rots[0] = fromRot;
            }

            Vector3 lastPos = poss[0];
            for (int i = 1; i < poss.Count; i++) {
                float progress = (float) i / (poss.Count - 1);

                Vector3 bb = xA2.mix(xB2, progress);
                Vector3 currentPos = xA1.mix(bb, progress).mix(bb.mix(xB1, progress), progress);
                poss[i] = currentPos;
                rots[i] = MUtil.qToAxesXZ(currentPos - lastPos, Z);
                if (!simpleOrientation) rots[i] = rots[i] * startRotDif.nlerp(endRotDif, progress);

                lastPos = currentPos;
            }
            if (!simpleOrientation) rots[rots.Count - 1] = toRot;
        }

        public void OnDrawGizmos() {
            for (int i = 0; i < poss.Count - 1; i++) {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(poss[i], poss[i + 1]);
            }

            for (int i = 0; i < poss.Count; i++) {
                paintOrigin(poss[i], rots[i]);
            }
            Gizmos.color = Color.red;
            
            Vector3 a2 = transform.rotation.rotate(new Vector3(radius1, 0, 0)) + transform.position;
            Vector3 b2 = targetRot.rotate(new Vector3(-radius2, 0, 0)) + targetPos;
            
            Gizmos.DrawLine(transform.position, a2);
            Gizmos.DrawLine(targetPos, b2);

        }

        private static void paintOrigin(Vector3 last, Quaternion lastRot, float size = 0.1f) {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(last, lastRot.rotate(new Vector3(size, 0, 0)));
            Gizmos.color = Color.green;
            Gizmos.DrawRay(last, lastRot.rotate(new Vector3(0, size, 0)));
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(last, lastRot.rotate(new Vector3(0, 0, size)));
        }
    }
}


//TODO quaternion tutorial, testing scene
//            Quaternion startRotDif = MUtil.qToAxesXZ(vA, Z).conjug() * fromRot;
//            Quaternion startRotDif2 = MUtil.qToAxesXZ(vA, Z) * fromRot.conjug();
//            Quaternion startRotDif3 = fromRot.conjug() * MUtil.qToAxesXZ(vA, Z);
//            Quaternion startRotDif4 = fromRot * MUtil.qToAxesXZ(vA, Z).conjug();
            

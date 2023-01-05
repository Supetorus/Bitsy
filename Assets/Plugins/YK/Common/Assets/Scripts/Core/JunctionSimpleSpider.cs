using System;
using moveen.descs;
using moveen.utils;
using UnityEngine;
using UnityEngine.Serialization;
using MyMath = moveen.utils.MyMath;

namespace moveen.core {
    [Serializable]
    public class JunctionSimpleSpider : JunctionBase {

        //[CustomSkelControl(solve = false)]
        [Tooltip("Rotation axis. It defines \"up\" for this joint.\nIt is a good idea to keep this (0, 1, 0) and do an actual orientation with game object rotation.")]
        public Vector3 axisRel = new Vector3(0, 1, 0);
        [HideInInspector] public Vector3 axisAbs;
        
        [FormerlySerializedAs("axisForwardRel")]//01.12.17
        [FormerlySerializedAs("axisForward")]//TODO get rid (30.08.17)
        //[CustomSkelControl(solve = false)]
        [Tooltip("Axis that takes precedence when the target is closer to the pole of the joint. It is a good idea to keep it (1, 0, 0) (if the axis is kept (0, 1, 0)) and do an orientation by the game object rotation.")]
        public Vector3 secondaryAxisRel = new Vector3(1, 0, 0);
        [HideInInspector] public Vector3 secondaryAxisAbs;
        [HideInInspector] public Vector3 lastResult = new Vector3(1, 0, 0);

        [Range(0.1f, 50.0f)][Tooltip("Axis reaction speed if the target is around the equator")]
        public float rotSpeedAtEquator = 50;
        [Range(0.1f, 50.0f)][Tooltip("Axis reaction speed if the target is around the pole")]
        public float rotSpeedAtPole = 0.1f;

        [Range(0.0f, 10.0f)][Tooltip("Precedence strength of the pole axis (0 - the pole axis is off)")]
        public float poleAxisStrength;

        //TODO solve IK and tick normal in different methods
        public override void tick(float dt) {
            Vector3 targetLocal = targetAbs.v - basisAbs.v;
            float pole = -targetLocal.normalized.scalarProduct(axisAbs);
            float mul = MyMath.mix(rotSpeedAtEquator, rotSpeedAtPole, pole) * dt;
            Vector3 n = axisAbs;
            if (Application.isPlaying) //for battle mech 2 TODO place elbow at custom position
                if (poleAxisStrength > 0) n = n + secondaryAxisAbs.mul(pole * poleAxisStrength);
            resultAbs.v = lastResult.mix(targetLocal.crossProduct(n).normalized(), MyMath.min(mul, 1)).normalized;
            lastResult = resultAbs.v;
        }

        public override void calcAbs(float dt, Vector3 pos, Quaternion rot) {
            base.calcAbs(dt, pos, rot);
            axisAbs = rot.rotate(axisRel);
            secondaryAxisAbs = rot.rotate(secondaryAxisRel);
            if (poleAxisStrength > 0) secondaryAxisAbs = rot.rotate(secondaryAxisRel);
        }
    }
}

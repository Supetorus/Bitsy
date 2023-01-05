using moveen.descs;
using moveen.utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace moveen.example {
    /// <summary>
    /// target
    /// rotates its own GO
    /// limits
    /// ticks in tick (not in fixed tick)
    /// </summary>
    public class DirControl : OrderedMonoBehaviour {
        [BindWarning]
        [Tooltip("Target GameObject to look at")]
        public Transform target;

        //TODO remove limits?
        [Tooltip("Maximum allowed angle to rotate (degrees)")]
        public float maxAngle = 20;//degrees
        [Tooltip("If target is at larger angle than this, DirControl will cease orientation and will return body to the initial orientation")]
        public float maxPrepareAngle = 40;//degrees
        [ReadOnly] public float differenceDegrees;
        private Quaternion initialLocalRot;

        public DirControl() {
            participateInFixedTick = false;
            participateInTick = true;
        }

        public override void OnEnable() {
            base.OnEnable();
            initialLocalRot = transform.localRotation;
        }

        public override void OnValidate() {
            participateInFixedTick = false;
            participateInTick = true;
        }

        public override void tick(float dt) {
            if (target == null) return;
            Quaternion bestDirAbs = MUtil.qToAxes(target.position - transform.position, Vector3.up);
            Quaternion baseRotAbs = transform.parent == null ? initialLocalRot : transform.parent.rotation;
            Quaternion difference = baseRotAbs.rotSub(bestDirAbs);
            differenceDegrees = MyMath.abs(MyMath.angleNormalizeSigned(MyMath.acos(difference.w) * 2)) * 180f / MyMath.PI;
            if (differenceDegrees > maxPrepareAngle) bestDirAbs = baseRotAbs;//look forward if the angle is more than maxPrepareAngle
            else bestDirAbs = Quaternion.RotateTowards(baseRotAbs, bestDirAbs, maxAngle);//try to look to the target, but not more than maxAngle

            transform.rotation = bestDirAbs;
        }

    }
}
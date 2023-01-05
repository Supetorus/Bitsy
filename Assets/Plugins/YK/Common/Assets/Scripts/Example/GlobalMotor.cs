using System;
using moveen.core;
using moveen.descs;
using moveen.utils;
using UnityEngine;

namespace moveen.example {
    /// <summary>
    /// This component applies acceleration to the Rigidbody in order to reach target pos/rot.
    /// <para/>
    /// If there is RigidBody on its GameObject, then it will apply physical forces to it. If there is no RigidBody, it will apply forces to its own speed representation and move GameObject accordingly.
    /// <para/>

    
    /// As its impulses are preserved in local space, it moves with parent and its physics isn't distorted by parental movement.
    /// It can be useful for local, physics-independent animation - recoil, revolver, tank barrel, etc.
    /// 
    /// </summary>
    [ExecuteInEditMode]
    public class GlobalMotor : OrderedMonoBehaviour {
        [Tooltip("Enable position following")]
        public bool move = true;
        [Tooltip("Movement dynamics")]
        public MotorBean posMotor;
        [NonSerialized] public Vector3 speed;

        [Tooltip("Enable rotation following")]
        public bool rotate = true;
        [Tooltip("Rotation dynamics")]
        public MotorBean rotationMotor;
        [NonSerialized] public Vector3 angleSpeed;

        [NonSerialized] public Rigidbody rb;

        // public bool workInEditor;

        [BindWarning]
        public Transform target;

        public GlobalMotor() {
            participateInTick = false;
            participateInFixedTick = true;
        }

        public override void OnEnable() {
            base.OnEnable();
            if (target == null) return;
            rb = target.GetComponent<Rigidbody>();
        }

        public override void tick(float dt) {
        }

        public override void OnValidate() {
            participateInTick = false;
            participateInFixedTick = true;
        }

        public override void fixedTick(float dt) {
            if (target == null) return;

            if (rotate) {
                Quaternion curRotAbs = transform.rotation;
                Quaternion targetRotAbs = target.rotation;
                if (rb == null) {
                    angleSpeed += rotationMotor.getTorque(curRotAbs, targetRotAbs, angleSpeed) * dt;
                    Quaternion rot = MUtil.toAngleAxis(angleSpeed.length() * dt, angleSpeed.normalized);
                    transform.rotation = rot * curRotAbs;
                } else {
                    rb.AddTorque(rotationMotor.getTorque(curRotAbs, targetRotAbs, rb.angularVelocity), ForceMode.Acceleration);
                }
            }

            if (move) {
                Vector3 curPosAbs = transform.position;
                Vector3 targetPosAbs = target.position;
                if (rb == null) {
                    speed += posMotor.getAccel(targetPosAbs, curPosAbs, speed) * dt;
                    transform.position = curPosAbs + speed * dt;
                } else {
                    rb.AddForce(posMotor.getAccel(targetPosAbs, curPosAbs, rb.velocity), ForceMode.Acceleration);
                }
            }
        }

    }
}
using System;
using moveen.core;
using moveen.descs;
using moveen.editor;
using moveen.utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace moveen.example {
    /// <summary>
    /// This component tries to reach target pos/rot.
    /// As its impulses are preserved in local space, it moves with parent and its physics isn't distorted by parental movement.
    /// It can be useful for local, physics-independent animation - recoil, revolver, tank barrel, etc.
    /// </summary>
    [ExecuteInEditMode] //to fill local pos/rot
    public class LocalMotor : MoveenSkelBase {
        //deprecated for 0.83
        [Warning("DEPRECATED! Replace with GlobalMotor")]
        
        [Tooltip("Enable position following")]
        public bool move = true;
        [Tooltip("Movement dynamics")]
        public MotorBean posMotor;
        [NonSerialized] public Vector3 localSpeed;
        [ReadOnly] public Vector3 localAngularSpeed;

        [Tooltip("Enable rotation following")]
        public bool rotate = true;
        [Tooltip("Rotation dynamics")]
        public MotorBean rotationMotor;

        [Tooltip("Pursue target's local (instead of global) pos/rot")]
        public bool targetsLocal;

        private void updateLocal() {
            if (target == null) {
            } else {
                if (transform.parent == null) {
                    targetPosRel = target.position;
                    targetRotRel = target.rotation;
                } else {
                    if (targetsLocal) {
                        targetPosRel = target.localPosition;
                        targetRotRel = target.localRotation;
                    } else {
                        targetPosRel = transform.parent.InverseTransformPoint(target.position);
                        targetRotRel = target.rotation.rotSub(transform.parent.rotation);
                    }
                }
            }
        }

        public override void fixedTick(float dt) {
            updateLocal();
        }

        public override void setTarget(Vector3 targetPos, Quaternion targetRot) {
            if (transform.parent == null) {
                targetPosRel = targetPos;
                targetRotRel = targetRot;
            } else {
                if (targetsLocal) {
                } else {
                    targetPosRel = transform.parent.InverseTransformPoint(targetPos);
                    targetRotRel = targetRot.rotSub(transform.parent.rotation);
                }
            }
        }

        public override void tick(float dt) {
            if (!Application.isPlaying) {
                targetPosRel = transform.localPosition;
                targetRotRel = transform.localRotation;
                return;
            }
            updateLocal();
            
            if (!Application.isPlaying) return;
            if (rotate) {
                Quaternion localRot = transform.localRotation;
                Vector3 rotAccel = rotationMotor.getTorque(localRot, targetRotRel, localAngularSpeed);
                localAngularSpeed += rotAccel * dt;
                Quaternion rot = MUtil.toAngleAxis(localAngularSpeed.length() * dt, localAngularSpeed.normalized);
                transform.localRotation = rot * localRot;
            }

            if (move) {
                Vector3 localPos = transform.localPosition;
                Vector3 accel = posMotor.getAccel(targetPosRel, localPos, localSpeed);
                localSpeed += accel * dt;
                localPos += localSpeed * dt;
                transform.localPosition = localPos;
            }
        }

    }
}
using System;
using moveen.core;
using moveen.descs;
using moveen.utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace moveen.example {
    //TODO rename
    /// <summary>
    /// These classes encapsulate logic to control MoveEn. You can define speed, height, jump parameters, camera, and other.
    /// 
    /// This, MoveControl3, not controls the camera. It is supposed to be used by MoveenNpc1 class only.
    /// </summary>
    public class MoveControl3 : MonoBehaviour {
        [Header("   For NPC only")]
        [BindWarning]
        public MoveenStepper5 moveen;
        public Transform aimTarget;
        
        [Tooltip("Distance from body center to the ground")]
        public float height = 2;
        public float speed = 2;
        [Tooltip("Target rotation angular speed")]
        public float bodyRotReactionSpeed = 1;

        [Tooltip("Speed multiplication whe shift is pressed")]
        public float runSpeedMultiplier = 2;
//        [Tooltip("Adds additional jump force on the run")]
//        public float runJumpTime = 0.3f;
        [Range(0, 1)] public float strafeSpeedMultiplyer = 0.8f;
        [Range(0, 1)] public float rearSpeedMultiplyer = 0.5f;
        
        [Tooltip("Useful for multipeds to be aligned to terrain inclination")]
        public bool inclineBodyToLegs = true;
        public float inclineBodyToLegsRatio = 1;

        [Tooltip("If true, target position will switch sharply from point to point which produces more robotic like movements")]
        public bool quantCenter;
        [Tooltip("Quant size. Make it small for small models and big for big models. (Big for small models will make them walk by kind of grid. Small for big models will make quantCenter irrelevant)")]
        public float quantSize = 0.5f;

        private Rigidbody bodyRigid;
//        private float initialBodyLenHelp;
        
        [Header("Jump")]
        public float jumpPrepareHeightMul = 0.8f;
        public float jumpTargetHeightMul = 2;
        public float jumpPrepareTime = 1; 
        public MotorBean jumpMotor;
        [ReadOnly] public bool jumpPreparing;
        [ReadOnly] public bool jumpInProgress;
        [ReadOnly] public float jumpStrengthCurTime = 100;
        [NonSerialized] private readonly MotorBean previousVerticalMotor = new MotorBean(); 

        private Vector3 upByLegs;



        [Header("Debug")]
        public bool debugMoveForward;
        public bool debugFreezeRotation;
        public bool debugRotateRight;



        public void OnEnable() {
        }

        public void Start() {
            if (moveen != null) {
                bodyRigid = moveen.body == null ? moveen.gameObject.GetComponent<Rigidbody>() : moveen.body.GetComponent<Rigidbody>();
            }
        }

        public Quaternion chassisRot;
        public Vector3 moveDir;
        public Vector3 aimPos;

        public void FixedUpdate() {
            if (!enabled) return;
            if (bodyRigid == null) return;

            if (Application.isPlaying) calcUpByLegs();

            //calc h
            float h = 0;
            int dockedCount = 0;
            for (int i = 0; i < moveen.engine.steps.Count; i++) {
                Step2 step = moveen.engine.steps[i];

                if (!step.dockedState) {
                    h += step.bestTargetProgressiveAbs.y; //to rise sharper
                } else {
                    dockedCount++;
                    h += step.posAbs.y;
                }
//                h += step.bestTargetProgressiveAbs.y;
            }
            float targetHeightMultiplier = jumpPreparing ? jumpPrepareHeightMul : jumpInProgress ? jumpTargetHeightMul : 1;
            h = moveen.engine.steps.Count == 0 ? height : (h / moveen.engine.steps.Count + height * targetHeightMultiplier);

            moveen.engine.horizontalMotor.maxSpeed = speed;

            if (jumpStrengthCurTime < jumpPrepareTime) {
                if (jumpStrengthCurTime > 0) {
                    //set only after space was released (jump started), as while space is pressed jumpStrengthCurTime is 0
                    moveen.engine.verticalMotor.copyFrom(jumpMotor);
                    jumpInProgress = true;
                }
                jumpStrengthCurTime += Time.deltaTime;
                //revert old value when jump is ended
                if (jumpStrengthCurTime >= jumpPrepareTime) {
                    moveen.engine.verticalMotor.copyFrom(previousVerticalMotor);
                    jumpInProgress = false;
                }
            }
            //remember value from editor when not jumping
            if (jumpStrengthCurTime >= jumpPrepareTime) previousVerticalMotor.copyFrom(moveen.engine.verticalMotor);

            
            Vector3 add = moveDir.normalized;
            add *= speed / moveen.engine.horizontalMotor.distanceToSpeed;
            add = bodyRigid.transform.rotation.conjug().rotate(add); //global -> body local

            //limit forward, strafe, and rear speed
            if (add.x > 0) add.x = Math.Min(add.x, speed);
            if (add.x < 0) add.x = Math.Max(add.x, -speed * rearSpeedMultiplyer);
            if (add.z > 0) add.z = Math.Min(add.z, speed * strafeSpeedMultiplyer);
            if (add.z < 0) add.z = Math.Max(add.z, -speed * strafeSpeedMultiplyer);

            add = bodyRigid.transform.rotation.rotate(add); //body local -> global
            
            Vector3 newBestTargetPos = moveen.engine.imCenter.add(add).withSetY(h);
            if (quantCenter) {
                if (dockedCount == 0 || moveDir.length() > 0 || newBestTargetPos.dist(transform.position) > quantSize) transform.position = newBestTargetPos;
            } else {
                transform.position = newBestTargetPos;
            }

            Quaternion rotForTarget;
            if (inclineBodyToLegs) {
                Vector3 wantedForward = chassisRot.rotate(Vector3.right);
                Vector3 wantedUp = chassisRot.rotate(Vector3.up);
                Vector3 up = wantedUp.mix(upByLegs, inclineBodyToLegsRatio);
                rotForTarget = MUtil.qToAxesYX(up, wantedForward);
            } else {
                rotForTarget = chassisRot;
            }

            if (dockedCount > 0) {
                //rotate target only if at least one leg is touching ground
                float cosOfHalfAngle = (float) Math.Cos(bodyRotReactionSpeed / 2 / 180f * Math.PI); //TODO cache
                Quaternion rotDif = rotForTarget.rotSub(transform.rotation);
                if (rotDif.w < 0) rotDif = rotDif.mul(-1);
                if (rotDif.w < cosOfHalfAngle) rotDif = rotDif.normalizeWithFixedW(cosOfHalfAngle);
                transform.rotation = transform.rotation * rotDif;
            } else {
                //get rotation from the body when in air
                //  because else - body could accumulate rotation it will be looking weird after grounding
                transform.rotation = moveen.targetRot;
            }

            moveen.targetPos = transform.position;
            moveen.targetRot = transform.rotation;
        }

        private void calcUpByLegs() {
            Vector3 mid = new Vector3();
            for (int i = 0; i < moveen.engine.steps.Count; i++) {
                mid += moveen.engine.steps[i].bestTargetProgressiveAbs;
//                mid += moveen.engine.steps[i].legPosAbs.v;
            }
            mid /= moveen.engine.steps.Count;

            upByLegs = new Vector3();
            for (int i = 0; i < moveen.engine.steps.Count; i++) {
                Step2 step = moveen.engine.steps[i];
                Vector3 vec = step.bestTargetProgressiveAbs.sub(mid);
                if (!step.dockedState) vec = vec.mul(0.5f);
//                Vector3 vec = step.legPosAbs.v.sub(mid);
                upByLegs += vec.crossProduct(new Vector3(0, 1, 0)).crossProduct(vec).normalized;
            }
            upByLegs = upByLegs.normalized();
        }

        public void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.4f);
            if (Application.isPlaying) {
                Gizmos.DrawRay(transform.position, upByLegs * 2);
            }
        }

    }
}
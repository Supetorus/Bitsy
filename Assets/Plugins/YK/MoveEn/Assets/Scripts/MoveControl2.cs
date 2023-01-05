using System;
using System.Reflection;
using moveen.core;
using moveen.descs;
using moveen.utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace moveen.example {
    /// <summary>
    /// These classes encapsulate logic to control MoveEn. You can define speed, height, jump parameters, camera, and other.
    ///
    /// This, MoveControl2, places a camera in top-down view.
    /// </summary>
    public class MoveControl2 : MonoBehaviour {
        [Header("   \"Top down\" view")]
        [BindOrLocalWarning]
        public FocusGrabber focusGrabber;

        [BindWarning]
        public MoveenStepper5 moveen;
        public Transform aimTarget;
        
        [BindWarning]
        public Camera cam;
        [Tooltip("Distance from body center to the ground")]
        public float height = 2;
        [FormerlySerializedAs("walkLimit")]//03.11.17
        public float speed = 2;
        [Tooltip("Target rotation angular speed")]
        public float bodyRotReactionSpeed = 1;
        public float bodyRotLag = 0;

        public bool localWasd;
        public float camAheadMul = 2;

        [Tooltip("Speed multiplication when shift is pressed")]
        public float runSpeedMultiplier = 2;
        [Tooltip("Adds additional jump force on the run")]
        public float runJumpTime = 0.3f;
        [Range(0, 1)] public float strafeSpeedMultiplyer = 0.8f;
        [Range(0, 1)] public float rearSpeedMultiplyer = 0.5f;

        //TODO implement
        [Tooltip("Useful for multipeds to be aligned to terrain inclination")]
        public bool inclineBodyToLegs = true;
        public float inclineBodyToLegsRatio = 1;

        [Tooltip("If true, target position will switch sharply from point to point which produces more robotic like movements")]
        public bool quantCenter;
        [Tooltip("Quant size. Make it small for small models and big for big models. (Big for small models will make them walk by kind of grid. Small for big models will make quantCenter irrelevant)")]
        public float quantSize = 0.5f;

        [FormerlySerializedAs("camDif")]//03.11.17
        public Vector3 camPosition = new Vector3(-6, 2, 0);
        public Vector3 camRotation = new Vector3(60, 0, 0);
        
        //TODO understandable size
        public float camApproachFactor = 0.04f;
        private Vector3 wantedCamera;
        private Rigidbody bodyRigid;
        private Vector3 oldCam;
        private float initialBodyLenHelp;
        
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
            oldCam = cam.transform.position;
        }

        public void Start() {
            if (cam == null) return;
            if (moveen != null) {
                bodyRigid = moveen.body == null ? moveen.gameObject.GetComponent<Rigidbody>() : moveen.body.GetComponent<Rigidbody>();
            }
            if (focusGrabber == null) focusGrabber = GetComponent<FocusGrabber>();
        }

        public void FixedUpdate() {
            if (!enabled) return;
            if (bodyRigid == null) return;
            if (cam == null) return;

            bool grabInput = focusGrabber != null && focusGrabber.grab;

            if (Application.isPlaying) calcUpByLegs();

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

            float maxSpeed = speed;
            if (Input.GetKey(KeyCode.LeftShift)) {
                maxSpeed *= runSpeedMultiplier;
                moveen.engine.runJumpTime = runJumpTime;//it adds additional jump force on the step
            } else {
                moveen.engine.runJumpTime = 0;
            }
            moveen.engine.horizontalMotor.maxSpeed = maxSpeed;

            Vector3 add = new Vector3();
            bool movePressed = false;

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

            if (grabInput) {
                if (Input.GetKey(KeyCode.Space)) {
                    jumpStrengthCurTime = 0;
                    jumpPreparing = true;
                } else {
                    jumpPreparing = false;
                }
                Vector3 upDown = localWasd ? new Vector3(1, 0, 0) : new Vector3(0, 0, 1);
                Vector3 leftRight = localWasd ? new Vector3(0, 0, 1) : new Vector3(-1, 0, 0);

                if (Input.GetKey(KeyCode.W) || debugMoveForward) {
                    add += upDown;
                    movePressed = true;
                }
                if (Input.GetKey(KeyCode.S)) {
                    add -= upDown;
                    movePressed = true;
                }
                if (Input.GetKey(KeyCode.A)) {
                    add += leftRight;
                    movePressed = true;
                }
                if (Input.GetKey(KeyCode.D)) {
                    add -= leftRight;
                    movePressed = true;
                }
                add = add.normalized();
                add *= maxSpeed / moveen.engine.horizontalMotor.distanceToSpeed;
                if (!localWasd) {
                    add = bodyRigid.transform.rotation.conjug().rotate(add); //global -> body local
                }

                //limit forward, strafe, and rear speed
                if (add.x > 0) add.x = Math.Min(add.x, maxSpeed);
                if (add.x < 0) add.x = Math.Max(add.x, -maxSpeed * rearSpeedMultiplyer);
                if (add.z > 0) add.z = Math.Min(add.z, maxSpeed * strafeSpeedMultiplyer);
                if (add.z < 0) add.z = Math.Max(add.z, -maxSpeed * strafeSpeedMultiplyer);

                add = bodyRigid.transform.rotation.rotate(add); //body local -> global
            }
            
            Vector3 newBestTargetPos = moveen.engine.imCenter.add(add).withSetY(h);
            if (quantCenter) {
                if (dockedCount == 0 || movePressed || newBestTargetPos.dist(transform.position) > quantSize) transform.position = newBestTargetPos;
            } else {
                transform.position = newBestTargetPos;
            }

            wantedCamera = moveen.engine.imCenter.withSetY(h);


            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Quaternion wantedRot = transform.rotation;
            Vector3 hitPoint = new Vector3();

            bool wasHit = false;
            if (grabInput) {
                wasHit = Physics.Raycast(ray.origin, ray.direction, out hit); //TODO cached
                hitPoint = hit.point;
                if (wasHit) {
                    //lower target, because we are looking from the top, but the actor will be aiming from a side
                    //  but we don't want correct height for walls and terrain
//                    if (hit.normal.y > 0.5f && hit.point.y > hit.collider.transform.position.y) {
//                        hitPoint.y = hit.collider.transform.position.y;
//                    }
                    hitPoint.y += 5;
                    //Debug.DrawLine(new Vector3(-100, -100, -100), hitPoint, Color.red);

                    if (moveen.engine.imCenter.dist(hitPoint) > 1) {
                        wantedRot = MUtil.qToAxes(hitPoint - moveen.engine.imCenter, Vector3.up);
                    }
                }
            }

            oldCam = oldCam.mix(wantedCamera, camApproachFactor);
            if (grabInput) {
                cam.transform.position = cam.transform.position.mix(oldCam + camPosition + wantedRot.rotate(new Vector3(maxSpeed * camAheadMul, 0, 0)), 0.1f);
                cam.transform.eulerAngles = camRotation;
            }
//            cam.transform.position = oldCam + camPosition + wantedRot.rotate(new Vector3(maxSpeed, 0, 0));

            if (dockedCount > 0) {
                //rotate target only if at least one leg is touching ground
                float cosOfHalfAngle = (float) Math.Cos(bodyRotReactionSpeed / 2 / 180f * Math.PI); //TODO cache
                float cosOfHalfAngle2 = (float) Math.Cos(bodyRotLag / 2 / 180f * Math.PI); //TODO cache

                if (wantedRot.rotSub(quantedWantedRot).w < cosOfHalfAngle2) {
                    quantedWantedRot = wantedRot;
                }
                wantedRot = quantedWantedRot;
                Quaternion rotDif = wantedRot.rotSub(transform.rotation);


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

            //must be after this transform pos/rot change as target can be child of this
            if (wasHit && aimTarget != null) aimTarget.position = hitPoint;
        }

        private Quaternion quantedWantedRot = Quaternion.identity;

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
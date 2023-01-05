using System;
using moveen.core;
using moveen.descs;
using moveen.utils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace moveen.example {
    /// <summary>
    /// These classes encapsulate logic to control MoveEn. You can define speed, height, jump parameters, camera, and other.
    /// 
    /// This, MoveControl5, places the camera in "from behind" position.
    /// </summary>
    public class MoveControl5 : MonoBehaviour {
        [Header("   \"From behind\" view")]
        [BindOrLocalWarning]
        public FocusGrabber focusGrabber; 
        [BindWarning]
        public MoveenStepper5 moveen;
        [BindWarning]
        public Camera cam;
        [Tooltip("Distance from body center to the ground")]
        public float height = 2;
        [FormerlySerializedAs("walkLimit")]//03.11.17
        public float speed = 2;
        [Tooltip("Target rotation angular speed")]
        public float bodyRotReactionSpeed = 1;

        [Tooltip("Speed multiplication when shift is pressed")]
        public float runSpeedMultiplier = 2;
        [Tooltip("Adds additional jump force on the run")]
        public float runJumpTime = 0.3f;
        [Range(0, 1)] public float strafeSpeedMultiplyer = 0.8f;
        [Range(0, 1)] public float rearSpeedMultiplyer = 0.5f;
        
        [Tooltip("Useful for multipeds to be aligned to terrain inclination")]
        public bool inclineBodyToLegs = true;
        public float inclineBodyToLegsRatio = 1;

        [Tooltip("If true, target position will switch sharply from point to point which produces more robotic like movements")]
        public bool quantCenter;
        [Tooltip("Quant size. Make it small for small models and big for big models. (Big for small models will make them walk by kind of grid. Small for big models will make quantCenter irrelevant)")]
        public float quantSize = 0.5f;

        [FormerlySerializedAs("camDif")]//03.11.17
        public Vector3 camPosition = new Vector3(-6, 2, 0);
        //TODO understandable size
        public float camApproachFactor = 0.04f;
        private float camPitch;
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
            if (cam == null) return;
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
                if (Input.GetKey(KeyCode.W) || debugMoveForward) {
                    add += new Vector3(1, 0, 0);
                    movePressed = true;
                }
                if (Input.GetKey(KeyCode.S)) {
                    add += new Vector3(-1, 0, 0);
                    movePressed = true;
                }
                if (Input.GetKey(KeyCode.A)) {
                    add += new Vector3(0, 0, 1);
                    movePressed = true;
                }
                if (Input.GetKey(KeyCode.D)) {
                    add += new Vector3(0, 0, -1);
                    movePressed = true;
                }
                add = add.normalized();
                add *= (maxSpeed / moveen.engine.horizontalMotor.distanceToSpeed);

                //cam local
//                add = MUtil.toAngleAxis(camYaw, new Vector3(0, 1, 0)).rotate(add); //cam local -> global
//                add = bodyRigid.transform.rotation.conjug().rotate(add); //global -> body local


                if (add.x > 0) add.x = Math.Min(add.x, maxSpeed);
                if (add.x < 0) add.x = Math.Max(add.x, -maxSpeed * rearSpeedMultiplyer);
                if (add.z > 0) add.z = Math.Min(add.z, maxSpeed * strafeSpeedMultiplyer);
                if (add.z < 0) add.z = Math.Max(add.z, -maxSpeed * strafeSpeedMultiplyer);


//            add = transform.rotation.rotate(add); //body local -> global
                add = bodyRigid.transform.rotation.rotate(add); //body local -> global
//            transform.position = body.transform.position + body.transform.rotation.rotate(add);
            }
            
            Vector3 newBestTargetPos = moveen.engine.imCenter.add(add).withSetY(h);
            if (quantCenter) {
                if (dockedCount == 0 || movePressed || newBestTargetPos.dist(transform.position) > quantSize) transform.position = newBestTargetPos;
            } else {
                transform.position = newBestTargetPos;
            }

            float leftRight = 0;
            if (grabInput) {
                float my = Input.GetAxis("Mouse Y") * -0.02f / Time.timeScale;

                leftRight = (Input.mousePosition.x / Screen.width - 0.5f) * 2;
                if (debugFreezeRotation) leftRight = 0;
                if (debugRotateRight) leftRight = 0.5f;
                
                if (leftRight > 0) leftRight = MyMath.smoothstep(0, 1, leftRight);
                else leftRight = -MyMath.smoothstep(0, 1, -leftRight);

                camPitch = MyMath.clamp(camPitch + my, -1, 1);

            }

            
//            RaycastHit hit;
//            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

//            Quaternion yawRoll = Quaternion.AngleAxis(leftRight * 10, Vector3.up) * transform.rotation;
            Quaternion yawRoll = Quaternion.AngleAxis(leftRight * 10, Vector3.up) * MUtil.qToAxesYX(Vector3.up, transform.rotation.rotate(Vector3.right));
//            Quaternion yawRoll = Quaternion.FromToRotation(moveen.engine.realBodyRot.rotate(Vector3.right), hitPoint.sub(moveen.transform.position));
            
            
            
            Quaternion rotForCameraPos = yawRoll * MUtil.toAngleAxis(-camPitch*0.5f, new Vector3(0, 0, 1));
//            Quaternion rotForCameraLook = rotForCameraPos;
            Quaternion rotForCameraLook = yawRoll
                                          * MUtil.toAngleAxis((float) (Math.PI / 2), new Vector3(0, 1, 0))
                                          * MUtil.toAngleAxis(camPitch, new Vector3(1, 0, 0));


            
            
            
            
            Vector3 wantedDir = yawRoll.rotate(new Vector3(1, 0, 0));
//            if (wantedDir.scalarProduct(moveen.engine.inputWantedRot.rotate(Vector3.right)) > 0.95f) {
//                yawRoll = transform.rotation;
//                wantedDir = yawRoll.rotate(new Vector3(1, 0, 0));
//            }
            Quaternion rotForTarget;
            if (inclineBodyToLegs) {
                rotForTarget = MUtil.qToAxesYX(Vector3.up.mix(upByLegs, inclineBodyToLegsRatio), wantedDir);
//                rotForTarget = MUtil.qToAxes(yawRoll.rotate(new Vector3(1, 0, 0)), upByLegs);
            } else {
                rotForTarget = yawRoll * MUtil.toAngleAxis(camPitch * -0.5f, new Vector3(0, 0, 1));
            }


            if (grabInput) {
                Vector3 wantedCamera = moveen.engine.imCenter.withSetY(h);
                cam.transform.rotation = rotForCameraLook;
                oldCam = oldCam.mix(wantedCamera, camApproachFactor);
                cam.transform.position = oldCam + rotForCameraPos.rotate(camPosition);
            }

//            float reactionSpeed = bodyRotReactionSpeed;
//            if (bodyRigid != null) reactionSpeed /= Math.Max(1, bodyRigid.velocity.length() * bodyRotReactionSpeedSpeed);
            
            if (dockedCount > 0) {
                //rotate target only if at least one leg is touching ground
                float cosOfHalfAngle = (float) Math.Cos(bodyRotReactionSpeed / 2 / 180f * Math.PI); //TODO cache
                Quaternion rotDif = rotForTarget.rotSub(transform.rotation);
                if (rotDif.w < 0) rotDif = rotDif.mul(-1);
                if (rotDif.w < cosOfHalfAngle) rotDif = rotDif.normalizeWithFixedW(cosOfHalfAngle);
                transform.rotation = transform.rotation * rotDif;
//                transform.rotation = rotForTarget;
            } else {
                //get rotation from the body when in air
                //  because else - body could accumulate rotation it will be looking weird after grounding
                transform.rotation = moveen.targetRot;
            }

            moveen.targetPos = transform.position;
            moveen.targetRot = transform.rotation;

        
            //must be after this transform pos/rot change as target can be child of this
//            if (wasHit && aimTarget != null) aimTarget.position = hitPoint;
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
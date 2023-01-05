using System;
using moveen.core;
using moveen.utils;
using UnityEngine;

namespace moveen.descs {
    
    /// <summary>
    ///  The cornerstone of the whole library. Contains logic for stepping and pelvis+body animation. Most of the features advertised - located here.
    ///  <para/>
    /// * GameObject where this component is located also has to contain SurfaceDetector and RigidBody components.
    ///  <para/>
    /// * Somewhere in this hierarchy has to be located two or more MoveenStep2 components which will be used to complete walker layout.
    /// </summary>
    [ExecuteInEditMode] //needed to detect delta change
    public class MoveenStepper5 : OrderedMonoBehaviour {
        public static bool showInstrumentalInfo;  

        [Tooltip("Body")]
        public Transform body;
        [NonSerialized] public Rigidbody bodyRigid;
        [Tooltip("Hip (not necessary)")] public Transform hip;
        [Tooltip("Main animation engine")] public Stepper5 engine = new Stepper5();

        [HideInInspector] public Vector3 rootLocalPos;
        [HideInInspector] public Quaternion bodyDeltaR;
        [HideInInspector] public Vector3 hipDeltaPos;
        [HideInInspector] public Quaternion hipDeltaRot;

        [NonSerialized] public bool needsReset = true; //we can't do any updates at start, because not all scripts are instantiated and connected yet
        [NonSerialized] public bool needsUpdate = true;
        [NonSerialized] public Vector3 targetPos; 
        [NonSerialized] public Quaternion targetRot = Quaternion.identity; 
        [NonSerialized] private int lastLegsCount = -1;
        [NonSerialized]private bool updateWasCalled;

        public MoveenStepper5() {
            participateInTick = true;
            participateInFixedTick = true;
        }

        public void checkNeeds() {
            if (needsUpdate) updateData();
            if (needsReset) reset();
        }

        public override void OnEnable() {
            base.OnEnable();
            MUtil.logEvent(this, "OnEnable");
            needsUpdate = true;
            if (body != null) bodyRigid = body.GetComponent<Rigidbody>();
            else bodyRigid = GetComponent<Rigidbody>();
        }

        public void updateData() {
            MUtil.logEvent(this, "updateData");
            needsUpdate = false;
            //TODO update height
            //TODO only update, not create

            engine.steps.Clear();
            engine.legSkel.Clear();

            MoveenStep2[] step2s = transform.gameObject.GetComponentsInChildren<MoveenStep2>();
            for (int index = 0; index < step2s.Length; index++) {
                var step2 = step2s[index];
                if (!step2.isActiveAndEnabled) continue;
                step2.step.thisTransform = step2.transform;

// //            for (int i = 0; i < transform.childCount; i++) {
// //                Transform child = transform.GetChild(i);
// //                MoveenStep2 step2 = child.GetComponent<MoveenStep2>();
//                 if (!Application.isPlaying) {
// //we can't do this at runtime, because basises are changed by the flexible hip 
//                     step2.step.basisPosRel = step2.transform.localPosition;
//                     step2.step.basisRotRel = step2.transform.localRotation;
//                 }

                MoveenSkelBase skelDesc = step2.transform.GetComponent<MoveenSkelBase>();
                if (!skelDesc.isActiveAndEnabled) skelDesc = null;
                engine.legSkel.Add(skelDesc);
                if (skelDesc == null) {
                    continue;
                }

                step2.step.maxLen = skelDesc.maxLen;
                step2.step.surfaceDetector = engine.surfaceDetector;
                if (!step2.step.detachedComfortPosRel && !Application.isPlaying) {
                    Vector3 tipAbs = skelDesc.transform.TransformPoint(skelDesc.targetPosRel);
                    step2.step.comfortPosRel = transform.rotation.conjug().rotate(tipAbs - transform.position);//can't use Inverse, because in the enine - only rotation and translation is used
                }
                engine.steps.Add(step2.step);
            }
            if (engine.steps.Count < 4) {
                AssetsGo2SSkeletonsV.fillNeuroSimple(engine.steps, engine.rewardSelf, engine.rewardOthers);
            } else {
                AssetsGo2SSkeletonsV.fillNeuroNPares2(engine.steps, engine.rewardSelf, engine.rewardPare, engine.affectCounter, engine.rewardOthers, engine.affectOthers);
            }
        }

        public void reset() {
            MUtil.logEvent(this, "reset");
            needsReset = false;
            engine.reset(transform.position, transform.rotation);
            targetPos = transform.position;
            targetRot = transform.rotation;
        }

        public override void tick(float dt) {
            if (!Application.isPlaying) {
                if (transform.hasChanged) {
                    needsUpdate = true;
                    needsReset = true;
                }
                if (lastLegsCount != calcActualLegsCount()) {
                    lastLegsCount = calcActualLegsCount();
                    needsUpdate = true;
                    needsReset = true;
                }
            }
            checkNeeds();
            if (Application.isPlaying) {
                if (body != null) {//update for gizmos
                    //body.position -= engine.realBodyPos - transform.position;
                    transform.position = engine.realBodyPos;
                    transform.rotation = engine.realBodyRot;
                }
            } else {
                updateDeltas();
            }
            updateWasCalled = true;
        }

        public override void fixedTick(float dt) {
            if (bodyRigid == null) return;
            if (!updateWasCalled) return;//because it updates and resets
            if (engine.steps.Count < 1) return;

            if (body != null) {
                engine.realBodyPos = body.rotation.rotate(rootLocalPos) + body.position;
                engine.realBodyRot = body.rotation * bodyDeltaR;
            } else {
                engine.realBodyPos = transform.position;
                engine.realBodyRot = transform.rotation;
            }
            checkNeeds();            

            engine.realBodyAngularSpeed = bodyRigid.angularVelocity;
            engine.realSpeed = bodyRigid.velocity;
            engine.setWantedPos(dt, targetPos, targetRot);
            engine.doTickHip = hip != null;
            engine.tick(dt);

            bodyRigid.AddTorque(engine.resultRotAcceleration, ForceMode.Acceleration);//acceleration, so that mass doesn't change animation 
            bodyRigid.AddForce(engine.resultAcceleration, ForceMode.Acceleration);

            if (body != null) {
                transform.position = engine.realBodyPos; //because we want this update not only in Update/tick, but also in FixedUpdate/fixedTick 
                transform.rotation = engine.realBodyRot;
            }

            if (engine.protectBodyFromFallthrough) {
                Vector3 projected = engine.project(bodyRigid.transform.position);
                float dif = projected.y - bodyRigid.transform.position.y;
                if (!(dif > engine.protectBodyFromFallthroughMaxHeight) && bodyRigid.transform.position.y < projected.y) {
                    bodyRigid.transform.position = bodyRigid.transform.position.withSetY(projected.y);
                    bodyRigid.velocity = Vector3.zero;
                }
            }

            if (hip != null) {
                if (Application.isPlaying) {
                    hip.SetPositionAndRotation(engine.hipPosAbs + engine.hipRotAbs.rotate(hipDeltaPos), engine.hipRotAbs * hipDeltaRot);
                }
            }
        }

        private void updateDeltas() {
            Quaternion skelR = transform.rotation;
            if (hip != null) {
                Quaternion targetR = hip.rotation;

                hipDeltaRot = skelR.conjug() * targetR;
                hipDeltaPos = skelR.conjug().rotate(hip.position - (transform.position + skelR.rotate(engine.hipPosRel)));
            }
            if (body != null) {
                Quaternion geomR = body.rotation;
                rootLocalPos = geomR.conjug().rotate(transform.position - body.position);
                bodyDeltaR = geomR.conjug().mul(skelR);
            }
        }

        public int calcActualLegsCount() {
            return transform.GetComponentsInChildren<MoveenStep2>().Length;
        }

        //This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
        public override void OnValidate() {
            MUtil.logEvent(this, "OnValidate");
            needsUpdate = true;
            if (!Application.isPlaying) {
                needsReset = true;
            }
        }

    }
}





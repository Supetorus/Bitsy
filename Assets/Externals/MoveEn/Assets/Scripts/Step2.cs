namespace moveen.core {
    using System;
    using System.Collections.Generic;
    using moveen.descs;
    using moveen.utils;
    using UnityEngine;
    using UnityEngine.Serialization;

    [Serializable] public class Step2 {

        [NonSerialized] public Transform thisTransform;
        static int nextId;
        [NonSerialized][InstrumentalInfo] public int internalId = (Step2.nextId)++;
        [Tooltip("Put the foot closer on steep slopes")] public bool handleSteepSlopes = true;
        [Tooltip("Limit max step height (may lead to slippage)")] public bool limitMaxStepHeight;
        [Tooltip("Absolute max step height")] public float maxStepHeight = 0.5f;
        [Tooltip("Off: comfortPosRel will be same as the leg tip.\nOn: comfortPosRel can be defined separately from the leg tip.")] public bool detachedComfortPosRel;
        [Tooltip("Best (comfort) leg position")][FormerlySerializedAs("bestTargetRel")] public Vector3 comfortPosRel = new Vector3(0, -1, 0);
        [HideInInspector] public ISurfaceDetector surfaceDetector;
        [HideInInspector] public List<StepNeuro<Step2>> affectedByProgress = MUtil2.al<StepNeuro<Step2>>();
        [HideInInspector] public List<StepNeuro<Step2>> affectedByDeviation = MUtil2.al<StepNeuro<Step2>>();
        [HideInInspector] public List<StepNeuro<Step2>> affectedByDir = MUtil2.al<StepNeuro<Step2>>();
        [Header("Step dynamics (consider increasing of speeds if stepping is odd)")][Tooltip("Minimal leg stepping speed")] public float stepSpeedMin = 4;
        [Tooltip("Leg stepping speed X body speed dependency")] public float stepSpeedBodySpeedMul = 1.2f;
        [Tooltip("Leg stepping speed X body rotation speed dependency")] public float stepSpeedBodyRotSpeedMul = 4;
        public float maxAcceleration = 5;
        [Tooltip("Pause between leg decided to step, and it actually ups (0 for very light walk, 0.2 and more to add more weight)")] public float undockPause;
        [HideInInspector] public float airTime;
        [HideInInspector][InstrumentalInfo] public bool wasTooLong;
        [HideInInspector][InstrumentalInfo] public bool dockedState;
        [HideInInspector] public Vector3 bodyPos;
        [HideInInspector] public Quaternion bodyRot;
        [HideInInspector] public Quaternion projectedRot;
        [HideInInspector] public Vector3 bodySpeed;
        [HideInInspector] public float bodySpeedMax;
        [HideInInspector] public Vector3 additionalDisplacement;
        [HideInInspector] public Vector3 terrainNormal = new Vector3(0, 1, 0);
        [HideInInspector] public float pursueBodySpeed = 1;
        [NonSerialized] public Vector3 bodyForward = new Vector3(1, 0, 0);
        [NonSerialized] public Vector3 bodyUp = new Vector3(0, 1, 0);
        [Header("Step geometry")][Tooltip("Show possible step trajectory")] public bool showTrajectory;
        [Tooltip("Step radius multiplier. Some leg layouts may require modification of default value")] public float comfortRadiusRatio = 1;
        [HideInInspector][InstrumentalInfo] public float comfortRadius;
        [Tooltip("Speed, with which foot is orienting in body's direction while in air")] public int footOrientationSpeed = 5;
        Quaternion possibleFootOrientation;
        [HideInInspector] public Quaternion footOrientation = Quaternion.identity;
        [FloatWarning(min=0)][Tooltip("Parameter for step trajectory, turn on 'showTrajectory' and look at changes")] public float targetDockR = 0.3f;
        [HideInInspector][InstrumentalInfo] public Vector3 targetTo;
        [FloatWarning(min=0)][Tooltip("Parameter for step trajectory, turn on 'showTrajectory' and look at changes")] public float undockInitialStrength = 3;
        [FloatWarning(min=0)][Tooltip("Parameter for step trajectory, turn on 'showTrajectory' and look at changes")] public float undockTime = 0.3f;
        [HideInInspector] public float undockHDif;
        public bool emergencyDown;
        [NonSerialized] public bool forbidHalf;
        [NonSerialized] public Vector3 forbidHalfPos;
        [NonSerialized] public Vector3 forbidHalfDir;
        [NonSerialized] public bool isExample;
        [HideInInspector][InstrumentalInfo] public bool switchToBottom;
        [HideInInspector][InstrumentalInfo] public bool wasUnderground;
        [HideInInspector][InstrumentalInfo] public Vector3 curTarget;
        [HideInInspector][InstrumentalInfo] public float speedSlow;
        [HideInInspector][InstrumentalInfo] public Vector3 calcTarget;
        [HideInInspector][InstrumentalInfo] public float legAlt;
        [HideInInspector][InstrumentalInfo] public float lastStepLen;
        [HideInInspector][InstrumentalInfo] public float lastDockedLen;
        [HideInInspector][InstrumentalInfo] public float lastAirLen;
        [HideInInspector][InstrumentalInfo] public float lastAirTime;
        [HideInInspector][InstrumentalInfo] public float lastLandTime;
        [HideInInspector][InstrumentalInfo] public Vector3 lastDockedAtLocal;
        [HideInInspector][InstrumentalInfo] public Vector3 lastUndockedAtLocal;
        [HideInInspector][InstrumentalInfo] public Vector3 basisAbs;
        [HideInInspector][InstrumentalInfo] public Vector3 acceleration;
        [HideInInspector][InstrumentalInfo] public float comfortFromSkel;
        [HideInInspector][InstrumentalInfo] public Vector3 bestTargetConservativeAbs = new Vector3(0, 0, 0);
        [HideInInspector][InstrumentalInfo] public Vector3 bestTargetConservativeUnprojAbs = new Vector3(0, 0, 0);
        [HideInInspector][InstrumentalInfo] public Vector3 bestTargetConservativeUnprojAbs2 = new Vector3(0, 0, 0);
        [HideInInspector][InstrumentalInfo] public Vector3 bestTargetProgressiveAbs = new Vector3(0, 0, 0);
        [HideInInspector][InstrumentalInfo] public Vector3 posAbs;
        [HideInInspector][InstrumentalInfo] public Vector3 wantedSpeed;
        [HideInInspector][InstrumentalInfo] public Vector3 speedAbs;
        [HideInInspector][InstrumentalInfo] public float undockPauseCur;
        [HideInInspector][InstrumentalInfo] public float undockUpLength;
        [HideInInspector][InstrumentalInfo] public Vector3 undockVec;
        [HideInInspector][InstrumentalInfo] public float undockProgress;
        [HideInInspector][InstrumentalInfo] public float undockCurTime;
        [HideInInspector][InstrumentalInfo] public float maxLen;
        [HideInInspector][InstrumentalInfo] public float fromAbove;
        [HideInInspector][InstrumentalInfo] public float landTime;
        [HideInInspector][InstrumentalInfo] public float deviation;
        [HideInInspector][InstrumentalInfo] public float progress;
        [HideInInspector][InstrumentalInfo] public float timedProgress;
        [HideInInspector][InstrumentalInfo] public float beFaster;
        [HideInInspector][InstrumentalInfo] public float legSpeed = 4;
        [HideInInspector][InstrumentalInfo] public Vector3 g = new Vector3(0, -9.81f, 0);
        [NonSerialized] public bool collectSteppingHistory;
        public CounterStacksCollection paramHistory = new CounterStacksCollection((10 * 50));

        public virtual void calcAbs(float dt, Vector3 futurePos, Quaternion futureRot) {
            this.bodyUp = this.bodyRot.rotate(Vector3.up);
            this.bodyForward = this.bodyRot.rotate(Vector3.right);
            Vector3 chosenTargetRel = ExtensionMethods.add(this.comfortPosRel, this.additionalDisplacement);
            this.bestTargetConservativeUnprojAbs2 = Step2.toAbs(chosenTargetRel, this.bodyPos, this.projectedRot);
            this.bestTargetConservativeUnprojAbs = ExtensionMethods.withSetY(this.bestTargetConservativeUnprojAbs2, this.bodyPos.y);
            this.bestTargetConservativeAbs = this.surfaceDetector.detect(this.bestTargetConservativeUnprojAbs, Vector3.up).limit(this.bestTargetConservativeUnprojAbs, (this.comfortRadius * 4));
            futurePos = ExtensionMethods.mix(futurePos, ExtensionMethods.add(futurePos, this.bodySpeed), 0.5f);
            if (this.emergencyDown)  {
                this.bestTargetProgressiveAbs = this.bestTargetConservativeAbs;
            } else  {
                 {
                    this.bestTargetProgressiveAbs = ExtensionMethods.withSetY(Step2.toAbs(chosenTargetRel, futurePos, this.projectedRot), futurePos.y);
                    this.bestTargetProgressiveAbs = this.surfaceDetector.detect(this.bestTargetProgressiveAbs, Vector3.up);
                }
            }
            if (this.handleSteepSlopes)  {
                Vector3 downAbsPos = this.surfaceDetector.detect((this.basisAbs + (futurePos - this.bodyPos)), this.bodyUp);
                float fall = (this.bestTargetProgressiveAbs.y - downAbsPos.y);
                float mapped = MyMath.regionMapClamp(Math.Abs(fall), 0, (this.maxLen * 0.5f), 0, 1);
                this.bestTargetProgressiveAbs = ExtensionMethods.mix(this.bestTargetProgressiveAbs, downAbsPos, mapped);
            }
            this.bestTargetProgressiveAbs = ExtensionMethods.limit(this.bestTargetProgressiveAbs, this.bestTargetConservativeAbs, this.comfortRadius);
            if (this.forbidHalf)  {
                float x = ExtensionMethods.scalarProduct(ExtensionMethods.sub(this.bestTargetProgressiveAbs, this.forbidHalfPos), this.forbidHalfDir);
                if ((x < 0))  {
                    this.bestTargetProgressiveAbs = (this.bestTargetProgressiveAbs - ((x * 1.1f) * this.forbidHalfDir));
                }
            }
            if (this.limitMaxStepHeight)  {
                float cur = ExtensionMethods.sub(this.bestTargetProgressiveAbs, this.basisAbs).y;
                if ((cur > this.maxStepHeight))  {
                    this.bestTargetProgressiveAbs = ExtensionMethods.withSetY(this.bestTargetProgressiveAbs, -100500);
                }
            }
            this.comfortRadius = ((this.maxLen * 0.5f) * this.comfortRadiusRatio);
            this.deviationUnclamped = (
                MyMath.min(
                ExtensionMethods.dist(this.bestTargetConservativeAbs, this.posAbs), 
                ExtensionMethods.dist(this.bestTargetProgressiveAbs, this.posAbs)) / 
                this.comfortRadius);
            this.deviation = MyMath.clamp(this.deviationUnclamped, 0, 10);
        }

        [HideInInspector] public Vector3 oldPosAbs;
        [HideInInspector][InstrumentalInfo] public float airParam;
        [HideInInspector][InstrumentalInfo] public bool canGoAir;
        [HideInInspector][InstrumentalInfo] public Vector3 airTarget;
        float deviationUnclamped;
        [NonSerialized] P<Vector3> tmp = new P<Vector3>(new Vector3());
        public float cycleTime;
        public float curCycle;
        [NonSerialized] public bool prepare;

/*GENERATED*/        [Optimize]
/*GENERATED*/        public virtual void tick(float dt) {
/*GENERATED*/            this.curCycle = (this.curCycle + dt);
/*GENERATED*/            Vector3 i32_a = this.posAbs;
/*GENERATED*/            Vector3 i33_b = this.oldPosAbs;
/*GENERATED*/            this.speedAbs.x = ((i32_a.x + -i33_b.x) / dt);
/*GENERATED*/            this.speedAbs.y = ((i32_a.y + -i33_b.y) / dt);
/*GENERATED*/            this.speedAbs.z = ((i32_a.z + -i33_b.z) / dt);
/*GENERATED*/            this.oldPosAbs = this.posAbs;
/*GENERATED*/            bool _357 = (this.comfortFromSkel > 0.99f);
/*GENERATED*/            if (_357)  {
/*GENERATED*/                bool _365 = (this.dockedState && (this.landTime > 0.2f));
/*GENERATED*/                if (_365)  {
/*GENERATED*/                    bool _366 = this.collectSteppingHistory;
/*GENERATED*/                    if (_366)  {
/*GENERATED*/                        this.paramHistory.setValue(HistoryInfoBean.beginStep, 1);
/*GENERATED*/                    }
/*GENERATED*/                    bool _367 = this.dockedState;
/*GENERATED*/                    if (_367)  {
/*GENERATED*/                        this.undockPauseCur = (this.undockPause * 0);
/*GENERATED*/                        float i87_progress = this.deviation;
/*GENERATED*/                        this.undockPauseCur = ((this.undockPauseCur * (1 + -i87_progress)) + (this.undockPause * i87_progress));
/*GENERATED*/                    }
/*GENERATED*/                    bool _368 = (1 > 0);
/*GENERATED*/                    if (_368)  {
/*GENERATED*/                        this.switchToBottom = false;
/*GENERATED*/                    }
/*GENERATED*/                    this.dockedState = false;
/*GENERATED*/                    this.undockProgress = 0;
/*GENERATED*/                    this.undockCurTime = (this.undockProgress * this.undockTime);
/*GENERATED*/                    this.undockUpLength = (this.undockInitialStrength * this.undockHDif * (1 + -this.undockProgress));
/*GENERATED*/                    this.targetTo = this.bestTargetProgressiveAbs;
/*GENERATED*/                    this.emergencyDown = false;
/*GENERATED*/                    Vector3 i74_a = this.posAbs;
/*GENERATED*/                    Vector3 i75_b = this.bestTargetProgressiveAbs;
/*GENERATED*/                    this.lastUndockedAtLocal.x = (i74_a.x + -i75_b.x);
/*GENERATED*/                    this.lastUndockedAtLocal.y = (i74_a.y + -i75_b.y);
/*GENERATED*/                    this.lastUndockedAtLocal.z = (i74_a.z + -i75_b.z);
/*GENERATED*/                    Vector3 i78_a = this.lastDockedAtLocal;
/*GENERATED*/                    Vector3 i79_b = this.lastUndockedAtLocal;
/*GENERATED*/                    float i82_diff_x = (i78_a.x + -i79_b.x);
/*GENERATED*/                    float i83_diff_y = (i78_a.y + -i79_b.y);
/*GENERATED*/                    float i84_diff_z = (i78_a.z + -i79_b.z);
/*GENERATED*/                    this.lastStepLen = (float)(Math.Sqrt(((i82_diff_x * i82_diff_x) + (i83_diff_y * i83_diff_y) + (i84_diff_z * i84_diff_z))));
/*GENERATED*/                    this.lastDockedLen = this.lastStepLen;
/*GENERATED*/                    this.lastLandTime = this.landTime;
/*GENERATED*/                }
/*GENERATED*/                this.wasTooLong = true;
/*GENERATED*/            } else  {
/*GENERATED*/                 {
/*GENERATED*/                    this.wasTooLong = false;
/*GENERATED*/                }
/*GENERATED*/            }
/*GENERATED*/            this.wantedSpeed.x = 0;
/*GENERATED*/            this.wantedSpeed.y = 0;
/*GENERATED*/            this.wantedSpeed.z = 0;
/*GENERATED*/            this.airParam = 0;
/*GENERATED*/            bool _22 = !this.dockedState;
/*GENERATED*/            if (_22)  {
/*GENERATED*/                this.undockPauseCur = (this.undockPauseCur + dt);
/*GENERATED*/                this.landTime = 0;
/*GENERATED*/                bool _26 = (this.undockPauseCur >= this.undockPause);
/*GENERATED*/                if (_26)  {
/*GENERATED*/                    this.airTime = (this.airTime + dt);
/*GENERATED*/                    this.targetTo = this.bestTargetProgressiveAbs;
/*GENERATED*/                    this.undockCurTime = (this.undockCurTime + dt);
/*GENERATED*/                    this.undockProgress = (this.undockCurTime / this.undockTime);
/*GENERATED*/                    bool _370 = (this.undockProgress >= 1);
/*GENERATED*/                    if (_370)  {
/*GENERATED*/                        this.undockProgress = 1;
/*GENERATED*/                        this.undockUpLength = 0;
/*GENERATED*/                        this.undockVec.x = 0;
/*GENERATED*/                        this.undockVec.y = 0;
/*GENERATED*/                        this.undockVec.z = 0;
/*GENERATED*/                    } else  {
/*GENERATED*/                         {
/*GENERATED*/                            this.undockUpLength = ((this.undockInitialStrength / this.legSpeed) * this.undockHDif * (1 + -this.undockProgress));
/*GENERATED*/                            this.undockVec.x = 0;
/*GENERATED*/                            this.undockVec.y = this.undockUpLength;
/*GENERATED*/                            this.undockVec.z = 0;
/*GENERATED*/                        }
/*GENERATED*/                    }
/*GENERATED*/                    Vector3 i456_a = this.undockVec;
/*GENERATED*/                    Vector3 i460_a = this.basisAbs;
/*GENERATED*/                    Vector3 i461_b = this.posAbs;
/*GENERATED*/                    Vector3 i458_a = new Vector3((i460_a.x + -i461_b.x), (i460_a.y + -i461_b.y), (i460_a.z + -i461_b.z));
/*GENERATED*/                    float i459_newLen = ((this.undockInitialStrength / 3) * (1 + -this.undockProgress));
/*GENERATED*/                    Vector3 i462_a = Vector3.Normalize(i458_a);
/*GENERATED*/                    Vector3 i457_b = new Vector3((i462_a.x * i459_newLen), (i462_a.y * i459_newLen), (i462_a.z * i459_newLen));
/*GENERATED*/                    Vector3 newVar_489 = new Vector3((i456_a.x + i457_b.x), (i456_a.y + i457_b.y), (i456_a.z + i457_b.z));
/*GENERATED*/                    this.undockVec = newVar_489;
/*GENERATED*/                    Vector3 i101_from = this.posAbs;
/*GENERATED*/                    Vector3 i110_b = this.targetTo;
/*GENERATED*/                    float newVar_259 = (i101_from.x + -i110_b.x);
/*GENERATED*/                    float newVar_261 = (i101_from.z + -i110_b.z);
/*GENERATED*/                    Vector3 i114_a = this.targetTo;
/*GENERATED*/                    this.curTarget.x = i114_a.x;
/*GENERATED*/                    this.curTarget.y = (i114_a.y + this.targetDockR);
/*GENERATED*/                    this.curTarget.z = i114_a.z;
/*GENERATED*/                    bool _371 = (
/*GENERATED*/                        (
/*GENERATED*/                        ((float)(Math.Sqrt(((newVar_259 * newVar_259) + (newVar_261 * newVar_261)))) > this.targetDockR) && 
/*GENERATED*/                        !this.switchToBottom) || 
/*GENERATED*/                        (this.undockProgress < 0.3f));
/*GENERATED*/                    if (_371)  {
/*GENERATED*/                    } else  {
/*GENERATED*/                         {
/*GENERATED*/                            this.switchToBottom = true;
/*GENERATED*/                            Vector3 i169_a = this.targetTo;
/*GENERATED*/                            this.curTarget.x = i169_a.x;
/*GENERATED*/                            this.curTarget.y = (i169_a.y + -this.targetDockR);
/*GENERATED*/                            this.curTarget.z = i169_a.z;
/*GENERATED*/                        }
/*GENERATED*/                    }
/*GENERATED*/                    Vector3 i118_THIS = this.curTarget;
/*GENERATED*/                    Vector3 i119_center = this.basisAbs;
/*GENERATED*/                    float i120_max = (this.maxLen * 1.5f);
/*GENERATED*/                    Vector3 i123_THIS = new Vector3((i118_THIS.x + -i119_center.x), (i118_THIS.y + -i119_center.y), (i118_THIS.z + -i119_center.z));
/*GENERATED*/                    Vector3 i121_a;
/*GENERATED*/                    bool _372 = (
/*GENERATED*/                        (float)(Math.Sqrt(((i123_THIS.x * i123_THIS.x) + (i123_THIS.y * i123_THIS.y) + (i123_THIS.z * i123_THIS.z)))) > 
/*GENERATED*/                        i120_max);
/*GENERATED*/                    if (_372)  {
/*GENERATED*/                        Vector3 i466_a = Vector3.Normalize(i123_THIS);
/*GENERATED*/                        Vector3 newVar_495 = new Vector3((i466_a.x * i120_max), (i466_a.y * i120_max), (i466_a.z * i120_max));
/*GENERATED*/                        i121_a = newVar_495;
/*GENERATED*/                    } else  {
/*GENERATED*/                        i121_a = i123_THIS;
/*GENERATED*/                    }
/*GENERATED*/                    this.curTarget.x = (i121_a.x + i119_center.x);
/*GENERATED*/                    this.curTarget.y = (i121_a.y + i119_center.y);
/*GENERATED*/                    this.curTarget.z = (i121_a.z + i119_center.z);
/*GENERATED*/                    this.airParam = ((this.canGoAir) ? (MyMath.clamp(((this.bestTargetConservativeUnprojAbs2.y + -this.curTarget.y) / this.maxLen), 0, 1)) : (0));
/*GENERATED*/                    Vector3 i130_a = this.curTarget;
/*GENERATED*/                    Vector3 i131_b = this.bestTargetConservativeUnprojAbs2;
/*GENERATED*/                    float i132_progress = this.airParam;
/*GENERATED*/                    this.airTarget.x = (((i131_b.x + -i130_a.x) * i132_progress) + i130_a.x);
/*GENERATED*/                    this.airTarget.y = (((i131_b.y + -i130_a.y) * i132_progress) + i130_a.y);
/*GENERATED*/                    this.airTarget.z = (((i131_b.z + -i130_a.z) * i132_progress) + i130_a.z);
/*GENERATED*/                    Vector3 i139_a = this.airTarget;
/*GENERATED*/                    Vector3 i140_b = this.posAbs;
/*GENERATED*/                    Vector3 i103_pos2target = new Vector3((i139_a.x + -i140_b.x), (i139_a.y + -i140_b.y), (i139_a.z + -i140_b.z));
/*GENERATED*/                    float i104_posTargetLen = (float)(Math.Sqrt((
/*GENERATED*/                        (i103_pos2target.x * i103_pos2target.x) + 
/*GENERATED*/                        (i103_pos2target.y * i103_pos2target.y) + 
/*GENERATED*/                        (i103_pos2target.z * i103_pos2target.z))));
/*GENERATED*/                    Vector3 i105_dir;
/*GENERATED*/                    bool _373 = (i104_posTargetLen > 0.01f);
/*GENERATED*/                    if (_373)  {
/*GENERATED*/                        Vector3 newVar_499 = new Vector3(
/*GENERATED*/                            (i103_pos2target.x / i104_posTargetLen), 
/*GENERATED*/                            (i103_pos2target.y / i104_posTargetLen), 
/*GENERATED*/                            (i103_pos2target.z / i104_posTargetLen));
/*GENERATED*/                        i105_dir = newVar_499;
/*GENERATED*/                    } else  {
/*GENERATED*/                        i105_dir = i103_pos2target;
/*GENERATED*/                    }
/*GENERATED*/                    this.speedSlow = 1;
/*GENERATED*/                    bool _374 = (this.airParam > 0.1f);
/*GENERATED*/                    if (_374)  {
/*GENERATED*/                        this.speedSlow = 0.1f;
/*GENERATED*/                    }
/*GENERATED*/                    Vector3 i150_b = this.undockVec;
/*GENERATED*/                    Vector3 i147_a = new Vector3((i105_dir.x + i150_b.x), (i105_dir.y + i150_b.y), (i105_dir.z + i150_b.z));
/*GENERATED*/                    Vector3 i153_a = Vector3.Normalize(i147_a);
/*GENERATED*/                    float forPare_632 = (this.legSpeed * this.speedSlow);
/*GENERATED*/                    this.calcTarget.x = (i153_a.x * forPare_632);
/*GENERATED*/                    this.calcTarget.y = (i153_a.y * forPare_632);
/*GENERATED*/                    this.calcTarget.z = (i153_a.z * forPare_632);
/*GENERATED*/                    this.tmp.v = this.calcTarget;
/*GENERATED*/                    Vector3 i155_a = this.wantedSpeed;
/*GENERATED*/                    Vector3 i156_b = this.tmp.v;
/*GENERATED*/                    this.wantedSpeed.x = (i155_a.x + i156_b.x);
/*GENERATED*/                    this.wantedSpeed.y = (i155_a.y + i156_b.y);
/*GENERATED*/                    this.wantedSpeed.z = (i155_a.z + i156_b.z);
/*GENERATED*/                    bool _27 = (this.wantedSpeed.y < 0);
/*GENERATED*/                    if (_27)  {
/*GENERATED*/                        this.undockCurTime = this.undockTime;
/*GENERATED*/                    }
/*GENERATED*/                    Vector3 i157_THIS = this.wantedSpeed;
/*GENERATED*/                    float i158_max = this.legSpeed;
/*GENERATED*/                    Vector3 newVar_15;
/*GENERATED*/                    bool _375 = (
/*GENERATED*/                        (float)(Math.Sqrt(((i157_THIS.x * i157_THIS.x) + (i157_THIS.y * i157_THIS.y) + (i157_THIS.z * i157_THIS.z)))) > 
/*GENERATED*/                        i158_max);
/*GENERATED*/                    if (_375)  {
/*GENERATED*/                        Vector3 i472_a = Vector3.Normalize(i157_THIS);
/*GENERATED*/                        Vector3 newVar_503 = new Vector3((i472_a.x * i158_max), (i472_a.y * i158_max), (i472_a.z * i158_max));
/*GENERATED*/                        newVar_15 = newVar_503;
/*GENERATED*/                    } else  {
/*GENERATED*/                        newVar_15 = i157_THIS;
/*GENERATED*/                    }
/*GENERATED*/                    this.wantedSpeed = newVar_15;
/*GENERATED*/                    Vector3 i162_a = this.bodySpeed;
/*GENERATED*/                    float i163_d = this.pursueBodySpeed;
/*GENERATED*/                    Vector3 i164_a = this.wantedSpeed;
/*GENERATED*/                    this.wantedSpeed.x = (i164_a.x + (i162_a.x * i163_d));
/*GENERATED*/                    this.wantedSpeed.y = (i164_a.y + (i162_a.y * i163_d));
/*GENERATED*/                    this.wantedSpeed.z = (i164_a.z + (i162_a.z * i163_d));
/*GENERATED*/                } else  {
/*GENERATED*/                     {
/*GENERATED*/                    }
/*GENERATED*/                }
/*GENERATED*/                Vector3 i88_a = this.wantedSpeed;
/*GENERATED*/                Vector3 i89_b = this.speedAbs;
/*GENERATED*/                Vector3 newVar_12 = new Vector3((i88_a.x + -i89_b.x), (i88_a.y + -i89_b.y), (i88_a.z + -i89_b.z));
/*GENERATED*/                float i95_max = this.maxAcceleration;
/*GENERATED*/                Vector3 newVar_11;
/*GENERATED*/                bool _369 = (
/*GENERATED*/                    (float)(Math.Sqrt(((newVar_12.x * newVar_12.x) + (newVar_12.y * newVar_12.y) + (newVar_12.z * newVar_12.z)))) > 
/*GENERATED*/                    i95_max);
/*GENERATED*/                if (_369)  {
/*GENERATED*/                    Vector3 i476_a = Vector3.Normalize(newVar_12);
/*GENERATED*/                    Vector3 newVar_507 = new Vector3((i476_a.x * i95_max), (i476_a.y * i95_max), (i476_a.z * i95_max));
/*GENERATED*/                    newVar_11 = newVar_507;
/*GENERATED*/                } else  {
/*GENERATED*/                    newVar_11 = newVar_12;
/*GENERATED*/                }
/*GENERATED*/                this.acceleration = newVar_11;
/*GENERATED*/            } else  {
/*GENERATED*/                 {
/*GENERATED*/                    this.airTime = 0;
/*GENERATED*/                    this.undockPauseCur = 0;
/*GENERATED*/                    this.landTime = (this.landTime + dt);
/*GENERATED*/                    this.wantedSpeed = this.g;
/*GENERATED*/                    Vector3 i173_a = this.wantedSpeed;
/*GENERATED*/                    Vector3 i174_b = this.speedAbs;
/*GENERATED*/                    this.acceleration.x = (i173_a.x + -i174_b.x);
/*GENERATED*/                    this.acceleration.y = (i173_a.y + -i174_b.y);
/*GENERATED*/                    this.acceleration.z = (i173_a.z + -i174_b.z);
/*GENERATED*/                }
/*GENERATED*/            }
/*GENERATED*/            Vector3 i40_a = this.speedAbs;
/*GENERATED*/            Vector3 i41_b = this.acceleration;
/*GENERATED*/            this.speedAbs.x = (i40_a.x + i41_b.x);
/*GENERATED*/            this.speedAbs.y = (i40_a.y + i41_b.y);
/*GENERATED*/            this.speedAbs.z = (i40_a.z + i41_b.z);
/*GENERATED*/            Vector3 i42_a = this.speedAbs;
/*GENERATED*/            Vector3 i44_a = this.posAbs;
/*GENERATED*/            this.posAbs.x = (i44_a.x + (i42_a.x * dt));
/*GENERATED*/            this.posAbs.y = (i44_a.y + (i42_a.y * dt));
/*GENERATED*/            this.posAbs.z = (i44_a.z + (i42_a.z * dt));
/*GENERATED*/            bool _23 = this.isExample;
/*GENERATED*/            if (_23)  {
/*GENERATED*/                return ;
/*GENERATED*/            }
/*GENERATED*/            Vector3 i46_a = this.posAbs;
/*GENERATED*/            Vector3 newVar_8 = new Vector3(i46_a.x, (i46_a.y + this.maxLen), i46_a.z);
/*GENERATED*/            Vector3 curPosProjected = this.surfaceDetector.detect(newVar_8, Vector3.up);
/*GENERATED*/            Vector3 i50_a = this.posAbs;
/*GENERATED*/            Vector3 i52_a = this.terrainNormal;
/*GENERATED*/            this.legAlt = (
/*GENERATED*/                (i52_a.x * (i50_a.x + -curPosProjected.x)) + 
/*GENERATED*/                (i52_a.y * (i50_a.y + -curPosProjected.y)) + 
/*GENERATED*/                (i52_a.z * (i50_a.z + -curPosProjected.z)));
/*GENERATED*/            this.wasUnderground = false;
/*GENERATED*/            bool _24 = (this.legAlt < 0.01f);
/*GENERATED*/            if (_24)  {
/*GENERATED*/                this.wasUnderground = true;
/*GENERATED*/                this.posAbs = curPosProjected;
/*GENERATED*/            }
/*GENERATED*/            bool _25 = ((this.legAlt < 0.01f) && this.switchToBottom);
/*GENERATED*/            if (_25)  {
/*GENERATED*/                bool _28 = !this.dockedState;
/*GENERATED*/                if (_28)  {
/*GENERATED*/                    Vector3 i175_a = this.posAbs;
/*GENERATED*/                    Vector3 i176_b = this.bestTargetConservativeAbs;
/*GENERATED*/                    this.lastDockedAtLocal.x = (i175_a.x + -i176_b.x);
/*GENERATED*/                    this.lastDockedAtLocal.y = (i175_a.y + -i176_b.y);
/*GENERATED*/                    this.lastDockedAtLocal.z = (i175_a.z + -i176_b.z);
/*GENERATED*/                    Vector3 i177_a = this.lastDockedAtLocal;
/*GENERATED*/                    Vector3 i178_b = this.lastUndockedAtLocal;
/*GENERATED*/                    float i181_diff_x = (i177_a.x + -i178_b.x);
/*GENERATED*/                    float i182_diff_y = (i177_a.y + -i178_b.y);
/*GENERATED*/                    float i183_diff_z = (i177_a.z + -i178_b.z);
/*GENERATED*/                    this.lastStepLen = (float)(Math.Sqrt(((i181_diff_x * i181_diff_x) + (i182_diff_y * i182_diff_y) + (i183_diff_z * i183_diff_z))));
/*GENERATED*/                    this.lastAirLen = this.lastStepLen;
/*GENERATED*/                    this.lastAirTime = this.airTime;
/*GENERATED*/                    this.cycleTime = ((this.cycleTime * 0.9f) + (this.curCycle * 0.1f));
/*GENERATED*/                    this.curCycle = 0;
/*GENERATED*/                }
/*GENERATED*/                this.dockedState = true;
/*GENERATED*/            }
/*GENERATED*/            float i54_wholeCycle = (this.lastAirTime + this.lastLandTime);
/*GENERATED*/            bool _358 = this.dockedState;
/*GENERATED*/            if (_358)  {
/*GENERATED*/                this.timedProgress = (this.landTime / i54_wholeCycle);
/*GENERATED*/            } else  {
/*GENERATED*/                 {
/*GENERATED*/                    this.timedProgress = ((this.airTime + this.lastLandTime) / i54_wholeCycle);
/*GENERATED*/                }
/*GENERATED*/            }
/*GENERATED*/            bool _359 = float.IsNaN(this.progress);
/*GENERATED*/            if (_359)  {
/*GENERATED*/                this.progress = 0;
/*GENERATED*/            }
/*GENERATED*/            float i55_value = this.progress;
/*GENERATED*/            float i59_arg1;
/*GENERATED*/            bool _360 = (i55_value < 1);
/*GENERATED*/            if (_360)  {
/*GENERATED*/                i59_arg1 = i55_value;
/*GENERATED*/            } else  {
/*GENERATED*/                i59_arg1 = 1;
/*GENERATED*/            }
/*GENERATED*/            this.progress = (((0 > i59_arg1)) ? (0) : (i59_arg1));
/*GENERATED*/            bool _361 = this.collectSteppingHistory;
/*GENERATED*/            if (_361)  {
/*GENERATED*/                this.paramHistory.setValue(HistoryInfoBean.timedProgress, this.timedProgress);
/*GENERATED*/            }
/*GENERATED*/            float i62_value = (this.curCycle / i54_wholeCycle);
/*GENERATED*/            float i66_arg1;
/*GENERATED*/            bool _362 = (i62_value < 1);
/*GENERATED*/            if (_362)  {
/*GENERATED*/                i66_arg1 = i62_value;
/*GENERATED*/            } else  {
/*GENERATED*/                i66_arg1 = 1;
/*GENERATED*/            }
/*GENERATED*/            this.timedProgress = (((0 > i66_arg1)) ? (0) : (i66_arg1));
/*GENERATED*/            bool _363 = (!this.dockedState && (this.undockPauseCur > this.undockPause));
/*GENERATED*/            if (_363)  {
/*GENERATED*/                this.surfaceDetector.detect(this.posAbs, Vector3.up);
/*GENERATED*/                Vector3 newVar_354 = new Vector3(0, 1, 0);
/*GENERATED*/                this.possibleFootOrientation = Quaternion.FromToRotation(newVar_354, this.surfaceDetector.normal).mul(this.projectedRot);
/*GENERATED*/                float i190_value = (dt * this.footOrientationSpeed);
/*GENERATED*/                float i194_arg1;
/*GENERATED*/                bool _376 = (i190_value < 1);
/*GENERATED*/                if (_376)  {
/*GENERATED*/                    i194_arg1 = i190_value;
/*GENERATED*/                } else  {
/*GENERATED*/                    i194_arg1 = 1;
/*GENERATED*/                }
/*GENERATED*/                float i189_blend;
/*GENERATED*/                bool _377 = (0 > i194_arg1);
/*GENERATED*/                if (_377)  {
/*GENERATED*/                    i189_blend = 0;
/*GENERATED*/                } else  {
/*GENERATED*/                    i189_blend = i194_arg1;
/*GENERATED*/                }
/*GENERATED*/                this.footOrientation = Quaternion.Lerp(this.footOrientation, this.possibleFootOrientation, i189_blend);
/*GENERATED*/            }
/*GENERATED*/            bool _364 = this.collectSteppingHistory;
/*GENERATED*/            if (_364)  {
/*GENERATED*/                this.paramHistory.setValue(HistoryInfoBean.deviation, this.deviation);
/*GENERATED*/                float p;
/*GENERATED*/                bool _378 = (this.progress > 0.5f);
/*GENERATED*/                if (_378)  {
/*GENERATED*/                    p = (1 + -this.progress);
/*GENERATED*/                } else  {
/*GENERATED*/                    p = this.progress;
/*GENERATED*/                }
/*GENERATED*/                p = (p * 2);
/*GENERATED*/                this.paramHistory.setValue(HistoryInfoBean.progress, ((this.dockedState) ? (p) : (-p)));
/*GENERATED*/                this.paramHistory.setValue(HistoryInfoBean.land, ((this.dockedState) ? (1) : (0)));
/*GENERATED*/                this.paramHistory.setValue(HistoryInfoBean.switchToBottom, ((this.switchToBottom) ? (1) : (0)));
/*GENERATED*/                this.paramHistory.setValue(HistoryInfoBean.legAlt, (this.legAlt * 3));
/*GENERATED*/                this.paramHistory.setValue(HistoryInfoBean.wasTooLong, ((this.wasTooLong) ? (1) : (0)));
/*GENERATED*/                this.paramHistory.setValue(HistoryInfoBean.undockProgress, this.undockProgress);
/*GENERATED*/            }
/*GENERATED*/        }
        void history() {
            if (this.collectSteppingHistory)  {
                this.paramHistory.setValue(HistoryInfoBean.deviation, this.deviation);
                float p = (((this.progress > 0.5f)) ? ((1 - this.progress)) : (this.progress));
                p = (p * 2);
                this.paramHistory.setValue(HistoryInfoBean.progress, ((this.dockedState) ? (p) : (-p)));
                this.paramHistory.setValue(HistoryInfoBean.land, ((this.dockedState) ? (1) : (0)));
                this.paramHistory.setValue(HistoryInfoBean.switchToBottom, ((this.switchToBottom) ? (1) : (0)));
                this.paramHistory.setValue(HistoryInfoBean.legAlt, (this.legAlt * 3));
                this.paramHistory.setValue(HistoryInfoBean.wasTooLong, ((this.wasTooLong) ? (1) : (0)));
                this.paramHistory.setValue(HistoryInfoBean.undockProgress, this.undockProgress);
            }
        }
        void tickProgress() {
            float wholeCycle = (this.lastAirTime + this.lastLandTime);
            if (this.dockedState)  {
                this.timedProgress = (this.landTime / wholeCycle);
            } else  {
                 {
                    this.timedProgress = ((this.airTime + this.lastLandTime) / wholeCycle);
                }
            }
            if (float.IsNaN(this.progress))  {
                this.progress = 0;
            }
            this.progress = MyMath.clamp(this.progress, 0, 1);
            if (this.collectSteppingHistory)  {
                this.paramHistory.setValue(HistoryInfoBean.timedProgress, this.timedProgress);
            }
            this.timedProgress = MyMath.clamp((this.curCycle / wholeCycle), 0, 1);
        }
/*GENERATED*/        [Optimize]
/*GENERATED*/        public virtual void tickFoot(float dt) {
/*GENERATED*/            bool _638 = (!this.dockedState && (this.undockPauseCur > this.undockPause));
/*GENERATED*/            if (_638)  {
/*GENERATED*/                this.surfaceDetector.detect(this.posAbs, Vector3.up);
/*GENERATED*/                Vector3 newVar_634 = new Vector3(0, 1, 0);
/*GENERATED*/                this.possibleFootOrientation = Quaternion.FromToRotation(newVar_634, this.surfaceDetector.normal).mul(this.projectedRot);
/*GENERATED*/                float newVar_637 = (dt * this.footOrientationSpeed);
/*GENERATED*/                float i645_arg1;
/*GENERATED*/                bool _652 = (newVar_637 < 1);
/*GENERATED*/                if (_652)  {
/*GENERATED*/                    i645_arg1 = newVar_637;
/*GENERATED*/                } else  {
/*GENERATED*/                    i645_arg1 = 1;
/*GENERATED*/                }
/*GENERATED*/                float newVar_636;
/*GENERATED*/                bool _653 = (0 > i645_arg1);
/*GENERATED*/                if (_653)  {
/*GENERATED*/                    newVar_636 = 0;
/*GENERATED*/                } else  {
/*GENERATED*/                    newVar_636 = i645_arg1;
/*GENERATED*/                }
/*GENERATED*/                Quaternion newVar_635 = Quaternion.Lerp(this.footOrientation, this.possibleFootOrientation, newVar_636);
/*GENERATED*/                this.footOrientation = newVar_635;
/*GENERATED*/            }
/*GENERATED*/        }
        public virtual void checkTooLong() {
            if ((this.comfortFromSkel > 0.99f))  {
                if ((this.dockedState && (this.landTime > 0.2f)))  {
                    this.beginStep(1);
                }
                this.wasTooLong = true;
            } else  {
                 {
                    this.wasTooLong = false;
                }
            }
        }
        public virtual void beginStep(float undockStrength) {
            if (this.collectSteppingHistory)  {
                this.paramHistory.setValue(HistoryInfoBean.beginStep, 1);
            }
            if (this.dockedState)  {
                this.undockPauseCur = (this.undockPause * (1 - undockStrength));
                this.undockPauseCur = MyMath.mix(this.undockPauseCur, this.undockPause, this.deviation);
            }
            if ((undockStrength > 0))  {
                this.switchToBottom = false;
            }
            this.dockedState = false;
            this.undockProgress = (1 - undockStrength);
            this.undockCurTime = (this.undockProgress * this.undockTime);
            this.undockUpLength = MyMath.mix((this.undockInitialStrength * this.undockHDif), 0, this.undockProgress);
            this.targetTo = this.bestTargetProgressiveAbs;
            this.emergencyDown = false;
            this.lastUndockedAtLocal = ExtensionMethods.sub(this.posAbs, this.bestTargetProgressiveAbs);
            this.lastStepLen = ExtensionMethods.dist(this.lastDockedAtLocal, this.lastUndockedAtLocal);
            this.lastDockedLen = this.lastStepLen;
            this.lastLandTime = this.landTime;
        }
        public virtual void reset(Vector3 pos, Quaternion rot) {
            MUtil.logEvent(this, ("reset " + pos));
            this.comfortRadius = ((this.maxLen * 0.5f) * this.comfortRadiusRatio);
            this.bodyPos = pos;
            this.bodyRot = rot;
            this.bestTargetConservativeUnprojAbs = (Step2.toAbs(this.comfortPosRel, this.bodyPos, rot) + this.additionalDisplacement);
            this.bestTargetConservativeAbs = this.bestTargetConservativeUnprojAbs;
            this.bestTargetProgressiveAbs = this.bestTargetConservativeUnprojAbs;
            this.posAbs = this.bestTargetConservativeUnprojAbs;
            this.oldPosAbs = this.bestTargetConservativeUnprojAbs;
            this.speedAbs = new Vector3();
            this.acceleration = new Vector3();
            this.undockPauseCur = 0;
            this.airTime = 0;
            this.undockProgress = 100500;
            this.undockCurTime = 100500;
            this.footOrientation = rot;
            this.lastDockedAtLocal = this.comfortPosRel;
            this.lastUndockedAtLocal = this.comfortPosRel;
        }
        public static Vector3 toAbs(Vector3 abs, Vector3 pos, Quaternion rot) {
            return ExtensionMethods.add(ExtensionMethods.rotate(rot, abs), pos);
        }
        public virtual void targetFill(P<Vector3> pvec) {
            Vector3 from = this.posAbs;
            float fromToDist = ExtensionMethods.length(ExtensionMethods.withSetY(ExtensionMethods.sub(from, this.targetTo), 0));
            this.curTarget = ExtensionMethods.add(this.targetTo, 0, this.targetDockR, 0);
            if ((((fromToDist > this.targetDockR) && !this.switchToBottom) || (this.undockProgress < 0.3f)))  {
            } else  {
                 {
                    this.switchToBottom = true;
                    this.curTarget = ExtensionMethods.sub(this.targetTo, 0, this.targetDockR, 0);
                }
            }
            this.curTarget = ExtensionMethods.limit(this.curTarget, this.basisAbs, (this.maxLen * 1.5f));
            this.airParam = ((this.canGoAir) ? (MyMath.clamp(((this.bestTargetConservativeUnprojAbs2.y - this.curTarget.y) / this.maxLen), 0, 1)) : (0));
            this.airTarget = ExtensionMethods.mix(this.curTarget, this.bestTargetConservativeUnprojAbs2, this.airParam);
            Vector3 pos2target = ExtensionMethods.sub(this.airTarget, this.posAbs);
            float posTargetLen = ExtensionMethods.length(pos2target);
            Vector3 dir = (((posTargetLen > 0.01f)) ? ((pos2target / posTargetLen)) : (pos2target));
            this.speedSlow = 1;
            if ((this.airParam > 0.1f))  {
                this.speedSlow = 0.1f;
            }
            this.calcTarget = (ExtensionMethods.normalized(ExtensionMethods.add(dir, this.undockVec), this.legSpeed) * this.speedSlow);
            pvec.v = this.calcTarget;
        }
        public virtual void undockTick(float dt) {
            this.undockCurTime += dt;
            this.undockProgress = (this.undockCurTime / this.undockTime);
            if ((this.undockProgress >= 1))  {
                this.undockProgress = 1;
                this.undockUpLength = 0;
                this.undockVec = new Vector3();
            } else  {
                 {
                    this.undockUpLength = MyMath.mix(((this.undockInitialStrength / this.legSpeed) * this.undockHDif), 0, this.undockProgress);
                    this.undockVec = new Vector3(0, this.undockUpLength, 0);
                }
            }
            this.undockVec += ExtensionMethods.normalized((this.basisAbs - this.posAbs), ((this.undockInitialStrength / 3) * (1 - this.undockProgress)));
        }

    }
}

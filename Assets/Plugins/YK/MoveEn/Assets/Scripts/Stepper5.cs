namespace moveen.core {
    using System;
    using System.Collections.Generic;
    using moveen.descs;
    using moveen.utils;
    using UnityEngine;

    [Serializable] public class Stepper5 {

        [Tooltip("Logical right leg to calculate gait (even for multipeds)")] public int leadingLegRight;
        [Tooltip("Logical left leg to calculate gait (even for multipeds)")] public int leadingLegLeft = 1;
        [Range(0,1)][Tooltip("Step phase. 0.5 - normal step. 0.1 - left right pause. 0.9 - right left pause")] public float phase = 0.5f;
        [Range(0,1)][Tooltip("The leg will try to get more support from behind")] public float lackOfSpeedCompensation = 0.1f;
        public float rewardSelf = 2;
        public float rewardOthers = 2;
        public float affectOthers = 0;
        public float rewardPare = 5;
        public float affectCounter = 20;
        [HideInInspector] public float runJumpTime;
        public bool forceBipedalEarlyStep;
        [Tooltip("Reduce foot entanglement for bipedals")] public bool bipedalForbidPlacement;
        [Tooltip("Protects the body from fall through. Must be enabled if no colliders is used")] public bool protectBodyFromFallthrough = true;
        [Tooltip("Ceiling height which will not be seen as a floor, through which it fell. Don't make it too small, as it is critical on steep slopes")] public float protectBodyFromFallthroughMaxHeight = 1;
        [Header("Body movement")][Range(0.5f,1.5f)][Tooltip("0.5 - lower body between lands, 1 - no lowering, 1.5 - higher between lands (unnatural)")] public float downOnStep = 0.7f;
        public MotorBean horizontalMotor;
        public MotorBean verticalMotor;
        public MotorBean rotationMotor;
        [Header("Center Of Gravity simulation (important for certain gait)")][Tooltip("Center Of Gravity")] public float cogUpDown;
        [Range(-0.5f,0.5f)][Tooltip("Rotate around Center Of Gravity")] public float cogAngle = 0.2f;
        public float cogRotationMultiplier = 1;
        [Tooltip("Push acceleration to compensate Center Of Gravity")] public float cogAccel = 10;
        [NonSerialized] public Vector3 calculatedCOG;
        [NonSerialized] public Vector3 calculatedCOGSpeed;
        [Header("Body helps or opposes legs position")][Range(-1,1)][Tooltip("Rotation for the body to help steps length or oppose (-1 - clumsy, +1 - agile)")] public float bodyLenHelp;
        [Tooltip("Body helps the length in movement only")] public bool bodyLenHelpAtSpeedOnly = true;
        [Tooltip("Speed at which maximum rotation is achieved")] public float bodyLenHelpMaxSpeed = 1;
        [Header("Hip")][Range(0,0.5f)][Tooltip("Hip flexibility relative to the body")] public float hipFlexibility;
        [HideInInspector] public Quaternion wantedHipRot;
        Quaternion slowLocalHipRot;
        [Tooltip("Hip position relative to the body (center of its rotation)")] public Vector3 hipPosRel = new Vector3(0, -0.5f, 0);
        [HideInInspector] public Vector3 hipPosAbs;
        [HideInInspector] public Quaternion hipRotAbs = Quaternion.identity;
        [NonSerialized] public bool doTickHip;
        [Header("_system")] public bool collectSteppingHistory;
        public bool showPhaseDials;
        [HideInInspector] public Quaternion projectedRot;
        [HideInInspector] public Vector3 realSpeed;
        [HideInInspector] public Vector3 g = new Vector3(0, -9.81f, 0);
        [HideInInspector] public Vector3 realBodyAngularSpeed;
        [HideInInspector][InstrumentalInfo] public Vector3 resultAcceleration;
        [HideInInspector][InstrumentalInfo] public Vector3 resultRotAcceleration;
        [HideInInspector] public Vector3 realBodyPos;
        [HideInInspector] public Quaternion realBodyRot = Quaternion.identity;
        [HideInInspector] public Vector3 projPos;
        [HideInInspector][InstrumentalInfo] public Vector3 inputWantedPos;
        [HideInInspector] public Quaternion inputWantedRot;
        [HideInInspector][InstrumentalInfo] public Vector3 inputAnimPos;
        [HideInInspector] public Quaternion inputAnimRot;
        [NonSerialized] public ISurfaceDetector surfaceDetector = new SurfaceDetectorStub();
        [NonSerialized] public List<MoveenSkelBase> legSkel = MUtil2.al<MoveenSkelBase>();
        [NonSerialized] public List<Step2> steps = MUtil2.al<Step2>();
        [NonSerialized] public Vector3 up = new Vector3(0, 1, 0);
        [HideInInspector][InstrumentalInfo] public Vector3 imCenter;
        [HideInInspector][InstrumentalInfo] public Vector3 imCenterSpeed;
        [HideInInspector][InstrumentalInfo] public Vector3 imCenterAngularSpeed;
        [HideInInspector][InstrumentalInfo] public Vector3 imBody;
        [HideInInspector][InstrumentalInfo] public Vector3 imBodySpeed;
        [HideInInspector][InstrumentalInfo] public Vector3 imActualCenterSpeed;
        [HideInInspector][InstrumentalInfo] public Vector3 speedLack;
        [HideInInspector][InstrumentalInfo] public Vector3 virtualForLegs;
        [HideInInspector][InstrumentalInfo] public float midLen;

        public Stepper5() {
            MUtil.logEvent(this, "constructor");
        }
        public virtual void setWantedPos(float dt, Vector3 wantedPos, Quaternion wantedRot) {
            this.inputWantedPos = wantedPos;
            this.inputWantedRot = wantedRot;
            this.projPos = this.project(this.realBodyPos);
            this.projectedRot = MUtil.qToAxes(ExtensionMethods.getXForVerticalAxis(ExtensionMethods.rotate(wantedRot, new Vector3(1, 0, 0)), this.up), this.up);
        }
        public virtual void tick(float dt) {
            for (int i = 0; (i < this.steps.Count); (i)++)  {
                if ((this.steps[i].thisTransform == null))  {
                    this.legSkel.RemoveAt(i);
                    this.steps.RemoveAt(i);
                    (i)--;
                }
            }
            for (int i = 0; (i < this.steps.Count); (i)++)  {
                Step2 step = this.steps[i];
                step.collectSteppingHistory = this.collectSteppingHistory;
                if (this.collectSteppingHistory)  {
                    step.paramHistory.next();
                }
            }
            this.tickHip(dt);
            this.calcAbs(dt);
            Step2 right = (((this.leadingLegRight < this.steps.Count)) ? (this.steps[this.leadingLegRight]) : (null));
            Step2 left = (((this.leadingLegLeft < this.steps.Count)) ? (this.steps[this.leadingLegLeft]) : (null));
            if (((right != null) && (left != null)))  {
                float p0 = right.timedProgress;
                float p1 = left.timedProgress;
                float fr = MyMath.fract((p1 - p0));
                if ((fr > this.phase))  {
                    right.beFaster = 0.5f;
                    left.beFaster = 0;
                } else  {
                    if ((fr < this.phase))  {
                        right.beFaster = 0;
                        left.beFaster = 0.5f;
                    }
                }
                right.legSpeed *= (1 + right.beFaster);
                left.legSpeed *= (1 + left.beFaster);
            }
            this.tickSteps(dt);
        }
/*GENERATED*/        [Optimize]
/*GENERATED*/        void tickHip(float dt) {
/*GENERATED*/            Step2 right;
/*GENERATED*/            bool _734 = (this.leadingLegRight < this.steps.Count);
/*GENERATED*/            if (_734)  {
/*GENERATED*/                right = this.steps[this.leadingLegRight];
/*GENERATED*/            } else  {
/*GENERATED*/                right = null;
/*GENERATED*/            }
/*GENERATED*/            Step2 left;
/*GENERATED*/            bool _735 = (this.leadingLegLeft < this.steps.Count);
/*GENERATED*/            if (_735)  {
/*GENERATED*/                left = this.steps[this.leadingLegLeft];
/*GENERATED*/            } else  {
/*GENERATED*/                left = null;
/*GENERATED*/            }
/*GENERATED*/            bool _736 = ((left != null) && (right != null));
/*GENERATED*/            if (_736)  {
/*GENERATED*/                this.midLen = ((left.maxLen + right.maxLen) / 2);
/*GENERATED*/            }
/*GENERATED*/            this.wantedHipRot = this.projectedRot;
/*GENERATED*/            Quaternion wantedRot = this.inputWantedRot;
/*GENERATED*/            int dockedCount = 0;
/*GENERATED*/            for (int i = 0; (i < this.steps.Count); i = (i + 1))  {
/*GENERATED*/                bool _745 = this.steps[i].dockedState;
/*GENERATED*/                if (_745)  {
/*GENERATED*/                    dockedCount = (dockedCount + 1);
/*GENERATED*/                }
/*GENERATED*/            }
/*GENERATED*/            for (int i = 0; (i < this.steps.Count); i = (i + 1))  {
/*GENERATED*/                this.steps[i].canGoAir = (dockedCount == 0);
/*GENERATED*/            }
/*GENERATED*/            float additionalSpeed_x = 0;
/*GENERATED*/            float additionalSpeed_y = 0;
/*GENERATED*/            float additionalSpeed_z = 0;
/*GENERATED*/            float i1500_y = this.cogUpDown;
/*GENERATED*/            Quaternion i763_THIS = this.realBodyRot;
/*GENERATED*/            float i767_x = (i763_THIS.x * 2);
/*GENERATED*/            float i769_z = (i763_THIS.z * 2);
/*GENERATED*/            Vector3 localCOG = new Vector3();
/*GENERATED*/            localCOG.x = (((i763_THIS.x * (i763_THIS.y * 2)) + -(i763_THIS.w * i769_z)) * i1500_y);
/*GENERATED*/            localCOG.y = ((1 + -((i763_THIS.x * i767_x) + (i763_THIS.z * i769_z))) * i1500_y);
/*GENERATED*/            localCOG.z = (((i763_THIS.y * i769_z) + (i763_THIS.w * i767_x)) * i1500_y);
/*GENERATED*/            Vector3 oldCalculatedCOG = this.calculatedCOG;
/*GENERATED*/            Vector3 i781_b = this.realBodyPos;
/*GENERATED*/            this.calculatedCOG.x = (localCOG.x + i781_b.x);
/*GENERATED*/            this.calculatedCOG.y = (localCOG.y + i781_b.y);
/*GENERATED*/            this.calculatedCOG.z = (localCOG.z + i781_b.z);
/*GENERATED*/            Vector3 i782_a = this.calculatedCOG;
/*GENERATED*/            Vector3 i790_a = this.calculatedCOGSpeed;
/*GENERATED*/            this.calculatedCOGSpeed.x = (((((i782_a.x + -oldCalculatedCOG.x) / dt) + -i790_a.x) * 0.1f) + i790_a.x);
/*GENERATED*/            this.calculatedCOGSpeed.y = (((((i782_a.y + -oldCalculatedCOG.y) / dt) + -i790_a.y) * 0.1f) + i790_a.y);
/*GENERATED*/            this.calculatedCOGSpeed.z = (((((i782_a.z + -oldCalculatedCOG.z) / dt) + -i790_a.z) * 0.1f) + i790_a.z);
/*GENERATED*/            for (int i = 0; (i < this.steps.Count); i = (i + 1))  {
/*GENERATED*/                Step2 step = this.steps[i];
/*GENERATED*/                Vector3 i955_a = step.comfortPosRel;
/*GENERATED*/                Vector3 i956_b = this.up;
/*GENERATED*/                Vector3 rollAxis = new Vector3(
/*GENERATED*/                    ((i955_a.y * i956_b.z) + -(i955_a.z * i956_b.y)), 
/*GENERATED*/                    ((i955_a.z * i956_b.x) + -(i955_a.x * i956_b.z)), 
/*GENERATED*/                    ((i955_a.x * i956_b.y) + -(i955_a.y * i956_b.x)));
/*GENERATED*/                bool _746 = (!step.dockedState && !step.wasTooLong);
/*GENERATED*/                if (_746)  {
/*GENERATED*/                    Quaternion rollQuaternion = Quaternion.AngleAxis((float)(((this.cogAngle / Math.PI) * 180)), rollAxis);
/*GENERATED*/                    Vector3 disp = rotDisp(rollQuaternion, -localCOG).withSetY(0);
/*GENERATED*/                    Quaternion i961_THIS = this.realBodyRot;
/*GENERATED*/                    float i965_x = (i961_THIS.x * 2);
/*GENERATED*/                    float i966_y = (i961_THIS.y * 2);
/*GENERATED*/                    float i967_z = (i961_THIS.z * 2);
/*GENERATED*/                    float i968_xx = (i961_THIS.x * i965_x);
/*GENERATED*/                    float i969_yy = (i961_THIS.y * i966_y);
/*GENERATED*/                    float i970_zz = (i961_THIS.z * i967_z);
/*GENERATED*/                    float i971_xy = (i961_THIS.x * i966_y);
/*GENERATED*/                    float i972_xz = (i961_THIS.x * i967_z);
/*GENERATED*/                    float i973_yz = (i961_THIS.y * i967_z);
/*GENERATED*/                    float i974_wx = (i961_THIS.w * i965_x);
/*GENERATED*/                    float i975_wy = (i961_THIS.w * i966_y);
/*GENERATED*/                    float i976_wz = (i961_THIS.w * i967_z);
/*GENERATED*/                    float i979_d = this.cogAccel;
/*GENERATED*/                    additionalSpeed_x = (
/*GENERATED*/                        additionalSpeed_x + 
/*GENERATED*/                        (
/*GENERATED*/                        (
/*GENERATED*/                        ((1 + -(i969_yy + i970_zz)) * disp.x) + 
/*GENERATED*/                        ((i971_xy + -i976_wz) * disp.y) + 
/*GENERATED*/                        ((i972_xz + i975_wy) * disp.z)) * 
/*GENERATED*/                        i979_d));
/*GENERATED*/                    additionalSpeed_y = (
/*GENERATED*/                        additionalSpeed_y + 
/*GENERATED*/                        (
/*GENERATED*/                        (
/*GENERATED*/                        ((i971_xy + i976_wz) * disp.x) + 
/*GENERATED*/                        ((1 + -(i968_xx + i970_zz)) * disp.y) + 
/*GENERATED*/                        ((i973_yz + -i974_wx) * disp.z)) * 
/*GENERATED*/                        i979_d));
/*GENERATED*/                    additionalSpeed_z = (
/*GENERATED*/                        additionalSpeed_z + 
/*GENERATED*/                        (
/*GENERATED*/                        (
/*GENERATED*/                        ((i972_xz + -i975_wy) * disp.x) + 
/*GENERATED*/                        ((i973_yz + i974_wx) * disp.y) + 
/*GENERATED*/                        ((1 + -(i968_xx + i969_yy)) * disp.z)) * 
/*GENERATED*/                        i979_d));
/*GENERATED*/                    bool _747 = (this.cogRotationMultiplier != 1);
/*GENERATED*/                    if (_747)  {
/*GENERATED*/                        Quaternion newVar_706 = Quaternion.AngleAxis((float)((((this.cogAngle * this.cogRotationMultiplier) / Math.PI) * 180)), rollAxis);
/*GENERATED*/                        rollQuaternion = newVar_706;
/*GENERATED*/                    }
/*GENERATED*/                    float i982_lhs_x_1794 = wantedRot.x;
/*GENERATED*/                    float i982_lhs_y_1795 = wantedRot.y;
/*GENERATED*/                    float i982_lhs_z_1796 = wantedRot.z;
/*GENERATED*/                    float i982_lhs_w_1797 = wantedRot.w;
/*GENERATED*/                    float i983_rhs_x_1798 = rollQuaternion.x;
/*GENERATED*/                    float i983_rhs_y_1799 = rollQuaternion.y;
/*GENERATED*/                    float i983_rhs_z_1800 = rollQuaternion.z;
/*GENERATED*/                    float i983_rhs_w_1801 = rollQuaternion.w;
/*GENERATED*/                    wantedRot.x = (
/*GENERATED*/                        (i982_lhs_w_1797 * i983_rhs_x_1798) + 
/*GENERATED*/                        (i982_lhs_x_1794 * i983_rhs_w_1801) + 
/*GENERATED*/                        (i982_lhs_y_1795 * i983_rhs_z_1800) + 
/*GENERATED*/                        -(i982_lhs_z_1796 * i983_rhs_y_1799));
/*GENERATED*/                    wantedRot.y = (
/*GENERATED*/                        (i982_lhs_w_1797 * i983_rhs_y_1799) + 
/*GENERATED*/                        (i982_lhs_y_1795 * i983_rhs_w_1801) + 
/*GENERATED*/                        (i982_lhs_z_1796 * i983_rhs_x_1798) + 
/*GENERATED*/                        -(i982_lhs_x_1794 * i983_rhs_z_1800));
/*GENERATED*/                    wantedRot.z = (
/*GENERATED*/                        (i982_lhs_w_1797 * i983_rhs_z_1800) + 
/*GENERATED*/                        (i982_lhs_z_1796 * i983_rhs_w_1801) + 
/*GENERATED*/                        (i982_lhs_x_1794 * i983_rhs_y_1799) + 
/*GENERATED*/                        -(i982_lhs_y_1795 * i983_rhs_x_1798));
/*GENERATED*/                    wantedRot.w = (
/*GENERATED*/                        (i982_lhs_w_1797 * i983_rhs_w_1801) + 
/*GENERATED*/                        -(i982_lhs_x_1794 * i983_rhs_x_1798) + 
/*GENERATED*/                        -(i982_lhs_y_1795 * i983_rhs_y_1799) + 
/*GENERATED*/                        -(i982_lhs_z_1796 * i983_rhs_z_1800));
/*GENERATED*/                }
/*GENERATED*/            }
/*GENERATED*/            float baseY = 0;
/*GENERATED*/            for (int i = 0; (i < this.steps.Count); (i)++) baseY = (baseY + this.steps[i].bestTargetConservativeAbs.y);
/*GENERATED*/            float baseY_728 = (baseY / this.steps.Count);
/*GENERATED*/            float maxDeviation = 0;
/*GENERATED*/            float commonProgress = 0;
/*GENERATED*/            for (int i = 0; (i < this.steps.Count); (i)++)  {
/*GENERATED*/                Step2 step = this.steps[i];
/*GENERATED*/                float dd = (step.deviation / step.comfortRadius);
/*GENERATED*/                bool _748 = !step.dockedState;
/*GENERATED*/                if (_748)  {
/*GENERATED*/                    commonProgress = (commonProgress + step.progress);
/*GENERATED*/                } else  {
/*GENERATED*/                     {
/*GENERATED*/                        bool _749 = (dd > maxDeviation);
/*GENERATED*/                        if (_749)  {
/*GENERATED*/                            maxDeviation = dd;
/*GENERATED*/                        }
/*GENERATED*/                    }
/*GENERATED*/                }
/*GENERATED*/            }
/*GENERATED*/            float i803_dstTo = this.downOnStep;
/*GENERATED*/            float i804_res = ((((maxDeviation + (commonProgress * 0.5f)) / 1) * (i803_dstTo + -1)) + 1);
/*GENERATED*/            float newVar_660;
/*GENERATED*/            bool _1481 = (1 < i803_dstTo);
/*GENERATED*/            if (_1481)  {
/*GENERATED*/                float i1635_arg1;
/*GENERATED*/                bool _1846 = (i804_res < i803_dstTo);
/*GENERATED*/                if (_1846)  {
/*GENERATED*/                    i1635_arg1 = i804_res;
/*GENERATED*/                } else  {
/*GENERATED*/                    i1635_arg1 = i803_dstTo;
/*GENERATED*/                }
/*GENERATED*/                newVar_660 = (((1 > i1635_arg1)) ? (1) : (i1635_arg1));
/*GENERATED*/            } else  {
/*GENERATED*/                float i1642_arg1;
/*GENERATED*/                bool _1847 = (i804_res < 1);
/*GENERATED*/                if (_1847)  {
/*GENERATED*/                    i1642_arg1 = i804_res;
/*GENERATED*/                } else  {
/*GENERATED*/                    i1642_arg1 = 1;
/*GENERATED*/                }
/*GENERATED*/                newVar_660 = (((i803_dstTo > i1642_arg1)) ? (i803_dstTo) : (i1642_arg1));
/*GENERATED*/            }
/*GENERATED*/            float d_729 = newVar_660;
/*GENERATED*/            bool _737 = (dockedCount == this.steps.Count);
/*GENERATED*/            if (_737)  {
/*GENERATED*/                d_729 = 1;
/*GENERATED*/            }
/*GENERATED*/            float i811_to = this.inputWantedPos.y;
/*GENERATED*/            float i812_progress = d_729;
/*GENERATED*/            float futureHeightDif;
/*GENERATED*/            bool _738 = ((this.runJumpTime > 0) && (dockedCount == 1));
/*GENERATED*/            if (_738)  {
/*GENERATED*/                float newVar_1315 = (this.realSpeed.y * this.runJumpTime);
/*GENERATED*/                futureHeightDif = (newVar_1315 + -((9.81f * this.runJumpTime * this.runJumpTime) / 2));
/*GENERATED*/            } else  {
/*GENERATED*/                futureHeightDif = 0;
/*GENERATED*/            }
/*GENERATED*/            Vector3 i813_a = this.inputWantedPos;
/*GENERATED*/            Vector3 i815_a = this.realBodyPos;
/*GENERATED*/            Vector3 newVar_663 = new Vector3((i813_a.x + -i815_a.x), 0, (i813_a.z + -i815_a.z));
/*GENERATED*/            Vector3 newVar_666 = new Vector3(additionalSpeed_x, 0, additionalSpeed_z);
/*GENERATED*/            this.realWantedSpeed = newVar_663.mul(this.horizontalMotor.distanceToSpeed).limit(this.horizontalMotor.maxSpeed).add(newVar_666);
/*GENERATED*/            Vector3 i823_a = this.inputWantedPos;
/*GENERATED*/            Vector3 i825_a = this.imCenter;
/*GENERATED*/            Vector3 newVar_667 = new Vector3((i823_a.x + -i825_a.x), 0, (i823_a.z + -i825_a.z));
/*GENERATED*/            Vector3 imCenterWantedSpeed = newVar_667.mul(this.horizontalMotor.distanceToSpeed).limit(this.horizontalMotor.maxSpeed);
/*GENERATED*/            Vector3 i831_a = this.imCenterSpeed;
/*GENERATED*/            Vector3 newVar_670 = new Vector3((imCenterWantedSpeed.x + -i831_a.x), imCenterWantedSpeed.y, (imCenterWantedSpeed.z + -i831_a.z));
/*GENERATED*/            Vector3 imCenterAccel = newVar_670.mul(this.horizontalMotor.speedDifToAccel);
/*GENERATED*/            float mult_730 = dockedCount;
/*GENERATED*/            float newVar_672 = (this.realBodyPos.y + futureHeightDif);
/*GENERATED*/            float verticalAccel = this.verticalMotor.getAccel(
/*GENERATED*/                ((baseY_728 * (1 + -i812_progress)) + (i811_to * i812_progress)), 
/*GENERATED*/                newVar_672, 
/*GENERATED*/                this.realSpeed.y, 
/*GENERATED*/                9.81f, 
/*GENERATED*/                dockedCount);
/*GENERATED*/            Vector3 i837_a = this.realSpeed;
/*GENERATED*/            Vector3 i839_a = this.realWantedSpeed;
/*GENERATED*/            Vector3 newVar_673 = new Vector3((i839_a.x + -i837_a.x), i839_a.y, (i839_a.z + -i837_a.z));
/*GENERATED*/            Vector3 realAccel = newVar_673.mul(this.horizontalMotor.speedDifToAccel).limit((this.horizontalMotor.maxAccel * mult_730)).add(0, verticalAccel, 0);
/*GENERATED*/            Vector3 oldImCenterPos = this.imCenter;
/*GENERATED*/            Vector3 imCenterAccel_731 = imCenterAccel.limit((this.horizontalMotor.maxAccel * mult_730));
/*GENERATED*/            Vector3 i845_a = this.imCenterSpeed;
/*GENERATED*/            this.imCenterSpeed.x = (i845_a.x + (imCenterAccel_731.x * dt));
/*GENERATED*/            this.imCenterSpeed.y = (i845_a.y + (imCenterAccel_731.y * dt));
/*GENERATED*/            this.imCenterSpeed.z = (i845_a.z + (imCenterAccel_731.z * dt));
/*GENERATED*/            this.imCenterSpeed.y = this.realSpeed.y;
/*GENERATED*/            this.imCenter.y = this.realBodyPos.y;
/*GENERATED*/            Vector3 i847_a = this.imCenterSpeed;
/*GENERATED*/            Vector3 i849_a = this.imCenter;
/*GENERATED*/            this.imCenter.x = (i849_a.x + (i847_a.x * dt));
/*GENERATED*/            this.imCenter.y = (i849_a.y + (i847_a.y * dt));
/*GENERATED*/            this.imCenter.z = (i849_a.z + (i847_a.z * dt));
/*GENERATED*/            Vector3 i851_a = this.imCenter;
/*GENERATED*/            Vector3 i852_b = this.imBody;
/*GENERATED*/            this.imCenter.x = (((i852_b.x + -i851_a.x) * 0.05f) + i851_a.x);
/*GENERATED*/            this.imCenter.y = (((i852_b.y + -i851_a.y) * 0.05f) + i851_a.y);
/*GENERATED*/            this.imCenter.z = (((i852_b.z + -i851_a.z) * 0.05f) + i851_a.z);
/*GENERATED*/            Vector3 oldImBodyPos = this.imBody;
/*GENERATED*/            Vector3 i860_a = this.imBodySpeed;
/*GENERATED*/            Vector3 i862_a = this.realWantedSpeed;
/*GENERATED*/            Vector3 newVar_680 = new Vector3((i862_a.x + -i860_a.x), i862_a.y, (i862_a.z + -i860_a.z));
/*GENERATED*/            this.imBodySpeed = (
/*GENERATED*/                this.imBodySpeed + 
/*GENERATED*/                (
/*GENERATED*/                newVar_680.mul(this.horizontalMotor.speedDifToAccel).limit((this.horizontalMotor.maxAccel * mult_730)) * 
/*GENERATED*/                dt));
/*GENERATED*/            Vector3 i866_a = this.imBodySpeed;
/*GENERATED*/            Vector3 i868_a = this.imBody;
/*GENERATED*/            this.imBody.x = (i868_a.x + (i866_a.x * dt));
/*GENERATED*/            this.imBody.y = (i868_a.y + (i866_a.y * dt));
/*GENERATED*/            this.imBody.z = (i868_a.z + (i866_a.z * dt));
/*GENERATED*/            Vector3 i870_a = this.realBodyPos;
/*GENERATED*/            Vector3 i871_b = this.imBody;
/*GENERATED*/            float newVar_1165 = ((i870_a.x + -i871_b.x) * 0.1f);
/*GENERATED*/            float newVar_1166 = ((i870_a.y + -i871_b.y) * 0.1f);
/*GENERATED*/            float newVar_1167 = ((i870_a.z + -i871_b.z) * 0.1f);
/*GENERATED*/            Vector3 i876_a = this.imBody;
/*GENERATED*/            this.imBody.x = (i876_a.x + newVar_1165);
/*GENERATED*/            this.imBody.y = (i876_a.y + newVar_1166);
/*GENERATED*/            this.imBody.z = (i876_a.z + newVar_1167);
/*GENERATED*/            Vector3 i878_a = this.imCenter;
/*GENERATED*/            this.imCenter.x = (i878_a.x + newVar_1165);
/*GENERATED*/            this.imCenter.y = (i878_a.y + newVar_1166);
/*GENERATED*/            this.imCenter.z = (i878_a.z + newVar_1167);
/*GENERATED*/            Vector3 i880_a = this.imBody;
/*GENERATED*/            this.imBodySpeed.x = ((i880_a.x + -oldImBodyPos.x) / dt);
/*GENERATED*/            this.imBodySpeed.y = ((i880_a.y + -oldImBodyPos.y) / dt);
/*GENERATED*/            this.imBodySpeed.z = ((i880_a.z + -oldImBodyPos.z) / dt);
/*GENERATED*/            Vector3 i888_a = this.imCenter;
/*GENERATED*/            this.imActualCenterSpeed.x = ((i888_a.x + -oldImCenterPos.x) / dt);
/*GENERATED*/            this.imActualCenterSpeed.y = ((i888_a.y + -oldImCenterPos.y) / dt);
/*GENERATED*/            this.imActualCenterSpeed.z = ((i888_a.z + -oldImCenterPos.z) / dt);
/*GENERATED*/            Vector3 i896_a = this.imBody;
/*GENERATED*/            Vector3 i897_b = this.realBodyPos;
/*GENERATED*/            float i900_diff_x = (i896_a.x + -i897_b.x);
/*GENERATED*/            float i901_diff_y = (i896_a.y + -i897_b.y);
/*GENERATED*/            float i902_diff_z = (i896_a.z + -i897_b.z);
/*GENERATED*/            bool _739 = (
/*GENERATED*/                (float)(Math.Sqrt(((i900_diff_x * i900_diff_x) + (i901_diff_y * i901_diff_y) + (i902_diff_z * i902_diff_z)))) > 
/*GENERATED*/                5);
/*GENERATED*/            if (_739)  {
/*GENERATED*/                this.imCenter = this.realBodyPos;
/*GENERATED*/                this.imBody = this.realBodyPos;
/*GENERATED*/                this.imCenterSpeed = this.realSpeed;
/*GENERATED*/                this.imBodySpeed = this.realSpeed;
/*GENERATED*/            }
/*GENERATED*/            Vector3 i903_a = this.imCenter;
/*GENERATED*/            Vector3 i904_b = this.imBody;
/*GENERATED*/            float i907_diff_x = (i903_a.x + -i904_b.x);
/*GENERATED*/            float i908_diff_y = (i903_a.y + -i904_b.y);
/*GENERATED*/            float i909_diff_z = (i903_a.z + -i904_b.z);
/*GENERATED*/            bool _740 = (
/*GENERATED*/                (float)(Math.Sqrt(((i907_diff_x * i907_diff_x) + (i908_diff_y * i908_diff_y) + (i909_diff_z * i909_diff_z)))) > 
/*GENERATED*/                (this.cogAccel * 0.3f));
/*GENERATED*/            if (_740)  {
/*GENERATED*/                this.imCenter = this.imBody;
/*GENERATED*/                this.imCenterSpeed = this.imBodySpeed;
/*GENERATED*/            }
/*GENERATED*/            Vector3 i910_a = this.inputWantedPos;
/*GENERATED*/            Vector3 i911_b = this.imCenter;
/*GENERATED*/            Vector3 newVar_691 = new Vector3((i910_a.x + -i911_b.x), (i910_a.y + -i911_b.y), (i910_a.z + -i911_b.z));
/*GENERATED*/            Vector3 speedDif = (this.imActualCenterSpeed - (newVar_691 * this.horizontalMotor.distanceToSpeed));
/*GENERATED*/            Vector3 speedDif_732 = speedDif.limit(this.horizontalMotor.maxSpeed);
/*GENERATED*/            Vector3 i912_a = new Vector3();
/*GENERATED*/            i912_a.x = additionalSpeed_x;
/*GENERATED*/            i912_a.y = additionalSpeed_y;
/*GENERATED*/            i912_a.z = additionalSpeed_z;
/*GENERATED*/            Vector3 addNormalized = Vector3.Normalize(i912_a);
/*GENERATED*/            float i913_a_x_1752 = additionalSpeed_x;
/*GENERATED*/            float i913_a_y_1753 = additionalSpeed_y;
/*GENERATED*/            float i913_a_z_1754 = additionalSpeed_z;
/*GENERATED*/            float adLen = (float)(Math.Sqrt(((i913_a_x_1752 * i913_a_x_1752) + (i913_a_y_1753 * i913_a_y_1753) + (i913_a_z_1754 * i913_a_z_1754))));
/*GENERATED*/            float proj = (
/*GENERATED*/                (speedDif_732.x * addNormalized.x) + 
/*GENERATED*/                (speedDif_732.y * addNormalized.y) + 
/*GENERATED*/                (speedDif_732.z * addNormalized.z));
/*GENERATED*/            bool _741 = (proj > adLen);
/*GENERATED*/            if (_741)  {
/*GENERATED*/                proj = (proj + -adLen);
/*GENERATED*/            } else  {
/*GENERATED*/                bool _750 = (proj < -adLen);
/*GENERATED*/                if (_750)  {
/*GENERATED*/                    proj = (proj + adLen);
/*GENERATED*/                } else  {
/*GENERATED*/                    proj = 0;
/*GENERATED*/                }
/*GENERATED*/            }
/*GENERATED*/            float i924_b = (
/*GENERATED*/                (addNormalized.x * speedDif_732.x) + 
/*GENERATED*/                (addNormalized.y * speedDif_732.y) + 
/*GENERATED*/                (addNormalized.z * speedDif_732.z));
/*GENERATED*/            float i932_b = proj;
/*GENERATED*/            Vector3 newVar_692 = new Vector3(
/*GENERATED*/                (speedDif_732.x + -(addNormalized.x * i924_b) + (addNormalized.x * i932_b)), 
/*GENERATED*/                (speedDif_732.y + -(addNormalized.y * i924_b) + (addNormalized.y * i932_b)), 
/*GENERATED*/                (speedDif_732.z + -(addNormalized.z * i924_b) + (addNormalized.z * i932_b)));
/*GENERATED*/            float newVar_697 = (this.midLen * this.lackOfSpeedCompensation);
/*GENERATED*/            Vector3 newVar_696;
/*GENERATED*/            bool _1482 = (
/*GENERATED*/                (float)(Math.Sqrt(((newVar_692.x * newVar_692.x) + (newVar_692.y * newVar_692.y) + (newVar_692.z * newVar_692.z)))) > 
/*GENERATED*/                newVar_697);
/*GENERATED*/            if (_1482)  {
/*GENERATED*/                Vector3 i1647_a = Vector3.Normalize(newVar_692);
/*GENERATED*/                Vector3 newVar_1649 = new Vector3((i1647_a.x * newVar_697), (i1647_a.y * newVar_697), (i1647_a.z * newVar_697));
/*GENERATED*/                newVar_696 = newVar_1649;
/*GENERATED*/            } else  {
/*GENERATED*/                newVar_696 = newVar_692;
/*GENERATED*/            }
/*GENERATED*/            Vector3 i944_a = this.speedLack;
/*GENERATED*/            this.speedLack.x = (((newVar_696.x + -i944_a.x) * 0.1f) + i944_a.x);
/*GENERATED*/            this.speedLack.y = (((newVar_696.y + -i944_a.y) * 0.1f) + i944_a.y);
/*GENERATED*/            this.speedLack.z = (((newVar_696.z + -i944_a.z) * 0.1f) + i944_a.z);
/*GENERATED*/            this.speedLack.y = 0;
/*GENERATED*/            Vector3 i953_a = this.imCenter;
/*GENERATED*/            Vector3 i954_b = this.speedLack;
/*GENERATED*/            this.virtualForLegs.x = (i953_a.x + i954_b.x);
/*GENERATED*/            this.virtualForLegs.y = (i953_a.y + i954_b.y);
/*GENERATED*/            this.virtualForLegs.z = (i953_a.z + i954_b.z);
/*GENERATED*/            bool _742 = (realAccel.y > 0);
/*GENERATED*/            if (_742)  {
/*GENERATED*/                this.resultAcceleration = realAccel;
/*GENERATED*/            } else  {
/*GENERATED*/                this.resultAcceleration.x = 0;
/*GENERATED*/                this.resultAcceleration.y = 0;
/*GENERATED*/                this.resultAcceleration.z = 0;
/*GENERATED*/            }
/*GENERATED*/            bool _743 = (((this.bodyLenHelp != 0) && (left != null)) && (right != null));
/*GENERATED*/            if (_743)  {
/*GENERATED*/                Vector3 i986_a = right.posAbs;
/*GENERATED*/                Vector3 i987_b = left.posAbs;
/*GENERATED*/                Vector3 newVar_712 = new Vector3((i986_a.x + -i987_b.x), 0, (i986_a.z + -i987_b.z));
/*GENERATED*/                Vector3 s0to1Proj = Vector3.Normalize(newVar_712);
/*GENERATED*/                float i991_THIS_y_1813 = wantedRot.y;
/*GENERATED*/                float i991_THIS_z_1814 = wantedRot.z;
/*GENERATED*/                float i996_y = (i991_THIS_y_1813 * 2);
/*GENERATED*/                float i997_z = (i991_THIS_z_1814 * 2);
/*GENERATED*/                Vector3 newVar_714 = new Vector3(
/*GENERATED*/                    (1 + -((i991_THIS_y_1813 * i996_y) + (i991_THIS_z_1814 * i997_z))), 
/*GENERATED*/                    0, 
/*GENERATED*/                    ((wantedRot.x * i997_z) + -(wantedRot.w * i996_y)));
/*GENERATED*/                Vector3 curLook = Vector3.Normalize(newVar_714);
/*GENERATED*/                float speedDump;
/*GENERATED*/                bool _751 = this.bodyLenHelpAtSpeedOnly;
/*GENERATED*/                if (_751)  {
/*GENERATED*/                    Vector3 i1024_a = this.inputWantedPos;
/*GENERATED*/                    Vector3 i1025_b = this.realBodyPos;
/*GENERATED*/                    float newVar_1382 = (i1024_a.x + -i1025_b.x);
/*GENERATED*/                    float newVar_1383 = (i1024_a.y + -i1025_b.y);
/*GENERATED*/                    float newVar_1384 = (i1024_a.z + -i1025_b.z);
/*GENERATED*/                    float i1017_value = (
/*GENERATED*/                        (float)(Math.Sqrt(((newVar_1382 * newVar_1382) + (newVar_1383 * newVar_1383) + (newVar_1384 * newVar_1384)))) / 
/*GENERATED*/                        this.bodyLenHelpMaxSpeed);
/*GENERATED*/                    float i1028_arg1;
/*GENERATED*/                    bool _1483 = (i1017_value < 1);
/*GENERATED*/                    if (_1483)  {
/*GENERATED*/                        i1028_arg1 = i1017_value;
/*GENERATED*/                    } else  {
/*GENERATED*/                        i1028_arg1 = 1;
/*GENERATED*/                    }
/*GENERATED*/                    speedDump = (((0 > i1028_arg1)) ? (0) : (i1028_arg1));
/*GENERATED*/                } else  {
/*GENERATED*/                    speedDump = 1;
/*GENERATED*/                }
/*GENERATED*/                Quaternion newVar_718 = Quaternion.AngleAxis(
/*GENERATED*/                    (float)((
/*GENERATED*/                    (
/*GENERATED*/                    (
/*GENERATED*/                    ((curLook.x * s0to1Proj.x) + (curLook.y * s0to1Proj.y) + (curLook.z * s0to1Proj.z)) * 
/*GENERATED*/                    this.bodyLenHelp * 
/*GENERATED*/                    speedDump) / 
/*GENERATED*/                    Math.PI) * 
/*GENERATED*/                    180)), 
/*GENERATED*/                    this.up);
/*GENERATED*/                float i1015_lhs_x_1822 = wantedRot.x;
/*GENERATED*/                float i1015_lhs_y_1823 = wantedRot.y;
/*GENERATED*/                float i1015_lhs_z_1824 = wantedRot.z;
/*GENERATED*/                float i1015_lhs_w_1825 = wantedRot.w;
/*GENERATED*/                wantedRot.x = (
/*GENERATED*/                    (i1015_lhs_w_1825 * newVar_718.x) + 
/*GENERATED*/                    (i1015_lhs_x_1822 * newVar_718.w) + 
/*GENERATED*/                    (i1015_lhs_y_1823 * newVar_718.z) + 
/*GENERATED*/                    -(i1015_lhs_z_1824 * newVar_718.y));
/*GENERATED*/                wantedRot.y = (
/*GENERATED*/                    (i1015_lhs_w_1825 * newVar_718.y) + 
/*GENERATED*/                    (i1015_lhs_y_1823 * newVar_718.w) + 
/*GENERATED*/                    (i1015_lhs_z_1824 * newVar_718.x) + 
/*GENERATED*/                    -(i1015_lhs_x_1822 * newVar_718.z));
/*GENERATED*/                wantedRot.z = (
/*GENERATED*/                    (i1015_lhs_w_1825 * newVar_718.z) + 
/*GENERATED*/                    (i1015_lhs_z_1824 * newVar_718.w) + 
/*GENERATED*/                    (i1015_lhs_x_1822 * newVar_718.y) + 
/*GENERATED*/                    -(i1015_lhs_y_1823 * newVar_718.x));
/*GENERATED*/                wantedRot.w = (
/*GENERATED*/                    (i1015_lhs_w_1825 * newVar_718.w) + 
/*GENERATED*/                    -(i1015_lhs_x_1822 * newVar_718.x) + 
/*GENERATED*/                    -(i1015_lhs_y_1823 * newVar_718.y) + 
/*GENERATED*/                    -(i1015_lhs_z_1824 * newVar_718.z));
/*GENERATED*/            }
/*GENERATED*/            this.resultRotAcceleration = this.rotationMotor.getTorque(this.realBodyRot, wantedRot, this.realBodyAngularSpeed, ((float)(dockedCount) / this.steps.Count));
/*GENERATED*/            bool _744 = this.doTickHip;
/*GENERATED*/            if (_744)  {
/*GENERATED*/                bool _752 = ((left != null) && (right != null));
/*GENERATED*/                if (_752)  {
/*GENERATED*/                    this.calcHipLenHelp();
/*GENERATED*/                }
/*GENERATED*/                Quaternion i1031_THIS = this.wantedHipRot;
/*GENERATED*/                Quaternion i1032_from = this.realBodyRot;
/*GENERATED*/                float newVar_1392 = -i1032_from.x;
/*GENERATED*/                float newVar_1393 = -i1032_from.y;
/*GENERATED*/                float newVar_1394 = -i1032_from.z;
/*GENERATED*/                float i1498_w = i1032_from.w;
/*GENERATED*/                Quaternion newVar_723 = new Quaternion((
/*GENERATED*/                    (i1031_THIS.w * newVar_1392) + 
/*GENERATED*/                    (i1031_THIS.x * i1498_w) + 
/*GENERATED*/                    (i1031_THIS.y * newVar_1394) + 
/*GENERATED*/                    -(i1031_THIS.z * newVar_1393)), (
/*GENERATED*/                    (i1031_THIS.w * newVar_1393) + 
/*GENERATED*/                    (i1031_THIS.y * i1498_w) + 
/*GENERATED*/                    (i1031_THIS.z * newVar_1392) + 
/*GENERATED*/                    -(i1031_THIS.x * newVar_1394)), (
/*GENERATED*/                    (i1031_THIS.w * newVar_1394) + 
/*GENERATED*/                    (i1031_THIS.z * i1498_w) + 
/*GENERATED*/                    (i1031_THIS.x * newVar_1393) + 
/*GENERATED*/                    -(i1031_THIS.y * newVar_1392)), (
/*GENERATED*/                    (i1031_THIS.w * i1498_w) + 
/*GENERATED*/                    -(i1031_THIS.x * newVar_1392) + 
/*GENERATED*/                    -(i1031_THIS.y * newVar_1393) + 
/*GENERATED*/                    -(i1031_THIS.z * newVar_1394)));
/*GENERATED*/                Quaternion newVar_722 = Quaternion.Lerp(this.slowLocalHipRot, newVar_723, 0.1f);
/*GENERATED*/                this.slowLocalHipRot = newVar_722;
/*GENERATED*/                Quaternion i1041_THIS = this.realBodyRot;
/*GENERATED*/                Vector3 i1042_vector = this.hipPosRel;
/*GENERATED*/                float i1045_x = (i1041_THIS.x * 2);
/*GENERATED*/                float i1046_y = (i1041_THIS.y * 2);
/*GENERATED*/                float i1047_z = (i1041_THIS.z * 2);
/*GENERATED*/                float i1048_xx = (i1041_THIS.x * i1045_x);
/*GENERATED*/                float i1049_yy = (i1041_THIS.y * i1046_y);
/*GENERATED*/                float i1050_zz = (i1041_THIS.z * i1047_z);
/*GENERATED*/                float i1051_xy = (i1041_THIS.x * i1046_y);
/*GENERATED*/                float i1052_xz = (i1041_THIS.x * i1047_z);
/*GENERATED*/                float i1053_yz = (i1041_THIS.y * i1047_z);
/*GENERATED*/                float i1054_wx = (i1041_THIS.w * i1045_x);
/*GENERATED*/                float i1055_wy = (i1041_THIS.w * i1046_y);
/*GENERATED*/                float i1056_wz = (i1041_THIS.w * i1047_z);
/*GENERATED*/                Vector3 i1059_b = this.realBodyPos;
/*GENERATED*/                this.hipPosAbs.x = (
/*GENERATED*/                    ((1 + -(i1049_yy + i1050_zz)) * i1042_vector.x) + 
/*GENERATED*/                    ((i1051_xy + -i1056_wz) * i1042_vector.y) + 
/*GENERATED*/                    ((i1052_xz + i1055_wy) * i1042_vector.z) + 
/*GENERATED*/                    i1059_b.x);
/*GENERATED*/                this.hipPosAbs.y = (
/*GENERATED*/                    ((i1051_xy + i1056_wz) * i1042_vector.x) + 
/*GENERATED*/                    ((1 + -(i1048_xx + i1050_zz)) * i1042_vector.y) + 
/*GENERATED*/                    ((i1053_yz + -i1054_wx) * i1042_vector.z) + 
/*GENERATED*/                    i1059_b.y);
/*GENERATED*/                this.hipPosAbs.z = (
/*GENERATED*/                    ((i1052_xz + -i1055_wy) * i1042_vector.x) + 
/*GENERATED*/                    ((i1053_yz + i1054_wx) * i1042_vector.y) + 
/*GENERATED*/                    ((1 + -(i1048_xx + i1049_yy)) * i1042_vector.z) + 
/*GENERATED*/                    i1059_b.z);
/*GENERATED*/                bool _753 = (this.hipFlexibility == 0);
/*GENERATED*/                if (_753)  {
/*GENERATED*/                    this.hipRotAbs = this.realBodyRot;
/*GENERATED*/                } else  {
/*GENERATED*/                    Quaternion i1060_lhs = this.slowLocalHipRot;
/*GENERATED*/                    Quaternion i1061_rhs = this.realBodyRot;
/*GENERATED*/                    Quaternion newVar_727 = new Quaternion((
/*GENERATED*/                        (i1060_lhs.w * i1061_rhs.x) + 
/*GENERATED*/                        (i1060_lhs.x * i1061_rhs.w) + 
/*GENERATED*/                        (i1060_lhs.y * i1061_rhs.z) + 
/*GENERATED*/                        -(i1060_lhs.z * i1061_rhs.y)), (
/*GENERATED*/                        (i1060_lhs.w * i1061_rhs.y) + 
/*GENERATED*/                        (i1060_lhs.y * i1061_rhs.w) + 
/*GENERATED*/                        (i1060_lhs.z * i1061_rhs.x) + 
/*GENERATED*/                        -(i1060_lhs.x * i1061_rhs.z)), (
/*GENERATED*/                        (i1060_lhs.w * i1061_rhs.z) + 
/*GENERATED*/                        (i1060_lhs.z * i1061_rhs.w) + 
/*GENERATED*/                        (i1060_lhs.x * i1061_rhs.y) + 
/*GENERATED*/                        -(i1060_lhs.y * i1061_rhs.x)), (
/*GENERATED*/                        (i1060_lhs.w * i1061_rhs.w) + 
/*GENERATED*/                        -(i1060_lhs.x * i1061_rhs.x) + 
/*GENERATED*/                        -(i1060_lhs.y * i1061_rhs.y) + 
/*GENERATED*/                        -(i1060_lhs.z * i1061_rhs.z)));
/*GENERATED*/                    Quaternion newVar_726 = Quaternion.Lerp(this.realBodyRot, newVar_727, this.hipFlexibility);
/*GENERATED*/                    this.hipRotAbs = newVar_726;
/*GENERATED*/                }
/*GENERATED*/            }
/*GENERATED*/        }

        Quaternion oldHipLenHelp = Quaternion.identity;

/*GENERATED*/        [Optimize]
/*GENERATED*/        void calcHipLenHelp() {
/*GENERATED*/            bool _1906 = (this.steps.Count < 2);
/*GENERATED*/            if (_1906)  {
/*GENERATED*/                return ;
/*GENERATED*/            }
/*GENERATED*/            Step2 left = this.steps[this.leadingLegLeft];
/*GENERATED*/            Step2 right = this.steps[this.leadingLegRight];
/*GENERATED*/            Vector3 leg1 = this.surfaceDetector.detect(left.posAbs, Vector3.up);
/*GENERATED*/            Vector3 leg2 = this.surfaceDetector.detect(right.posAbs, Vector3.up);
/*GENERATED*/            Vector3 newVar_1888 = this.surfaceDetector.detect(left.basisAbs, Vector3.up);
/*GENERATED*/            Vector3 i1913_b = left.posAbs;
/*GENERATED*/            float i1916_diff_x = (newVar_1888.x + -i1913_b.x);
/*GENERATED*/            float i1917_diff_y = (newVar_1888.y + -i1913_b.y);
/*GENERATED*/            float i1918_diff_z = (newVar_1888.z + -i1913_b.z);
/*GENERATED*/            float d1 = (
/*GENERATED*/                (float)(Math.Sqrt(((i1916_diff_x * i1916_diff_x) + (i1917_diff_y * i1917_diff_y) + (i1918_diff_z * i1918_diff_z)))) + 
/*GENERATED*/                (0.1f * this.midLen));
/*GENERATED*/            Vector3 newVar_1891 = this.surfaceDetector.detect(right.basisAbs, Vector3.up);
/*GENERATED*/            Vector3 i1921_b = right.posAbs;
/*GENERATED*/            float i1924_diff_x = (newVar_1891.x + -i1921_b.x);
/*GENERATED*/            float i1925_diff_y = (newVar_1891.y + -i1921_b.y);
/*GENERATED*/            float i1926_diff_z = (newVar_1891.z + -i1921_b.z);
/*GENERATED*/            float d2 = (
/*GENERATED*/                (float)(Math.Sqrt(((i1924_diff_x * i1924_diff_x) + (i1925_diff_y * i1925_diff_y) + (i1926_diff_z * i1926_diff_z)))) + 
/*GENERATED*/                (0.1f * this.midLen));
/*GENERATED*/            bool _1907 = !left.dockedState;
/*GENERATED*/            if (_1907)  {
/*GENERATED*/                d1 = (d1 + -this.midLen);
/*GENERATED*/            }
/*GENERATED*/            bool _1908 = !right.dockedState;
/*GENERATED*/            if (_1908)  {
/*GENERATED*/                d2 = (d2 + -this.midLen);
/*GENERATED*/            }
/*GENERATED*/            float forPare_2040 = (d1 + d2);
/*GENERATED*/            float newVar_1895 = (1 + -(d1 / forPare_2040));
/*GENERATED*/            Vector3 i1927_a = this.up;
/*GENERATED*/            float newVar_1894_x = 0;
/*GENERATED*/            float newVar_1894_y = 0;
/*GENERATED*/            float newVar_1894_z = 0;
/*GENERATED*/            newVar_1894_x = (i1927_a.x * newVar_1895);
/*GENERATED*/            newVar_1894_y = (i1927_a.y * newVar_1895);
/*GENERATED*/            newVar_1894_z = (i1927_a.z * newVar_1895);
/*GENERATED*/            float newVar_1893_x = 0;
/*GENERATED*/            float newVar_1893_y = 0;
/*GENERATED*/            float newVar_1893_z = 0;
/*GENERATED*/            newVar_1893_x = newVar_1894_x;
/*GENERATED*/            newVar_1893_y = newVar_1894_y;
/*GENERATED*/            newVar_1893_z = newVar_1894_z;
/*GENERATED*/            float dbgPoint1_x = 0;
/*GENERATED*/            float dbgPoint1_y = 0;
/*GENERATED*/            float dbgPoint1_z = 0;
/*GENERATED*/            dbgPoint1_x = (leg1.x + newVar_1893_x);
/*GENERATED*/            dbgPoint1_y = (leg1.y + newVar_1893_y);
/*GENERATED*/            dbgPoint1_z = (leg1.z + newVar_1893_z);
/*GENERATED*/            float newVar_1900 = (1 + -(d2 / forPare_2040));
/*GENERATED*/            Vector3 i1933_a = this.up;
/*GENERATED*/            float newVar_1899_x = 0;
/*GENERATED*/            float newVar_1899_y = 0;
/*GENERATED*/            float newVar_1899_z = 0;
/*GENERATED*/            newVar_1899_x = (i1933_a.x * newVar_1900);
/*GENERATED*/            newVar_1899_y = (i1933_a.y * newVar_1900);
/*GENERATED*/            newVar_1899_z = (i1933_a.z * newVar_1900);
/*GENERATED*/            float newVar_1898_x = 0;
/*GENERATED*/            float newVar_1898_y = 0;
/*GENERATED*/            float newVar_1898_z = 0;
/*GENERATED*/            newVar_1898_x = newVar_1899_x;
/*GENERATED*/            newVar_1898_y = newVar_1899_y;
/*GENERATED*/            newVar_1898_z = newVar_1899_z;
/*GENERATED*/            float dbgPoint2_x = 0;
/*GENERATED*/            float dbgPoint2_y = 0;
/*GENERATED*/            float dbgPoint2_z = 0;
/*GENERATED*/            dbgPoint2_x = (leg2.x + newVar_1898_x);
/*GENERATED*/            dbgPoint2_y = (leg2.y + newVar_1898_y);
/*GENERATED*/            dbgPoint2_z = (leg2.z + newVar_1898_z);
/*GENERATED*/            Vector3 Z = new Vector3((dbgPoint1_x + -dbgPoint2_x), (dbgPoint1_y + -dbgPoint2_y), (dbgPoint1_z + -dbgPoint2_z));
/*GENERATED*/            Vector3 i1942_Y = this.up;
/*GENERATED*/            Vector3 newVar_1992 = Vector3.Normalize(Z);
/*GENERATED*/            Vector3 i1945_a = new Vector3(
/*GENERATED*/                ((i1942_Y.y * newVar_1992.z) + -(i1942_Y.z * newVar_1992.y)), 
/*GENERATED*/                ((i1942_Y.z * newVar_1992.x) + -(i1942_Y.x * newVar_1992.z)), 
/*GENERATED*/                ((i1942_Y.x * newVar_1992.y) + -(i1942_Y.y * newVar_1992.x)));
/*GENERATED*/            Vector3 i1943_X = Vector3.Normalize(i1945_a);
/*GENERATED*/            Vector3 i1950_a = new Vector3(
/*GENERATED*/                ((newVar_1992.y * i1943_X.z) + -(newVar_1992.z * i1943_X.y)), 
/*GENERATED*/                ((newVar_1992.z * i1943_X.x) + -(newVar_1992.x * i1943_X.z)), 
/*GENERATED*/                ((newVar_1992.x * i1943_X.y) + -(newVar_1992.y * i1943_X.x)));
/*GENERATED*/            Vector3 newVar_2011 = Vector3.Normalize(i1950_a);
/*GENERATED*/            Quaternion newVar_1904 = MUtil.qToAxes(
/*GENERATED*/                i1943_X.x, 
/*GENERATED*/                i1943_X.y, 
/*GENERATED*/                i1943_X.z, 
/*GENERATED*/                newVar_2011.x, 
/*GENERATED*/                newVar_2011.y, 
/*GENERATED*/                newVar_2011.z, 
/*GENERATED*/                newVar_1992.x, 
/*GENERATED*/                newVar_1992.y, 
/*GENERATED*/                newVar_1992.z);
/*GENERATED*/            Quaternion newVar_1903 = Quaternion.Lerp(this.oldHipLenHelp, newVar_1904, 0.2f);
/*GENERATED*/            this.oldHipLenHelp = newVar_1903;
/*GENERATED*/            Quaternion newVar_1905 = Quaternion.Lerp(this.wantedHipRot, this.oldHipLenHelp, 0.8f);
/*GENERATED*/            this.wantedHipRot = newVar_1905;
/*GENERATED*/        }
        public static Vector3 rotDisp(Quaternion rot, Vector3 vec) {
            return (ExtensionMethods.rotate(rot, vec) - vec);
        }

        [HideInInspector] public float emphasis;
        public Vector3 realWantedSpeed;

        public virtual void tickSteps(float dt) {
            Step2 right = (((this.leadingLegRight < this.steps.Count)) ? (this.steps[this.leadingLegRight]) : (null));
            Step2 left = (((this.leadingLegLeft < this.steps.Count)) ? (this.steps[this.leadingLegLeft]) : (null));
            if ((right != null))  {
                right.additionalDisplacement = new Vector3(-this.emphasis, 0, 0);
            }
            for (int stepIndex = 0; (stepIndex < this.steps.Count); (stepIndex)++)  {
                Step2 step = this.steps[stepIndex];
                float hDif = MyMath.clamp(((step.bestTargetProgressiveAbs.y - (step.posAbs.y + step.lastDockedAtLocal.y)) / step.maxLen), -1, 1);
                step.undockHDif = (1 + Math.Max(-0.8f, (((hDif > 0)) ? ((hDif * 5)) : (hDif))));
                 {
                    float value = (-1 + Math.Min(1, step.landTime));
                    step.fromAbove += value;
                    if (this.collectSteppingHistory)  {
                        step.paramHistory.setValue(HistoryInfoBean.lt, value);
                    }
                }
                float restTime = 0.5f;
                if ((step.dockedState && (step.landTime < restTime)))  {
                    float value = ((-1 + (step.landTime / restTime)) * 5);
                    step.fromAbove += value;
                    if (this.collectSteppingHistory)  {
                        step.paramHistory.setValue(HistoryInfoBean.lt2, value);
                    }
                }
                if (!step.dockedState)  {
                    for (int index = 0; (index < step.affectedByProgress.Count); (index)++)  {
                        StepNeuro<Step2> affected = step.affectedByProgress[index];
                        float value = (affected.add + (affected.mul * step.progress));
                        affected.leg.fromAbove += value;
                        if (this.collectSteppingHistory)  {
                            affected.leg.paramHistory.setValue(affected.desc, value);
                        }
                    }
                } else  {
                     {
                        for (int index = 0; (index < step.affectedByDeviation.Count); (index)++)  {
                            StepNeuro<Step2> affected = step.affectedByDeviation[index];
                            float value = (affected.add + (affected.mul * step.deviation));
                            affected.leg.fromAbove += value;
                            if (this.collectSteppingHistory)  {
                                affected.leg.paramHistory.setValue(affected.desc, value);
                            }
                        }
                    }
                }
            }
            if (((left != null) && (right != null)))  {
                left.forbidHalf = this.bipedalForbidPlacement;
                right.forbidHalf = this.bipedalForbidPlacement;
                if (this.bipedalForbidPlacement)  {
                    left.forbidHalfPos = right.posAbs;
                    right.forbidHalfPos = left.posAbs;
                    left.forbidHalfDir = ExtensionMethods.normalized(ExtensionMethods.withSetY(ExtensionMethods.sub(left.basisAbs, right.basisAbs), 0));
                    right.forbidHalfDir = -left.forbidHalfDir;
                }
                if (((this.forceBipedalEarlyStep && right.dockedState) && left.dockedState))  {
                    Vector3 zeroSpeed = ExtensionMethods.withSetY(this.imCenterSpeed, 0);
                    if ((ExtensionMethods.length(zeroSpeed) > (0.2f * this.midLen)))  {
                        float sRight = ExtensionMethods.scalarProduct(ExtensionMethods.sub(right.posAbs, this.realBodyPos), zeroSpeed);
                        float sLeft = ExtensionMethods.scalarProduct(ExtensionMethods.sub(left.posAbs, this.realBodyPos), zeroSpeed);
                        if (((sRight < 0) && (sLeft < 0)))  {
                            left.fromAbove -= (sLeft / this.midLen);
                            if (this.collectSteppingHistory)  {
                                left.paramHistory.setValue(HistoryInfoBean.bipedEarlyStep, (-sLeft / this.midLen));
                            }
                            right.fromAbove -= (sRight / this.midLen);
                            if (this.collectSteppingHistory)  {
                                right.paramHistory.setValue(HistoryInfoBean.bipedEarlyStep, (-sRight / this.midLen));
                            }
                        }
                    }
                }
            }
            if (this.collectSteppingHistory)  {
                foreach (Step2 step in this.steps) {
                    step.paramHistory.setValue(HistoryInfoBean.fromAbove, step.fromAbove);
                }
            }
            float biggestFromAbove = 0;
            Step2 biggestStep = null;
            for (int i = 0; (i < this.steps.Count); (i)++)  {
                Step2 step = this.steps[i];
                if ((step.dockedState && (step.fromAbove > biggestFromAbove)))  {
                    biggestFromAbove = step.fromAbove;
                    biggestStep = step;
                }
            }
            if ((biggestStep != null))  {
                biggestStep.beginStep(1);
            }
            for (int i = 0; (i < this.steps.Count); (i)++)  {
                MoveenSkelBase skel = this.legSkel[i];
                Step2 step = this.steps[i];
                step.tick(dt);
                if ((skel != null))  {
                    skel.setTarget(step.posAbs, step.footOrientation);
                    skel.tick(dt);
                    step.comfortFromSkel = skel.comfort;
                    step.posAbs = skel.limitedResultTarget;
                } else  {
                     {
                    }
                }
            }
        }
        void calcAbs(float dt) {
            float bodySpeedForLeg = Math.Min(
                this.horizontalMotor.maxSpeed, 
                Math.Max(ExtensionMethods.length(this.realSpeed), ExtensionMethods.length((this.realBodyPos - this.inputWantedPos))));
            for (int i = 0; (i < this.steps.Count); (i)++)  {
                Step2 step = this.steps[i];
                step.g = this.g;
                step.basisAbs = step.thisTransform.position;
                step.projectedRot = this.projectedRot;
                step.legSpeed = MyMath.max(
                    0, 
                    step.stepSpeedMin, 
                    (bodySpeedForLeg * step.stepSpeedBodySpeedMul), 
                    (ExtensionMethods.length(this.realBodyAngularSpeed) * step.stepSpeedBodyRotSpeedMul));
                step.bodyPos = this.imCenter;
                step.bodyRot = this.realBodyRot;
                step.bodySpeed = this.imActualCenterSpeed;
                step.bodySpeedMax = this.horizontalMotor.maxSpeed;
                step.calcAbs(dt, this.virtualForLegs, this.inputWantedRot);
                step.fromAbove = -1;
            }
        }
        public virtual void reset(Vector3 pos, Quaternion rot) {
            this.inputWantedPos = pos;
            this.inputWantedRot = rot;
            this.realBodyPos = pos;
            this.realBodyRot = rot;
            this.hipPosAbs = (ExtensionMethods.rotate(this.realBodyRot, this.hipPosRel) + this.realBodyPos);
            this.hipRotAbs = this.realBodyRot;
            this.calculatedCOG = (ExtensionMethods.rotate(this.realBodyRot, new Vector3(0, this.cogUpDown, 0)) + this.realBodyPos);
            this.imCenter = this.realBodyPos;
            this.imBody = this.realBodyPos;
            for (int index = 0; (index < this.steps.Count); (index)++)  {
                this.steps[index].reset(pos, rot);
            }
        }
        public virtual Vector3 project(Vector3 input) {
            return this.surfaceDetector.detect(input, Vector3.up);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using moveen.core;
using moveen.descs;
using moveen.utils;
using UnityEditor;
using UnityEngine;


namespace moveen.editor {

    public class RegressionEntry {
        public FieldInfo fieldInfo;
        public object targetObject;
        public float linearEstimation;
        public float quadraticEstimation;

        public virtual void estimate(MoveenSkelWithBonesEditor ed) {
        }

        public virtual void setNewValue() {
        }
        public virtual void setOldValue() {
        }

    }

    public class FloatEntry : RegressionEntry {
        public float currentValue;
        public float step;

        public float min;
        public float max;

        public FloatEntry clone() {
            return new FloatEntry {
                max = max,
                min = min,
                fieldInfo = fieldInfo,
                targetObject = targetObject,
                currentValue = currentValue,
                step = step,
                linearEstimation = linearEstimation,
                quadraticEstimation = quadraticEstimation
            };
        }
        
        public override void setNewValue() {
            //TODO use max/min
            fieldInfo.SetValue(targetObject, currentValue + step);
        }

        public override void setOldValue() {
            fieldInfo.SetValue(targetObject, currentValue);
        }

        public override void estimate(MoveenSkelWithBonesEditor ed) {
            estimateImpl(ed);

            FloatEntry entry2 = clone();
            entry2.step *= -1;
            entry2.estimateImpl(ed);
            if (quadraticEstimation > entry2.quadraticEstimation) {
                step = entry2.step;
                quadraticEstimation = entry2.quadraticEstimation;
                linearEstimation = entry2.linearEstimation;
            }

        }

        private void estimateImpl(MoveenSkelWithBonesEditor ed) {
            fieldInfo.SetValue(targetObject, currentValue + step);
            ed.skel.updateData();
            ed.skel.reset();
            linearEstimation = ed.evaluate();
            quadraticEstimation = ed.quadraticEstimation;
        }
    }

    public class Vector3Entry : RegressionEntry {
        public Vector3 currentValue;
        public Vector3 step;

        public Vector3Entry clone() {
            return new Vector3Entry {
                fieldInfo = fieldInfo,
                targetObject = targetObject,
                currentValue = currentValue,
                step = step,
                linearEstimation = linearEstimation,
                quadraticEstimation = quadraticEstimation
            };
        }

        public override void setNewValue() {
            fieldInfo.SetValue(targetObject, (currentValue + step).normalized);
        }

        public override void setOldValue() {
            fieldInfo.SetValue(targetObject, currentValue);
        }

        public override void estimate(MoveenSkelWithBonesEditor ed) {
            estimateImpl(ed);
            Vector3Entry entry2 = clone();
            entry2.step = step * -1;
            entry2.estimateImpl(ed);
            if (quadraticEstimation > entry2.quadraticEstimation) {
                step = entry2.step;
                quadraticEstimation = entry2.quadraticEstimation;
                linearEstimation = entry2.linearEstimation;
            }
        }

        private void estimateImpl(MoveenSkelWithBonesEditor ed) {
            fieldInfo.SetValue(targetObject, (currentValue + step).normalized);
            ed.skel.updateData();
            ed.skel.reset();
            linearEstimation = ed.evaluate();
            quadraticEstimation = ed.quadraticEstimation;
        }
    }

    [CustomEditor(typeof(MoveenSkelWithBones), true)]
    [CanEditMultipleObjects]
    public class MoveenSkelWithBonesEditor : Editor {
        public static float IK_DISK_R1 = 15;
        public static float IK_DISK_R2 = 20;
        public static Color IK_GIZMO_COLOR = new Color(1, 0.7f, 0, 0.8f);
        public static Color BONE_COLOR = new Color(0.5f, 0.9f, 0.5f);
        public static Color LIMIT_ANGLE_COLOR = new Color(0.5f, 0.9f, 0.5f, 0.6f);
        
        public MoveenSkelWithBones skel;
        public static bool solveIk;

        public void OnEnable() {
//            Tools.hidden = true;
            skel = (MoveenSkelWithBones) target;

//            if (skel.wanteds == null) {
                initWanteds(skel);
//            }
        }

        private void OnDisable() {
//            Tools.hidden = false;
        }

        public void initWanteds(object input) {
            skel.wanteds = new List<Vector3>();
            foreach (FieldInfo field in MUtil.getFieldsWhereAttributes(input.GetType(), typeof(CustomSkelResultAttribute))) {
                skel.wanteds.Add(skel.transform.InverseTransformPoint((Vector3) field.GetValue(input)));
            }
        }

        public override void OnInspectorGUI() {
//            bool oldValue = solveIk;
//            solveIk = GUILayout.Toggle(solveIk, "IK help (editor only)");
//            if (oldValue != solveIk) {
//                initWanteds(skel);
//            }
            if (solveIk) {
//                if (GUILayout.Button("Solve IK")) {
//                    solve(100, 0.95f);
//                }

                if (GUI.changed) {
                    initWanteds(skel);
                }
            }

            EditorGUI.BeginChangeCheck();
            DrawDefaultInspector();
            if (EditorGUI.EndChangeCheck()) {
                if (solveIk) {
                    skel.reset();
                    initWanteds(skel);
                }
                // Do something when the property changes 
            }
        }

        public void solve(int steps, float speed) {
            for (int i = 0; i < steps; i++) {
//                step *= speed;
                List<RegressionEntry> entries = new List<RegressionEntry>();

//                float curLinear = evaluate();
                float curQuadratic = quadraticEstimation;
//                if (step > 1) break;
//                step = quadraticDifference;

                collectEntries(skel, curQuadratic, entries);

//                float sumEst = 0;
//                foreach (var e in entries) sumEst += e.linearEstimation;//must stop irrelevance
                foreach (var e in entries) {
//                    if (Math.Abs(e.quadraticEstimation - step) < 0.01f) continue;
                    if (e.quadraticEstimation < curQuadratic * 0.99f) { 
//                        Debug.Log("estimations: " + e.quadraticEstimation + " " + step);
                    e.setNewValue();
//                        float newValue = e.currentValue + e.step;
//                        float newValue = e.currentValue + e.step * (e.linearEstimation / sumEst);
//                        e.fieldInfo.SetValue(skel, MyMath.clamp(newValue, e.min, e.max));
//                        if (newValue > 0 && newValue < 100500) e.fieldInfo.SetValue(skel, newValue);
//                        Debug.Log("set: " + e.fieldInfo.Name + " " + e.currentValue + " + " + e.step + " = " + (e.currentValue + e.step));
                    }

                    
//                    e.fieldInfo.SetValue(skel, e.currentValue + e.step * (e.linearEstimation / sumQDif));
                }

                skel.updateData();
                skel.reset();
//                float newEval = evaluate();

                //when new state is in errors space
                //  or estimation is getting worse
                 if (skel.isInError) {
//                 if (skel.isInError || newEval > 1) {
//                if (skel.isInError || quadraticEstimation > step) {
//                    Debug.Log("worse: " + skel.isInError + " " + newEval + " > " + curEstimation);
                    foreach (var e in entries) e.setOldValue();
//                    foreach (var e in entries) if (e.quadraticEstimation < curQuadratic) e.setOldValue();
//                    foreach (var e in entries) if (e.quadraticEstimation < step) e.fieldInfo.SetValue(skel, MyMath.clamp(e.currentValue, e.min, e.max));
                    skel.updateData();
                    skel.reset();
                    break;
                }

            }
        }

        private void collectEntries(object skel, float step, List<RegressionEntry> entries) {
            foreach (FieldInfo field in MUtil.getFieldsWhereAttributes(skel.GetType(), typeof(CustomSkelControlAttribute))) {
                CustomSkelControlAttribute[] attr = (CustomSkelControlAttribute[]) field.GetCustomAttributes(typeof(CustomSkelControlAttribute), true);

                if (field.FieldType == typeof(float)) {
//radiuses
                    float val = (float) field.GetValue(skel);
                    float curStep = Math.Min(step, Math.Abs(val * 0.01f)); //to avoid abnormal value jumps when fails to reach minimum
                    FloatEntry entry = new FloatEntry {
                        currentValue = val,
                        min = attr[0].min,
                        max = attr[0].max,
                        fieldInfo = field,
                        targetObject = skel,
                        step = curStep
                    };
                    entry.estimate(this);
//                        FloatEntry entry2 = entry.clone();
//                        entry2.step *= -1;
//                        entry2.estimate(this);

                    field.SetValue(skel, entry.currentValue);
                    entries.Add(entry);
//                        entries.Add(entry.linearEstimation < entry2.linearEstimation ? entry : entry2);
                } else if (field.FieldType == typeof(Vector3)) {
//axes
                    Vector3 val = (Vector3) field.GetValue(skel);
                    float curStep = Math.Min(step, Math.Abs(val.length() * 0.01f)); //to avoid abnormal value jumps when fails to reach minimum
                    Vector3Entry entryX = new Vector3Entry {
                        currentValue = val,
                        fieldInfo = field,
                        targetObject = skel,
                        step = new Vector3(curStep, 0, 0)
                    };
                    entryX.estimate(this);
                    Vector3Entry entryY = new Vector3Entry {
                        currentValue = val,
                        fieldInfo = field,
                        targetObject = skel,
                        step = new Vector3(0, curStep, 0)
                    };
                    entryY.estimate(this);
                    Vector3Entry entryZ = new Vector3Entry {
                        currentValue = val,
                        fieldInfo = field,
                        targetObject = skel,
                        step = new Vector3(0, 0, curStep)
                    };
                    entryZ.estimate(this);
                    
                    field.SetValue(skel, val);

                    if (entryX.quadraticEstimation < entryY.quadraticEstimation && entryX.quadraticEstimation < entryZ.quadraticEstimation) entries.Add(entryX);
                    else if (entryY.quadraticEstimation < entryZ.quadraticEstimation) entries.Add(entryY);
                    else entries.Add(entryZ);
                } else {
                    collectEntries(field.GetValue(skel), step, entries);
                }
            }
        }

        public float quadraticEstimation;

        public float evaluate() {
            float linearDifference = 0;
            quadraticEstimation = 0;
            int curIndex = 0;
//            float result = float.MaxValue;
            foreach (FieldInfo field in MUtil.getFieldsWhereAttributes(skel.GetType(), typeof(CustomSkelResultAttribute))) {
                if (field.FieldType == typeof(Vector3)) {
                    Vector3 value = skel.transform.InverseTransformPoint((Vector3) field.GetValue(skel));
                    if (float.IsNaN(value.x) || float.IsNaN(value.y) || float.IsNaN(value.z)) {
                        Debug.Log(value);
                    }
                    Vector3 wantedValue = skel.wanteds[curIndex];
//                    result = Math.Min(result, value.dist(wantedValue));
                    float difference = value.dist(wantedValue);
                    linearDifference += difference;
                    quadraticEstimation += MyMath.pow(difference, 2);
//                    if (curIndex == i) result += value.dist(wantedValue);
                    curIndex++;
                }
            {
                float difference = skel.limitedResultTarget.dist(skel.targetPos);
                linearDifference += difference;
                quadraticEstimation += MyMath.pow(difference, 2);
            }
        }
        return linearDifference;
        }

        public void OnSceneGUI() {
            //MUtil.logEvent(this, "OnSceneGUI");

            Undo.RecordObject(skel, "Moveen target position");

            //specifics when there is no target connected
            if (skel.target == null) {
                if (Application.isPlaying) {
                    //to directly control target position 
                    skel.targetPos = Handles.PositionHandle(skel.targetPos, Quaternion.identity);
                } else {
                    //to indirectly control target position
                    //AND to move this target position along with the skel itself
                    if (Tools.current.Equals(Tool.Move)) {
                        skel.targetPosRel =
                            skel.transform.InverseTransformPoint(Handles.PositionHandle(skel.transform.TransformPoint(skel.targetPosRel), Quaternion.identity));
                    }
                    if (Tools.current.Equals(Tool.Rotate)) {
                        skel.targetRotRel =
                            Handles.RotationHandle(skel.targetRot, skel.targetPos).rotSub(skel.gameObject.transform.rotation);
                    }
                }
            }

            if (!Application.isPlaying) {
                if (solveIk) {
                    for (int i = 0; i < skel.wanteds.Count; i++) {
                        skel.wanteds[i] =
                            skel.transform.InverseTransformPoint(Handles.PositionHandle(skel.transform.TransformPoint(skel.wanteds[i]), Quaternion.identity));
                    }
                }

                editType(skel.transform, skel);
                //TODO only if smthng is changed
                if (GUI.changed) {
                    if (solveIk) {
                        solve(5, 0.5f);
                    }

                    skel.needsUpdate = true;
                    if (!Application.isPlaying) {
                        skel.needsReset = true;
                        OrderedTick.forceTick(1f / 50);
                    }
                }
            }

            Undo.FlushUndoRecordObjects();
        }

        private static void editType(Transform main, object skel) {
            foreach (FieldInfo field in MUtil.getFieldsWhereAttributes(skel.GetType(), typeof(CustomSkelControlAttribute))) {
                object value = field.GetValue(skel);
                Type fieldType = field.FieldType;
                editType(main, value);

                if (Tools.current.Equals(Tool.Move)) {
                    if (fieldType == typeof(Vector3)) {
                        Vector3 cur = main.TransformPoint((Vector3) value);
                        Handles.Label(cur, field.Name);
                        cur = Handles.PositionHandle(cur, Quaternion.identity);
                        field.SetValue(skel, main.InverseTransformPoint(cur));
                    }
                    if (fieldType == typeof(P<Vector3>)) {
                        P<Vector3> f = (P<Vector3>) value;
                        Vector3 cur = main.TransformPoint(f.v);
                        Handles.Label(cur, field.Name);
                        cur = Handles.PositionHandle(cur, Quaternion.identity);
                        f.v = cur;
                    }
                }
            }
        }

        public static void OnDrawGizmos(MoveenSkelWithBones component) {
            if (!component.isActiveAndEnabled) return;
            Gizmos.color = BONE_COLOR;
            float averageR = 0;
            for (int i = 0; i < component.bones.Count; i++) {
                Bone bone = component.bones[i];
                averageR += bone.r;
                bone.origin.tick(); //TODO ensure tick in root
                if (bone.origin.getRot().magnitude() < 0.3f) {
                    MUtil.log(component, "wrong quaternion: " + bone.origin.getRot());
                    //Debug.Log("  " + i);
                    continue;
                }
                UnityEditorUtils.diamond(bone.origin.getPos(), bone.origin.getRot().normalized(), bone.r);
            }

            averageR = component.bones.Count > 0 ? averageR / component.bones.Count : 0.04f;

            Type type = component.GetType();
            Gizmos.color = Color.green;
            var fieldInfos = MUtil.getFieldsWhereAttributes(type, typeof(CustomSkelResultAttribute));
            for (int index = 0; index < fieldInfos.Count; index++) {
                FieldInfo field = fieldInfos[index];
                if (field.FieldType == typeof(Vector3)) {
                    Vector3 cur = (Vector3) field.GetValue(component);
                    Gizmos.DrawWireSphere(cur, averageR * 0.1f);
                } else if (field.FieldType == typeof(P<Vector3>)) {
                    P<Vector3> cur = (P<Vector3>) field.GetValue(component);
                    Gizmos.DrawWireSphere(cur.v, averageR * 0.1f);
                }
            }
        }
    }
}

using System;
using System.Reflection;
using moveen.core;
using moveen.descs;
using moveen.utils;
using UnityEditor;
using UnityEngine;

namespace moveen.editor {
    [CustomEditor(typeof(MoveenStep2), true)]
    [CanEditMultipleObjects]
    public class MoveenStep2Editor : Editor {
        private MoveenStep2 component;

        public void OnEnable() {
            component = (MoveenStep2) target;
        }

        public override void OnInspectorGUI() {
            Step2 componentStep = component.step;
            if (componentStep.showTrajectory) EditorGUILayout.HelpBox("Don't forget to turn off \"showTrajectory\" as it eats editor performance", MessageType.Warning);
            DrawDefaultInspector();

            MoveenStep2.showInstrumentalInfo = EditorGUILayout.Foldout(MoveenStep2.showInstrumentalInfo, "instrumental info", true);
            if (MoveenStep2.showInstrumentalInfo) drawInstrumentalProperties(componentStep);
        }

        public static void drawInstrumentalProperties(object componentStep) {
            GUI.enabled = false;
            //EditorGUILayout.LabelField("hello!", EditorStyles.boldLabel);
            Type type = componentStep.GetType();
            foreach (FieldInfo field in MUtil.getFieldsWhereAttributes(type, typeof(InstrumentalInfoAttribute))) {
                if (field.GetCustomAttributes(typeof(InstrumentalInfoAttribute), true).Length == 0) continue;
                string fieldName = field.Name;
                object value = field.GetValue(componentStep);
                if (value is float) {
                    EditorGUILayout.FloatField(fieldName, (float) value);
                } else if (value is int) {
                    EditorGUILayout.IntField(fieldName, (int) value);
                } else if (value is string) {
                    EditorGUILayout.TextField(fieldName, (string) value);
                } else if (value is Vector3) {
                    EditorGUILayout.Vector3Field(fieldName, (Vector3) value);
                } else if (value is bool) {
                    EditorGUILayout.Toggle(fieldName, (bool) value);
                }
            }
            GUI.enabled = true;
        }

        public void OnSceneGUI() {
            if (Application.isPlaying) return;
            Transform withStepper = MoveenStep2Editor.withStepper(component.transform);

            if (withStepper == null) return;
            MoveenSkelWithBones skel = component.transform.GetComponent<MoveenSkelWithBones>();


            if (skel == null || component.step.detachedComfortPosRel) {
                Vector3 oldAbs = withStepper.TransformPoint(component.step.comfortPosRel);
//            Vector3 oldAbs = component.transform.parent.TransformPoint(component.step.bestTargetRel);
                Handles.Label(oldAbs, "comfort position");

                Undo.RecordObject(component, "Moveen target position");
                component.step.comfortPosRel = withStepper.InverseTransformPoint(Handles.PositionHandle(oldAbs, Quaternion.identity));
                //Undo.FlushUndoRecordObjects();seems like done automatically
            }

            if (skel != null && !component.step.detachedComfortPosRel) {
                Vector3 tipAbs = skel.transform.TransformPoint(skel.targetPosRel);
                component.step.comfortPosRel = withStepper.rotation.conjug().rotate(tipAbs - withStepper.position);//can't use Inverse, because in the enine - only rotation and translation is used
                component.step.reset(withStepper.position, withStepper.rotation);
            }

            if (GUI.changed) {//to react on comfort position handler (when detached)
                component.step.reset(withStepper.position, withStepper.rotation);
            }
        }

        private static Transform withStepper(Transform component) {
            Transform withStepper = null;
            Transform cur = component;
            while (cur != null) {
                if (cur.GetComponent<MoveenStepper5>() != null) {
                    withStepper = cur;
                    break;
                }
                cur = cur.parent;
            }
            return withStepper;
        }

//        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.Active)]
        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NonSelected | GizmoType.Active)]
        private static void Gizmo(MoveenStep2 component, GizmoType gizmoType) {
            if (!component.isActiveAndEnabled) return;
            Step2 step = component.step;
            if (step.thisTransform == null) return;//if not updated first time yet
            Transform withStepper = MoveenStep2Editor.withStepper(component.transform);

            Gizmos.color = new Color(0.5f, 0.5f, 0.5f);
            UnityEditorUtils.circle3d(step.bestTargetConservativeAbs, Quaternion.AngleAxis(0, Vector3.up), step.comfortRadius, 20);
            if (!Application.isPlaying) {
                if (step.showTrajectory) {
                    Gizmos.color = new Color(0, 1, 0);
                    Gizmos.DrawRay(step.posAbs, new Vector3(0, 1, 0) * step.undockTime * step.undockInitialStrength);

                    Vector3 far = step.posAbs + withStepper.rotation.rotate(new Vector3(1, 0, 0)) * step.comfortRadius;
                    Gizmos.color = new Color(1, 0, 0);
                    Gizmos.DrawRay(far, new Vector3(0, 1, 0) * step.targetDockR);

                    Step2 example = new Step2();

                    example.posAbs = step.posAbs;
                    example.oldPosAbs = step.posAbs;
//                        example.legPosFromStruct = example.legPosAbs;
                    example.undockPause = step.undockPause;
                    example.targetDockR = step.targetDockR;
                    example.undockInitialStrength = step.undockInitialStrength;
                    example.undockTime = step.undockTime;
//                    example.targetUnderground = step.targetUnderground;

                    example.bestTargetProgressiveAbs = far;
                    example.bestTargetConservativeAbs = step.posAbs;
                    example.bestTargetConservativeUnprojAbs = step.posAbs;
                    example.legSpeed = step.stepSpeedMin;
                    example.maxAcceleration = step.maxAcceleration;
                    example.basisAbs = step.thisTransform.position;
                    example.maxLen = step.maxLen;
                    example.beginStep(1);
                    example.isExample = true;
                    example.undockHDif = 1;
                    Vector3 lastPos = step.posAbs;

                    for (int j = 0; j < 1000; j++) {
                        example.tick(1f / 50);
                        Gizmos.color = new Color(0, 0, 1);
                        Gizmos.DrawLine(lastPos, example.posAbs);
                        lastPos = example.posAbs;
                        if (example.posAbs.dist(far) < example.targetDockR * 0.3f) break;
                    }
                    Gizmos.color = new Color(0, 1, 0);
                    Gizmos.DrawLine(lastPos, far);
                }
            }
            
            if (Application.isPlaying) {
                Gizmos.color = new Color(0, 1, 0);
                Gizmos.DrawWireSphere(step.bestTargetConservativeAbs, 0.03f);
                Gizmos.color = new Color(1, 1, 0);
                Gizmos.DrawWireSphere(step.bestTargetConservativeUnprojAbs2, 0.08f);
                Gizmos.color = new Color(0, 1, 1);
                Gizmos.DrawWireSphere(step.airTarget, 0.08f);
                Gizmos.color = new Color(1, 0, 0);
                Gizmos.DrawWireSphere(step.bestTargetProgressiveAbs, 0.03f);
                Gizmos.color = new Color(1, 1, 1);
                Gizmos.DrawWireSphere(step.curTarget, 0.05f);

                Gizmos.color = Color.green;

                if (step.wasTooLong) {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(step.posAbs, 0.2f);
                }
                if (step.dockedState) {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(step.posAbs, 0.15f);
                }
            }
        }
    }
}
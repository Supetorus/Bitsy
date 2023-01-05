using System;
using System.Collections.Generic;
using System.Reflection;
using moveen.descs;
using moveen.utils;
using UnityEditor;
using UnityEngine;

namespace moveen.editor {
    
    [CustomEditor(typeof(MoveenSkelLimb1), true)]
    [CanEditMultipleObjects]
    public class MoveenSkelLimb1Editor : MoveenSkelWithBonesEditor  {
        private bool showDefaultInspector;

        public override void OnInspectorGUI() {
            MoveenSkelLimb1 skel = (MoveenSkelLimb1) this.skel;
            GUIStyle labelStyle = new GUIStyle {
                fontStyle = FontStyle.Bold,
                fontSize = 20,
                margin = new RectOffset(10, 10, 10, 10)
            };
            
            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("Structure", labelStyle);
            EditorGUILayout.LabelField("");

            bool oldValue = solveIk;
            solveIk = GUILayout.Toggle(solveIk, "regression solver help (editor only)");
            if (oldValue != solveIk) {
                initWanteds(skel);
            }

            EditorGUILayout.LabelField("");
            EditorGUI.BeginChangeCheck();
            Undo.RecordObject(skel, "limb1editing");

            skel.r1 = EditorGUILayout.FloatField("Bone 1 length", skel.r1);
            skel.r2 = EditorGUILayout.FloatField("Bone 2 length", skel.r2);
            skel.footPlatformHeight = EditorGUILayout.FloatField("Foot platform height", skel.footPlatformHeight);
            
            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("Geometry connection", labelStyle);
            EditorGUILayout.LabelField("");

            skel.bonesGeometry[0] = (Transform) EditorGUILayout.ObjectField("Bone 1 geometry", skel.bonesGeometry[0], typeof(Transform), true);
            skel.bonesGeometry[1] = (Transform) EditorGUILayout.ObjectField("Bone 2 geometry", skel.bonesGeometry[1], typeof(Transform), true);
            skel.bonesGeometry[2] = (Transform) EditorGUILayout.ObjectField("Foot geometry", skel.bonesGeometry[2], typeof(Transform), true);
            
            EditorGUILayout.LabelField("");
            MoveenSkelLimb2Editor.addBonesButton(skel);
            showDefaultInspector = EditorGUILayout.Foldout(showDefaultInspector, "raw inspector", true);
            if (showDefaultInspector) {
                DrawDefaultInspector();
            }

            if (EditorGUI.EndChangeCheck()) {
                //why do we need both of these?
                EditorUtility.SetDirty(skel);
                skel.needsUpdate = true;
                
                if (solveIk) {
                    skel.reset();
                    initWanteds(skel);
                }
            }
        }

//        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.Active)]
        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NonSelected | GizmoType.Active)]
        private static void Gizmo(MoveenSkelLimb1 h, GizmoType gizmoType) {
            if (!h.isActiveAndEnabled) return;
            OnDrawGizmos(h);
            Gizmos.color = new Color(0, 1, 0, 0.4f);
            MoveenSkelLimb2Editor.drawSimpleSpiderGizmo(h.rotJoint, h.maxLen * 0.2f, h.transform.position, h.targetPosRel);

            if (((int) gizmoType & (int) GizmoType.InSelectionHierarchy) != 0 && solveIk) {
                Gizmos.color = IK_GIZMO_COLOR;
                Vector3 w2s = Camera.current.WorldToScreenPoint(h.targetPos);
                if (w2s.z > 0) UnityEditorUtils.fillStripe2D(w2s.getXy(), IK_DISK_R1, IK_DISK_R2, 20);
                drawIkHelp(h.transform, h);
            }
        }

        public static void draw2DLeg(Vector3 X, Vector3 Y, Vector3 pos, float size) {
//            Vector3 Z = X.crossProduct(Y);
//            X = Y.crossProduct(Z).normalized;
//                Gizmos.DrawLine(h.transform.position, h.transform.position + X * 0.2f + Y * 0.2f);
//                Gizmos.DrawLine(h.transform.position + X * 0.2f + Y * 0.2f, h.transform.position + X * 0.4f);
            Quaternion rot = MUtil.qToAxesXZ(X, Y);
            GL.PushMatrix();
            GL.LoadProjectionMatrix(Camera.current.projectionMatrix);

            GL.MultMatrix(Matrix4x4.TRS(
                pos,
                rot * Quaternion.AngleAxis(-45, Vector3.up), new Vector3(size, size, size)));
            diamond2d();
            GL.MultMatrix(Matrix4x4.TRS(
                pos + rot.rotate(new Vector3(size * MyMath.cos(MyMath.PI / 4), 0, size * MyMath.cos(MyMath.PI / 4))),
                rot * Quaternion.AngleAxis(45, Vector3.up), new Vector3(size, size, size)));
            diamond2d();

            GL.PopMatrix();
        }

        private static void diamond2d() {
            GL.Begin(GL.QUADS);
            GL.Color(Gizmos.color);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(0.3f, 0, 0.1f);
            GL.Vertex3(1   , 0, 0);
            GL.Vertex3(0.3f, 0, -0.1f);
            GL.End();
        }

        public static void drawIkHelp(Transform main, object skel) {
            foreach (FieldInfo field in MUtil.getFieldsWhereAttributes(skel.GetType(), typeof(CustomSkelResultAttribute))) {
                // if (field.GetCustomAttributes(typeof(CustomSkelResultAttribute), true).Length == 0) continue;
                object value = field.GetValue(skel);
                Type fieldType = field.FieldType;
                drawIkHelp(main, value);

                if (fieldType == typeof(Vector3)) {
                    Vector3 w2s = Camera.current.WorldToScreenPoint((Vector3) value);
                    if (w2s.z < 0) continue;
                    UnityEditorUtils.fillStripe2D(w2s.getXy(), IK_DISK_R1, IK_DISK_R2, 20);
                }
                if (fieldType == typeof(P<Vector3>)) {
                }
            }
        }

    }
}
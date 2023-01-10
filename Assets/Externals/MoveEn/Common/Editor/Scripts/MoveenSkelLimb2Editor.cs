using moveen.core;
using moveen.descs;
using moveen.utils;
using UnityEditor;
using UnityEngine;

namespace moveen.editor {
    [CustomEditor(typeof(MoveenSkelLimb2), true)]
    [CanEditMultipleObjects]
    public class MoveenSkelLimb2Editor : MoveenSkelWithBonesEditor {
        private bool showDefaultInspector;

        public override void OnInspectorGUI() {
            MoveenSkelLimb2 skel = (MoveenSkelLimb2) this.skel;
            
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
            Undo.RecordObject(skel, "limb2editing");
            skel.rA = EditorGUILayout.FloatField("Bone 1 length", skel.rA);
            skel.rB = EditorGUILayout.FloatField("Bone 2 length", skel.rB);
            skel.rC = EditorGUILayout.FloatField("Bone 3 length", skel.rC);
            skel.footPlatformHeight = EditorGUILayout.FloatField("Foot platform height", skel.footPlatformHeight);
            
            skel.style = EditorGUILayout.Toggle("Structure magic 1", skel.style);
            skel.z = EditorGUILayout.Toggle("Structure magic 2", skel.z);
            skel.styleRatio = EditorGUILayout.Slider("Structure magic 3", skel.styleRatio, 0, 1);
            
            EditorGUILayout.LabelField("");
            EditorGUILayout.LabelField("Geometry connection", labelStyle);
            EditorGUILayout.LabelField("");

            skel.bonesGeometry[0] = (Transform) EditorGUILayout.ObjectField("Bone 1 geometry", skel.bonesGeometry[0], typeof(Transform), true);
            skel.bonesGeometry[1] = (Transform) EditorGUILayout.ObjectField("Bone 2 geometry", skel.bonesGeometry[1], typeof(Transform), true);
            skel.bonesGeometry[2] = (Transform) EditorGUILayout.ObjectField("Bone 3 geometry", skel.bonesGeometry[2], typeof(Transform), true);
            skel.bonesGeometry[3] = (Transform) EditorGUILayout.ObjectField("Foot geometry", skel.bonesGeometry[3], typeof(Transform), true);
            
            addBonesButton(skel);
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

        public static void addBonesButton(MoveenSkelWithBones skel) {
            EditorGUILayout.LabelField("");
            GUI.enabled = skel.transform.childCount < skel.bones.Count;
            if (GUILayout.Button(new GUIContent("Add missing bones",
                "Create absent child bones. They will be affected by this limb even in the editor." +
                "\nNote, you can do without child bones if you connect external bones."))) {
                for (int i = skel.transform.childCount; i < skel.bones.Count; i++) {
                    if (i < skel.bones.Count - 1) {
                        new GameObject("bone " + (i + 1)).transform.parent = skel.transform;
                    } else {
                        new GameObject("bone foot").transform.parent = skel.transform;
                    }
                }
            }
            GUI.enabled = true;
        }

//        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.Active)]
        [DrawGizmo(GizmoType.InSelectionHierarchy | GizmoType.NonSelected | GizmoType.Active)]
        private static void Gizmo(MoveenSkelLimb2 h, GizmoType gizmoType) {
            if (!h.isActiveAndEnabled) return;
            OnDrawGizmos(h);
            Gizmos.color = new Color(0, 1, 0, 0.4f);
            drawSimpleSpiderGizmo(h.rotJoint, h.maxLen * 0.2f, h.transform.position, h.targetPosRel);

            if (((int) gizmoType & (int) GizmoType.InSelectionHierarchy) != 0 && solveIk) {
                Gizmos.color = IK_GIZMO_COLOR;
                Vector3 w2s = Camera.current.WorldToScreenPoint(h.targetPos);
                if (w2s.z > 0) UnityEditorUtils.fillStripe2D(w2s.getXy(), IK_DISK_R1, IK_DISK_R2, 20);
//                UnityEditorUtils.fillStripe2D(Camera.current.WorldToScreenPoint(h.transform.TransformPoint(h.rotJoint.axisRel)).getXy(), MoveenGizmos_Limb1.IK_DISK_R1, MoveenGizmos_Limb1.IK_DISK_R2, 20);
                MoveenSkelLimb1Editor.drawIkHelp(h.transform, h);
            }

            Gizmos.color = new Color(0, 0.5f, 0, 0.8f);
            
//            Gizmos.DrawLine(rotationJoint.basisAbs.v, rotationJoint.targetAbs.v);
            Gizmos.DrawLine(h.invisibleElbowJoint.basisAbs.v, h.invisibleElbowJoint.resultAbs.v);
            Gizmos.DrawLine(h.invisibleElbowJoint.resultAbs.v, h.invisibleElbowJoint.targetAbs.v);

        }

        public static void drawSimpleSpiderGizmo(JunctionSimpleSpider junctionSimpleSpider, float size, Vector3 posAbs, Vector3 targetAbs) {
            UnityEditorUtils.cylinder(posAbs, MUtil.qToAxesXZ(junctionSimpleSpider.axisAbs, targetAbs), 1, 10, 0, 1 * size, 0.1f * size, 0.1f * size);
            UnityEditorUtils.cylinder(posAbs,
                MUtil.qToAxesXZ(junctionSimpleSpider.secondaryAxisAbs, junctionSimpleSpider.axisAbs),
                1, 10, 0, 1 * size * 0.3f, 0.1f * size * 0.5f, 0.1f * size * 0.5f);
        }
    }
}
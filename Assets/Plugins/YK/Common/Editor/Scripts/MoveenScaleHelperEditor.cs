using moveen.core;
using moveen.descs;
using moveen.example;
using moveen.utils;
using UnityEditor;
using UnityEngine;

namespace moveen.editor {
    [CustomEditor(typeof(MoveenScaleHelper), true)]
    [CanEditMultipleObjects]
    public class MoveenScaleHelperEditor : Editor {

        public override void OnInspectorGUI() {
            if (GUILayout.Button("Mirror Z")) {

                foreach (var t in ((MoveenScaleHelper) target).transform.GetComponentsInChildren<Transform>()) {
                    t.transform.localPosition = t.transform.localPosition.mul(1, 1, -1);
                }
            }


            if (((MoveenScaleHelper) target).isActiveAndEnabled) {
                EditorGUILayout.HelpBox("This is an experimental implementation. It handles not all parameters and scripts. Check your scaled system after application.", MessageType.Info);
                EditorGUILayout.HelpBox("Be wary - this scaler is intrusive - it changes actual values and errors can accumulate. Make a copy before apply scaling.", MessageType.Warning);
                if (((MoveenScaleHelper) target).showWarning) {
                    EditorGUILayout.HelpBox("Wrong scale", MessageType.Error);
                }
            }
            GUI.enabled = false;
            EditorGUILayout.FloatField("Last Scale", ((MoveenScaleHelper) target).lastScale);
            GUI.enabled = true;
        }

        private void OnSceneGUI() {
            MoveenScaleHelper scaler = (MoveenScaleHelper) target;
            Transform transform = scaler.transform;
            float newScale = transform.lossyScale.x;
            if (newScale == scaler.lastScale) return;
            if (newScale < 0.001f || newScale > 1000f) {
                scaler.showWarning = true;
                return;
            }
            scaler.showWarning = false;
            Undo.RegisterFullObjectHierarchyUndo(transform.gameObject, "scale");

            foreach (var l in transform.GetComponentsInChildren<MoveenSkelLimb1>()) {
                l.r1 = l.r1 * newScale / scaler.lastScale;
                l.r2 = l.r2 * newScale / scaler.lastScale;
                l.footPlatformHeight = l.footPlatformHeight * newScale / scaler.lastScale;
                l.needsUpdate = true;
            }

            foreach (var l in transform.GetComponentsInChildren<MoveenSkelLimb2>()) {
                l.rA = l.rA * newScale / scaler.lastScale;
                l.rB = l.rB * newScale / scaler.lastScale;
                l.rC = l.rC * newScale / scaler.lastScale;
                l.footPlatformHeight = l.footPlatformHeight * newScale / scaler.lastScale;
                l.needsUpdate = true;
            }

            foreach (var l in transform.GetComponentsInChildren<MoveenStep2>()) {
            }

            foreach (var l in transform.GetComponentsInChildren<MoveControl1>()) {
                l.height = l.height * newScale / scaler.lastScale;
                l.camPosition = l.camPosition * newScale / scaler.lastScale;
            }

            foreach (var l in transform.GetComponentsInChildren<MoveControl2>()) {
                l.height = l.height * newScale / scaler.lastScale;
                l.camPosition = l.camPosition * newScale / scaler.lastScale;
            }

            foreach (var l in transform.GetComponentsInChildren<MoveenStepper5>()) {
                l.hipDeltaPos = l.hipDeltaPos * newScale / scaler.lastScale;
                l.needsUpdate = true;
            }

            foreach (var l in transform.GetComponentsInChildren<MoveenSkelRotElbow>()) {
                l.r1 = l.r1 * newScale / scaler.lastScale;
                l.r2 = l.r2 * newScale / scaler.lastScale;
                l.needsUpdate = true;
            }

            foreach (var l in transform.GetComponentsInChildren<MoveenSkelRotHydraulic>()) {
                l.r1 = l.r1 * newScale / scaler.lastScale;
                l.r2 = l.r2 * newScale / scaler.lastScale;
                l.needsUpdate = true;
            }

            foreach (var l in transform.GetComponentsInChildren<MoveenSkelBase>()) {
                l.maxLen = l.maxLen * newScale / scaler.lastScale;
                l.needsUpdate = true;
            }

            foreach (var l in transform.GetComponentsInChildren<MoveenSkelBezier>()) {
                l.radius1 = l.radius1 * newScale / scaler.lastScale;
                l.radius2 = l.radius2 * newScale / scaler.lastScale;
                l.needsUpdate = true;
            }

            foreach (var l in transform.GetComponentsInChildren<MoveenSkelBezierMesh>()) {
                l.finishRadius = l.finishRadius * newScale / scaler.lastScale;
                l.radius = l.radius * newScale / scaler.lastScale;
                l.needsUpdate = true;
            }

            scaler.lastScale = newScale;
        }
    }
}
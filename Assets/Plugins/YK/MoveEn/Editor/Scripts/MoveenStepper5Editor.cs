using System;
using System.Collections.Generic;
using moveen.core;
using moveen.descs;
using moveen.utils;
using UnityEditor;
using UnityEngine;

namespace moveen.editor {
    [CustomEditor(typeof(MoveenStepper5))]
    public class MoveenStepper5Editor : Editor {
        private MoveenStepper5 component;

        public void OnEnable() {
            component = (MoveenStepper5) target;
        }

        public override void OnInspectorGUI() {
            int legsCount = component.calcActualLegsCount();

            if (component.engine.collectSteppingHistory) EditorGUILayout.HelpBox("Don't forget to turn off \"collectSteppingHistory\" as it eats RUNTIME performance and memory", MessageType.Warning);

            if (component.body == null && component.GetComponent<Rigidbody>() == null) {
                EditorGUILayout.HelpBox("Add Rigidbody component, or connect this->Body with it", MessageType.Warning);
            }
            if (component.body != null && component.body.GetComponent<Rigidbody>() == null) {
                EditorGUILayout.HelpBox("this->Body is connected, but it has no RigidBody component", MessageType.Warning);
            }
            if (component.body != null && component.GetComponent<Rigidbody>() != null && component.body.GetComponent<Rigidbody>()) {
                EditorGUILayout.HelpBox("Two RigidBody is connected (in this-> and in this->Body->). There can be only one", MessageType.Warning);
            }

            if (legsCount < 2) {
                EditorGUILayout.HelpBox("Add legs! Leg is described by GameObjects with LegSkel component on it", MessageType.Warning);
            } else {
                if (component.engine.leadingLegLeft < 0 || component.engine.leadingLegLeft >= legsCount) {
                    EditorGUILayout.HelpBox("Wrong index for Engine -> Leading Leg Left", MessageType.Warning);
                }
                if (component.engine.leadingLegRight < 0 || component.engine.leadingLegRight >= legsCount) {
                    EditorGUILayout.HelpBox("Wrong index for Engine -> Leading Leg Right", MessageType.Warning);
                }
            }

            EditorGUILayout.HelpBox("Legs count: " + legsCount, MessageType.None);

            if (component.transform.parent == null) {
                EditorGUILayout.HelpBox("Place this game object as a child of another", MessageType.Warning);
            } else {
                if (GUILayout.Button("Update ProtoModel")) updateProtoModel(component.transform);
                if (GUILayout.Button("Fill ProtoModel")) fillProtoModel(component.transform);
            }

//            if (GUILayout.Button("Create from skeleton")) {
//                createDebugGeometry(t);
//                for (int i = 0; i < t.childCount; i++) {
//                    MoveenLegBase structureDesc = t.GetChild(i).GetComponent<MoveenLegBase>();
//                    if (structureDesc != null) {
//                        for (int b = 0; b < structureDesc.bonesDeltaPos.Count; b++) {
//                            structureDesc.bonesDeltaPos[b] = new Vector3(structureDesc.legSkel.bones[b].r / 2, 0, 0);
//                            structureDesc.bonesDeltaRot[b] = Quaternion.identity;
//                        }
//                        structureDesc.updateData(); //to copy new deltas 
//                    }
//                }
//
////            component.copyTransforms();//чтобы пересчитать transform
//            }

            if (GUILayout.Button("Reread layout")) component.needsUpdate = true;
            DrawDefaultInspector();

            MoveenStepper5.showInstrumentalInfo = EditorGUILayout.Foldout(MoveenStepper5.showInstrumentalInfo, "instrumental info", true);
            if (MoveenStepper5.showInstrumentalInfo) {
                int maxPerformance = 0;
                int performance = 0;
                addPerformance(ref performance, ref maxPerformance, 50, component.hip != null, "hip connected");
                addPerformance(ref performance, ref maxPerformance, 5 * legsCount, true, legsCount + " legs");
                addPerformance(ref performance, ref maxPerformance, 20, component.engine.forceBipedalEarlyStep, "forceBipedalEaryStep");
                addPerformance(ref performance, ref maxPerformance, 10, component.engine.bipedalForbidPlacement, "bipedalForbidPlacement");
                addPerformance(ref performance, ref maxPerformance, 5, component.engine.protectBodyFromFallthrough, "protectBodyFromFallthrough");
                addPerformance(ref performance, ref maxPerformance, 10, component.engine.bodyLenHelp != 0f, "bodyLenHelp");
//                addPerformance(ref performance, ref maxPerformance, 2, !component.engine.legsCanPull, "legsCanPull");
                
                //TODO measure numbers
                //TODO...
                
                EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(), (float)performance / maxPerformance, "CPU use " + performance + " / " + maxPerformance);
                EditorGUILayout.HelpBox("CPU use: " + performance + " / " + maxPerformance + " (lower number - fewer features, more performance)", MessageType.Info);
                MoveenStep2Editor.drawInstrumentalProperties(component.engine);
            }

        }

        private static void addPerformance(ref int current, ref int max, int value, bool add, string name) {
            EditorGUILayout.LabelField(name + " (" + value + ") : " + (add ? "on" : "off"), add ? EditorStyles.boldLabel : EditorStyles.label);
            max += value;
            if (add) current += value;
        }

        public void OnSceneGUI() {
            //if (component.body == null) return;

            if (component.hip != null) {
                Handles.Label(component.engine.hipPosRel + component.transform.position, "hip disp");
                component.engine.hipPosRel = Handles.PositionHandle(component.engine.hipPosRel + component.transform.position, Quaternion.identity) -
                                             component.transform.position;
            }
            
            //legend for stepping history
            if (component.engine.collectSteppingHistory) {
                string historyBeanLegend = "";
                foreach (var hb in HistoryGizmo1.historyBeans) historyBeanLegend += "<color=#" + ColorUtility.ToHtmlStringRGB(hb.color) + ">" + hb.name + "</color> ";
                Handles.Label(Camera.current.ScreenToWorldPoint(new Vector3(10, Camera.current.pixelHeight - 10, 10)), historyBeanLegend, new GUIStyle {richText = true});
            }
        }

        private const string modelName = "ProtoModel";

        public static void fillProtoModel(Transform animRoot) {
            int undoGroup = Undo.GetCurrentGroup();
            Undo.IncrementCurrentGroup();
            Undo.RegisterFullObjectHierarchyUndo(animRoot, modelName);

            Transform protoModel = findProtoModel(animRoot);

            //update body copier if there is one
            MoveenTransformCopier copier = animRoot.GetComponent<MoveenTransformCopier>();
            if (copier != null) copier.target = protoModel;

            updateProtoModel(animRoot, protoModel, (b, t) => {
                //bone cube for each bone
                t.gameObject.removeAllChildrenImmediate(true);
                
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Undo.RegisterCreatedObjectUndo(cube, modelName);
                cube.transform.name = modelName + "-bone-cube";
                DestroyImmediate(cube.GetComponent<BoxCollider>());
                Undo.SetTransformParent(cube.transform, t, "filling " + modelName);
                float boneR2 = b.r;
                cube.transform.localScale = new Vector3(boneR2, boneR2 * 0.1f, boneR2 * 0.1f);
                //cube.transform.SetPositionAndRotation(b.origin.getPos() + b.origin.getRot().rotate(new Vector3(boneR2 / 2, 0, 0)), b.origin.getRot());
                cube.transform.localPosition = new Vector3(boneR2 / 2, 0, 0);
                cube.transform.localRotation = Quaternion.identity;
            });
            
            {//body cube
                Transform bodyForGeo = protoModel.GetChild(0);
                bodyForGeo.gameObject.removeAllChildrenImmediate(true);
                
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Undo.RegisterCreatedObjectUndo(cube, modelName);
                cube.transform.name = modelName + "-body";
                DestroyImmediate(cube.GetComponent<BoxCollider>());
                Undo.SetTransformParent(cube.transform, bodyForGeo, "filling " + modelName);
                float boneR2 = 0.3f;
                cube.transform.localScale = new Vector3(boneR2, boneR2, boneR2);
                cube.transform.localPosition = Vector3.zero;
                cube.transform.localRotation = Quaternion.identity;
            }


            Undo.CollapseUndoOperations(undoGroup);
        }

        public static void updateProtoModel(Transform root) {
            int undoGroup = Undo.GetCurrentGroup();
            Undo.IncrementCurrentGroup();
            
            Undo.RegisterFullObjectHierarchyUndo(root, modelName);
            
            updateProtoModel(root, findProtoModel(root), (b, t) => {});
            
            Undo.CollapseUndoOperations(undoGroup);
        }

//        private static GameObject newObject() {
//            GameObject result = new GameObject();
//            Undo.RegisterCreatedObjectUndo(result, modelName);
//            return result;
//        }

        private static GameObject newObject(Transform target) {
            GameObject result = new GameObject();
            result.transform.SetPositionAndRotation(target.position, target.rotation);
            Undo.RegisterCreatedObjectUndo(result, modelName);
            Undo.SetTransformParent(result.transform, target, modelName);
            return result;
        }
        public static void updateProtoModel(Transform skelParent, Transform modelParent, Action<Bone, Transform> perBone) {
            if (modelParent.childCount < 1) newObject(modelParent);
            Transform forBones = modelParent.GetChild(0);
            forBones.name = modelName + "_forBones";
            MoveenSkelWithBones[] boness = skelParent.GetComponents<MoveenSkelWithBones>();
            int curBoneGeo = 0;
            for (int boneComponentIndex = 0; boneComponentIndex < boness.Length; boneComponentIndex++) {
                MoveenSkelWithBones bones = boness[boneComponentIndex];

                for (int boneIndex = 0; boneIndex < bones.bones.Count; boneIndex++) {
                    if (forBones.childCount <= curBoneGeo) newObject(forBones);
                    Transform boneGeo = forBones.GetChild(curBoneGeo);
                    boneGeo.name = modelName + "_bone_" + boneIndex;
                    curBoneGeo++;

                    Bone bone = bones.bones[boneIndex];
                    boneGeo.SetPositionAndRotation(bone.origin.getPos(), bone.origin.getRot());
                    if (bone.attachedGeometry == null || bone.attachedGeometry.name.StartsWith(modelName)) {
                        bones.bonesGeometry[boneIndex] = boneGeo;
                        bone.attachedGeometry = boneGeo;
                        perBone(bone, boneGeo);
                    }
                }
            }

            for (int i = 0; i < skelParent.childCount; i++) {
                Transform skelChild = skelParent.GetChild(i);
                if (modelParent.childCount <= i + 1) newObject(modelParent);
                Transform modelChild = modelParent.GetChild(i + 1);
                modelChild.name = modelName +"_" + skelChild.name;

                if (hierarchyContainsBones(skelChild)) {
                    updateProtoModel(skelChild, modelChild, perBone);
                }
            }
        }

        private static Transform findProtoModel(Transform root) {
            Transform protoModel = null;                 
            for (int i = 0; i < root.parent.childCount; i++) {
                Transform child = root.parent.GetChild(i);
                if (child.name.Equals(modelName)) {
                    protoModel = child;
                    break;
                }
            }
            if (protoModel == null) {
                protoModel = newObject(root.parent).transform;
                protoModel.name = modelName;
            }
            return protoModel;
        }

        public static bool hierarchyContainsBones(Transform t) {
            if (t.GetComponent<MoveenSkelWithBones>() != null) return true;
            for (int i = 0; i < t.childCount; i++) {
                if (hierarchyContainsBones(t.GetChild(i))) return true;
            }
            return false;
        }

        [DrawGizmo(GizmoType.NonSelected | GizmoType.Active)]
        private static void OnDrawGizmos(MoveenStepper5 component, GizmoType gizmoType) {
            if (!component.isActiveAndEnabled) return;
            Vector3 distSum = new Vector3();
            
            for (var i = 0; i < component.engine.steps.Count; i++) {
                var step = component.engine.steps[i];

                //step dial
                float r1 = 10;
                float r2 = 20;

                float airTime = step.lastStepLen / step.legSpeed * 1.5f;
                float earthTime = step.lastStepLen / step.bodySpeed.length();
                float dockedAngle = MyMath.PI * 2 * earthTime / (airTime + earthTime);
//                float angle = MyMath.PI;

                if (component.engine.showPhaseDials) {
                    Vector2 dialCenter = new Vector2(r2 * 2 + i * (r2 * 2.5f), r2 * 2);
                    Gizmos.color = Color.green;
                    UnityEditorUtils.fillStripe2D(dialCenter, r1, r2, 20);
                    Gizmos.color = Color.red;
                    UnityEditorUtils.fillArcStripe2D(dialCenter, r1, r2, 20, MyMath.PI * 2 * 3 / 4 - dockedAngle / 2, MyMath.PI * 2 * 3 / 4 + dockedAngle / 2);

                    float a2;
                    if (step.dockedState) {
                        a2 = MyMath.PI * 2 * 3 / 4 - dockedAngle / 2 + dockedAngle * step.progress;
                    } else {
                        a2 = MyMath.PI * 2 * 3 / 4 + dockedAngle / 2 + (MyMath.PI * 2 - dockedAngle) * step.progress;

                    }
                    Gizmos.color = new Color(0.8f, 0.8f, 0.8f, 1);
                    UnityEditorUtils.fillArcStripe2D(dialCenter, r2 - 5, r2 + 2, 5, a2, a2 + step.beFaster);
                    Gizmos.color = step.dockedState ? Color.white : Color.black;
                    HistoryGizmo1.drawNeedle(a2, dialCenter, r1, r2);
                }

                Gizmos.color = Color.green;

                if (component.engine.collectSteppingHistory && Application.isPlaying) {
                    CounterStacksCollection history = step.paramHistory;
                    HistoryGizmo1.drawHistory(history, HistoryGizmo1.historyBeans, i);
                }
            }
            distSum = MyMath.max(distSum, new Vector3(0.5f, 0.5f, 0.5f));
            Gizmos.color = Color.yellow;
            Gizmos.matrix = Matrix4x4.TRS(component.targetPos, component.targetRot, new Vector3(1, 1, 1));
            Gizmos.DrawWireCube(Vector3.zero, distSum * 0.2f);
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = Color.green;

            if (component.hip != null) {
                Gizmos.matrix = Matrix4x4.TRS(component.engine.hipPosAbs, component.engine.hipRotAbs, new Vector3(1, 1, 1));
                Gizmos.DrawWireCube(Vector3.zero, distSum / 2);
                Gizmos.matrix = Matrix4x4.identity;
            }

            Gizmos.matrix = Matrix4x4.TRS(component.transform.position, component.transform.rotation, new Vector3(1, 1, 1));
            UnityEditorUtils.arrow();
            Gizmos.DrawWireCube(Vector3.zero, distSum);
            Gizmos.matrix = Matrix4x4.identity;
            
            if (Application.isPlaying) {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(component.engine.imBody, 0.2f);
                Gizmos.color = new Color(0, 0, 1);
                Gizmos.DrawWireSphere(component.engine.imCenter, 0.4f);
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(component.engine.virtualForLegs, 0.35f);
            }

            Gizmos.color = new Color(1, 0.0f, 1);
            Gizmos.DrawWireSphere(component.engine.calculatedCOG, 0.1f);
        }
    }
}
using System.Collections.Generic;
using moveen.descs;
using UnityEditor;
using UnityEngine;

namespace moveen.editor {
    [CustomEditor(typeof(MoveenRagdoll1))]
    public class MoveenRagdoll1Editor : Editor {
        public MoveenRagdoll1 component;

        public void OnEnable() {
            component = (MoveenRagdoll1) target;
        }

        public override void OnInspectorGUI() {
            DrawDefaultInspector();
            if (component.stepper == null) {
                EditorGUILayout.HelpBox("Add MoveenStepper5 component", MessageType.Error);
                return;
            }
            GUI.enabled = !Application.isPlaying;
            if (GUILayout.Button("Create Ragdoll")) createRagdoll();
            GUI.enabled = Application.isPlaying && !component.isRagdoll;
            if (GUILayout.Button("Go Ragdoll")) component.startRagdoll();
            GUI.enabled = true;
        }
        
        public void createRagdoll() {
            foreach (var l in component.stepper.GetComponentsInChildren<MoveenSkelWithBones>()) {
                l.createRagdoll();
            }
            Transform top = null;
            if (component.stepper.body != null) {
                top = component.stepper.body;
                component.usedByStepper = top.GetComponent<Rigidbody>();
            }
            if (top == null) {
                MoveenTransformCopier copier = component.stepper.GetComponent<MoveenTransformCopier>();
                if (copier != null) top = copier.target;
                //don't need component.toConnect in case of copier
            }
            if (top == null) {
                top = component.transform; //in case everything is in moveen hierarchy
                component.usedByStepper = top.GetComponent<Rigidbody>();
            }
            MoveenSkelWithBones.createCapsule2(top, 0.3f, 0.3f);//TODO better r

            Transform hipT = null;
            component.jointsToConnect = new List<Joint>();
            if (component.stepper.hip != null) {
                MoveenTransformCopier connector = component.stepper.hip.GetComponent<MoveenTransformCopier>();
                if (connector != null) hipT = connector.target;
                if (hipT == null) hipT = component.stepper.hip;
                MoveenSkelWithBones.createCapsule(hipT, Quaternion.identity, 1);//TODO better r
                CharacterJoint joint = MoveenSkelWithBones.connectToParentCharacterJoint(hipT, top);
                top = hipT;
                if (component.usedByStepper != null && joint != null) component.jointsToConnect.Add(joint);
            }
            
            MoveenSkelWithBones[] steps = component.stepper.GetComponentsInChildren<MoveenSkelWithBones>();
            foreach (var limb in steps) {
                Transform affected = MoveenSkelWithBones.getAffected(limb, 0);
                if (affected != null) {
                    CharacterJoint joint = MoveenSkelWithBones.connectToParentCharacterJoint(affected, top);
                    if (component.usedByStepper != null && joint != null && hipT == null) component.jointsToConnect.Add(joint);
                }
            }

        }


    }
}
using System;
using System.Collections.Generic;
using moveen.core;
using moveen.utils;
using UnityEngine;

namespace moveen.descs {
    [ExecuteInEditMode] //to init structure properly
    public abstract class MoveenSkelWithBones : MoveenSkelBase {
        [HideInInspector]public List<Vector3> wanteds;//TODO make null when not used

        [Range(0, 1)] public float transition;

        public List<Transform> bonesGeometry; //TODO call MUtil2.al<Transform>(null) in children only for first time
        [NonSerialized] public bool isInError;
        [NonSerialized] public List<Bone> bones = MUtil2.al<Bone>();

        [HideInInspector] public List<Vector3> bonesDeltaPos = new List<Vector3>();
        [HideInInspector] public List<Quaternion> bonesDeltaRot = new List<Quaternion>();

        [HideInInspector] public Vector3 basePos;
        [HideInInspector] public Quaternion baseRot;

        protected MoveenSkelWithBones() {
            executionOrder = 20;
        }
        
        public override void tick(float dt) {
            isInError = false;

            basePos = transform.position;
            baseRot = transform.rotation;

            base.tick(dt);
            tickStructure(dt);
            for (int i = 0; i < bones.Count; i++) bones[i].origin.tick();
            
            //update/copy connected bones (if any) 
            if (Application.isPlaying) {
                copyTransforms();
            } else {
                updateDeltas();
            }
            
            //update child bones (if any) 
            copyChildrenTransformsDeltless();
        }

        public virtual void tickStructure(float dt) {
        }

        //actual params copy, after deserialization included
        public override void updateData() {
            base.updateData();
            updateBones();
//            updateChildren();
        }

        private void updateBones() {
            MUtil.madeCount(bonesDeltaPos, bonesGeometry.Count);
            MUtil.madeCount(bonesDeltaRot, bonesGeometry.Count);
            for (int index = 0; index < bonesGeometry.Count; index++) {
                Bone bone = bones[index];
                bone.attachedGeometry = bonesGeometry[index];
                bone.deltaPos = bonesDeltaPos[index];
                bone.deltaRot = bonesDeltaRot[index];
            }
        }

        private void updateDeltas() {
            for (int index = 0; index < bonesGeometry.Count; index++) {
                Transform geometry = bonesGeometry[index];
                if (geometry != null) {
                    bones[index].origin.tick();
                    Quaternion geomR = geometry.rotation;
                    Quaternion skelR = bones[index].origin.getRot();

                    bonesDeltaPos[index] = skelR.conjug().rotate(geometry.position - bones[index].origin.getPos());
                    bonesDeltaRot[index] = geomR.rotSub(skelR);
                }
            }
        }

        [NonSerialized] private bool canExpectExternalChange;

        private void copyTransforms() {
            for (int i = 0; i < bones.Count; i++) {
                Bone bone = bones[i];
                if (transition == 1) {
                } else if (transition == 0) {
                    bone.copy();
                } else {
                    bone.copy(bone.attachedGeometry, transition);
                }
            }
        }

        //update child bones (if any) 
        private void copyChildrenTransformsDeltless() {
            for (int i = 0; i < bones.Count; i++) {
                if (transform.childCount <= i) break;
                Transform child = transform.GetChild(i);
                Bone bone = bones[i];
                child.SetPositionAndRotation(bone.origin.getPos(), bone.origin.getRot());
            }
        }

        public virtual void createRagdoll() {
            for (int i = 0; i < bones.Count; i++) {
                Transform affected = getAffected(this, i);
                if (affected != null && bones[i].r != 0) createCapsule(affected, bones[i].deltaRot, bones[i].r);
            }
            for (int i = 0; i < bones.Count - 1; i++) {
                Transform p0 = getAffected(this, i);
                Transform p1 = getAffected(this, i + 1);
                if (p0 != null && p1 != null && p0.GetComponent<Rigidbody>() != null && p1.GetComponent<Rigidbody>() != null) {
                    connectToParentHingeJoint(p1, p0, new Vector3(0, 0, 1));
                }
            }
        }

        public static Transform getAffected(MoveenSkelWithBones bones, int i) {
            if (bones.bones[i].attachedGeometry != null) return bones.bones[i].attachedGeometry;
            if (bones.transform.childCount > i) return bones.transform.GetChild(i);
            return null;
        }

        public static CharacterJoint connectToParentCharacterJoint(Transform a, Transform p) {
            if (a == null || p == null) return null;
            CharacterJoint[] joints = a.gameObject.GetComponents<CharacterJoint>();
            for (int i = 1; i < joints.Length; i++) {
                DestroyImmediate(joints[i]);
            }
            CharacterJoint joint = a.gameObject.GetComponent<CharacterJoint>();
            joint = joint != null ? joint : a.gameObject.AddComponent<CharacterJoint>();
            joint.connectedBody = p.GetComponent<Rigidbody>();
            return joint;
        }

        public static void connectToParentHingeJoint(Transform a, Transform p, Vector3 jointAxis) {
            if (a == null || p == null) return;
            Joint[] joints = a.gameObject.GetComponents<Joint>();
            for (int i = 0; i < joints.Length; i++) {
                DestroyImmediate(joints[i]);
            }
            HingeJoint joint = a.gameObject.AddComponent<HingeJoint>();
            joint.useLimits = true;
            joint.limits = new JointLimits {min = -45, max = 45};
            joint.axis = jointAxis;
            joint.connectedBody = p.GetComponent<Rigidbody>();
        }

        public static T ensureOneComponent<T>(Transform t) where T : Component {
            T[] cc = t.GetComponents<T>();
            for (int i = 1; i < cc.Length; i++) DestroyImmediate(cc[i]);
            return cc.Length > 0 ? cc[0] : t.gameObject.AddComponent<T>();
        }

        public static void createCapsule(Transform target, Quaternion rot, float height) {
            if (target == null) return;
            CapsuleCollider capsuleCollider = ensureOneComponent<CapsuleCollider>(target);
            ensureOneComponent<Rigidbody>(target);

            Vector3 vec = rot.conjug().rotate(new Vector3(1, 0, 0));
            //the same effect:
//            Vector3 vec = bone.origin.getRot().rotate(new Vector3(1, 0, 0));
//            vec = bone.attachedGeometry.rotation.conjug().rotate(vec);
//            Vector3 vec = (bone.attachedGeometry.rotation.conjug() * bone.origin.getRot()).rotate(new Vector3(1, 0, 0));
//            Vector3 vec = bone.origin.getRot().rotSub(bone.attachedGeometry.rotation).rotate(new Vector3(1, 0, 0));

            Vector3 center = new Vector3();
            if (Math.Abs(vec.x) > Math.Abs(vec.y) && Math.Abs(vec.x) > Math.Abs(vec.z)) {
                capsuleCollider.direction = 0;
                center = new Vector3(vec.x > 0 ? 1 : -1, 0, 0);
            }
            if (Math.Abs(vec.y) > Math.Abs(vec.x) && Math.Abs(vec.y) > Math.Abs(vec.z)) {
                capsuleCollider.direction = 1;
                center = new Vector3(0, vec.y > 0 ? 1 : -1, 0);
            }
            if (Math.Abs(vec.z) > Math.Abs(vec.x) && Math.Abs(vec.z) > Math.Abs(vec.y)) {
                capsuleCollider.direction = 2;
                center = new Vector3(0, vec.z > 0 ? 1 : -1);
            }

            if (height == 0) height = 1;
            capsuleCollider.height = height;
            capsuleCollider.radius = height * 0.2f;
            capsuleCollider.center = center * height / 2;
        }

        public static void createCapsule2(Transform target, float height, float radius) {
            if (target == null) return;
            CapsuleCollider capsuleCollider = ensureOneComponent<CapsuleCollider>(target);
            ensureOneComponent<Rigidbody>(target);
            capsuleCollider.height = height;
            capsuleCollider.radius = radius;
        }

        //This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
        public override void OnValidate() {
            MUtil.logEvent(this, "OnValidate");
            base.OnValidate();
            int targetBonesCount = bones.Count;
            if (bonesGeometry.Count != targetBonesCount) {
                Debug.LogWarning("Don't change array size!");
                if (bonesGeometry.Count > targetBonesCount) {
                    bonesGeometry.RemoveRange(targetBonesCount, bonesGeometry.Count - targetBonesCount);
                }
                while (bonesGeometry.Count < targetBonesCount) {
                    bonesGeometry.Add(null);
                }
            }
            needsUpdate = true;
        }

        public override void reset() {
            MUtil.logEvent(this, "reset");
            base.reset();
            tick(1);
        }

        public virtual bool canBeSolved() {
            return false;
        }
    }
}
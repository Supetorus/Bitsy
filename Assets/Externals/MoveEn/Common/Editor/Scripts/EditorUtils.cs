using moveen.core;
using moveen.descs;
using moveen.example;
using moveen.utils;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace moveen.editor {
    public static class EditorUtils {
        [MenuItem("GameObject/Moveen biped", false, 11)]
        public static void AddNewBiped() {
            GameObject root = createCommon("Moveen biped");
            root.GetComponent<MoveenStepper5>().engine.forceBipedalEarlyStep = true;

            float w = 0.3f;
            //legs
            {
                GameObject leg = new GameObject("Anim leg R");
                leg.transform.parent = root.transform;
                leg.transform.localPosition = new Vector3(0, 0, -w);
                MoveenStep2 step = leg.AddComponent<MoveenStep2>();
                MoveenSkelLimb2 skel = leg.AddComponent<MoveenSkelLimb2>();

                step.step.comfortPosRel = new Vector3(0, -1, -w);
                skel.targetPosRel = new Vector3(0, -1, -w);

                skel.rotJoint.axisRel = new Vector3(1, 0, 0);
                skel.rotJoint.secondaryAxisRel = new Vector3(0, -1, 0);
                skel.rA = 0.5f;
                skel.rB = 0.5f;
                skel.rC = 0.5f;

                skel.styleRatio = 0.4f;
                skel.footPlatformHeight = 0.1f;
                step.step.undockInitialStrength = 2f;
                step.step.undockTime = 0.2f;
            }

            {
                GameObject leg = new GameObject("Anim leg L");
                leg.transform.parent = root.transform;
                leg.transform.localPosition = new Vector3(0, 0, w);

                MoveenStep2 step = leg.AddComponent<MoveenStep2>();
                MoveenSkelLimb2 skel = leg.AddComponent<MoveenSkelLimb2>();

                step.step.comfortPosRel = new Vector3(0, -1, w);
                skel.targetPosRel = new Vector3(0, -1, w);

                skel.rotJoint.axisRel = new Vector3(1, 0, 0);
                skel.rotJoint.secondaryAxisRel = new Vector3(0, -1, 0);
                skel.rA = 0.5f;
                skel.rB = 0.5f;
                skel.rC = 0.5f;

                skel.styleRatio = 0.4f;
                skel.footPlatformHeight = 0.1f;
                step.step.undockInitialStrength = 2f;
                step.step.undockTime = 0.2f;
            }
        }

        [MenuItem("GameObject/Moveen quadruped", false, 11)]
        public static void AddNewQuadruped() {
            GameObject root = createCommon("Moveen quadruped");
            Stepper5 stepper5 = root.GetComponent<MoveenStepper5>().engine;
            stepper5.leadingLegRight = 0;
            stepper5.leadingLegLeft = 2;
            stepper5.forceBipedalEarlyStep = false;
//            root.transform.position = root.transform.position.withSetY(0.6f);

            float w = 0.3f;
            float v = 0.8f;
            float lh = -0.6f;
            //legs
            {
                GameObject leg = new GameObject("Anim leg FR");
                leg.transform.parent = root.transform;
                leg.transform.localPosition = new Vector3(w, lh, -w);
                MoveenStep2 step = leg.AddComponent<MoveenStep2>();
                MoveenSkelLimb1 skel = leg.AddComponent<MoveenSkelLimb1>();
                step.step.comfortPosRel = new Vector3(v, -1, -v);
                skel.targetPosRel = step.step.comfortPosRel;
                skel.rotJoint.axisRel = new Vector3(0, 1, 0);
                skel.rotJoint.secondaryAxisRel = new Vector3(1, 0, -1).normalized;
                skel.footPlatformHeight = 0.1f;
            }

            {
                GameObject leg = new GameObject("Anim leg BL");
                leg.transform.parent = root.transform;
                leg.transform.localPosition = new Vector3(-w, lh, w);
                MoveenStep2 step = leg.AddComponent<MoveenStep2>();
                MoveenSkelLimb1 skel = leg.AddComponent<MoveenSkelLimb1>();
                step.step.comfortPosRel = new Vector3(-v, -1, v);
                skel.targetPosRel = step.step.comfortPosRel;
                skel.rotJoint.axisRel = new Vector3(0, 1, 0);
                skel.rotJoint.secondaryAxisRel = new Vector3(-1, 0, 1).normalized;
                skel.footPlatformHeight = 0.1f;
            }

            {
                GameObject leg = new GameObject("Anim leg FL");
                leg.transform.parent = root.transform;
                leg.transform.localPosition = new Vector3(w, lh, w);
                MoveenStep2 step = leg.AddComponent<MoveenStep2>();
                MoveenSkelLimb1 skel = leg.AddComponent<MoveenSkelLimb1>();
                step.step.comfortPosRel = new Vector3(v, -1, v);
                skel.targetPosRel = step.step.comfortPosRel;
                skel.rotJoint.axisRel = new Vector3(0, 1, 0);
                skel.rotJoint.secondaryAxisRel = new Vector3(1, 0, 1).normalized;
                skel.footPlatformHeight = 0.1f;
            }

            {
                GameObject leg = new GameObject("Anim leg BR");
                leg.transform.parent = root.transform;
                leg.transform.localPosition = new Vector3(-w, lh, -w);
                MoveenStep2 step = leg.AddComponent<MoveenStep2>();
                MoveenSkelLimb1 skel = leg.AddComponent<MoveenSkelLimb1>();
                step.step.comfortPosRel = new Vector3(-v, -1, -v);
                skel.targetPosRel = step.step.comfortPosRel;
                skel.rotJoint.axisRel = new Vector3(0, 1, 0);
                skel.rotJoint.secondaryAxisRel = new Vector3(-1, 0, -1).normalized;
                skel.footPlatformHeight = 0.1f;
            }

            foreach (var step in root.GetComponentsInChildren<MoveenStep2>()) {
                step.step.stepSpeedMin = 6f;
                step.step.stepSpeedMin = 6f;
                step.step.comfortRadiusRatio = 0.7f;
            }
        }


        private static Vector3 selectPosition() {
            Camera current = Camera.current;
            if (current == null) {
                SceneView view = SceneView.currentDrawingSceneView;
                if (view != null) current = view.camera;
            }
            if (current == null) {
                SceneView view = SceneView.lastActiveSceneView;
                if (view != null) current = view.camera;
            }

            Vector3 pos = new Vector3();
            if (current != null) {
                pos = current.transform.TransformPoint(Vector3.forward * 5);
            } else {
                Debug.Log("not found camera to place object in front of, placing it at 0, 0, 0");
            }
            return pos;
        }

        private static GameObject createCommon(string name) {
            Vector3 pos = selectPosition();

            GameObject top = new GameObject(name);
            top.transform.position = pos;

            GameObject root = new GameObject("Anim root");
            root.transform.parent = top.transform;
            root.transform.position = pos;
//            root.transform.localPosition = new Vector3();
            MoveenStepper5 stepper = root.AddComponent<MoveenStepper5>();
            stepper.participateInFixedTick = true;
            stepper.participateInTick = true;
            root.AddComponent<MoveenSurfaceDetector1>();
            root.AddComponent<Rigidbody>();
            root.AddComponent<MoveenTransformCopier>();

            GameObject target = new GameObject("Target");
            target.transform.parent = top.transform;
            target.transform.position = pos;
            MoveenBridle1 bridle = target.AddComponent<MoveenBridle1>();
            bridle.maxSpeed = 2;
            bridle.moveenStepper5 = stepper;
            return root;
        }
    }
}
#endif
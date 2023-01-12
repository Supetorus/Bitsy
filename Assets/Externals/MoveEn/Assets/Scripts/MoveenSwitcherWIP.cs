using moveen.utils;
using UnityEngine;

namespace moveen.descs {
    public class MoveenSwitcherWIP : MonoBehaviour {
        public MoveenMix animationToRagdoll;
        public MoveenMix allToStepper;
        public MoveenRagdollSwitch ragdoller;
        public MoveenStepper5 stepper;
        public Animator animator;
        public Transform geometry;

        public Vector3 animationDisplacement;
        public static int Idle_simple = Animator.StringToHash("Idle_simple");
        public static int Simple_jump = Animator.StringToHash("Simple_jump");
        public static int Revive = Animator.StringToHash("Revive");

        private void Update() {
            int currentStateHash = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
            
            //react on keys
            if (Input.GetKeyDown(KeyCode.E)) {
                if (stepperIsTurnedOn()) die();
                else revive();
            }
            if (Input.GetKeyDown(KeyCode.Q)) {
                if (animator.enabled && currentStateHash == Idle_simple) beginSimpleJump();
            }

            //copy root position stepper <-> animation
            if (animator.enabled) {
                geometry.transform.position = stepper.transform.position + animationDisplacement;
                Quaternion res = stepper.transform.rotation * Quaternion.AngleAxis(90, new Vector3(0, 1, 0));
                geometry.transform.rotation = Quaternion.AngleAxis(res.eulerAngles.y, new Vector3(0, 1, 0));
            } else {
                stepper.transform.position = geometry.transform.position - animationDisplacement;
                stepper.transform.rotation = geometry.transform.rotation * Quaternion.AngleAxis(-90, new Vector3(0, 1, 0));
            }

            //update progresses
            float progress = MyMath.clamp(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 0, 1);
            if (currentStateHash == Revive) {
                allToStepper.progress = progress;
                animationToRagdoll.progress = 1 - progress;
            }
            if (currentStateHash == Simple_jump) {
                float max = 3;
                float a = 1;
                float b = 2;
                float prog = MyMath.clamp(progress, 0, 1);
                prog *= max;
                if (prog < a) allToStepper.progress = MyMath.regionMap(prog, 0, a, 1, 0);
                else if (prog < b) allToStepper.progress = 0;
                else if (prog < max) allToStepper.progress = MyMath.regionMap(prog, b, max, 0, 1);
            }

            //default
            if (reviving > 0) {//without it, animator resets to default right after reviving
                reviving -= Time.deltaTime;
            } else if (animator.enabled && currentStateHash == Idle_simple && !animator.IsInTransition(0)) {
                ragdoller.enabled = false;
//                animationToRagdoll.enabled = false;
                allToStepper.progress = 1;
            }
        }

        public float reviving;
        public void revive() {
            reviving = 0.5f;
            ragdoller.stopRagdoll();
            turnOnStepper();

            animator.enabled = true;
            animator.SetTrigger("Revive");
            animationToRagdoll.enabled = true;
            animationToRagdoll.progress = 1;

            allToStepper.enabled = true;
            allToStepper.progress = 0;
        }

        public void die() {
            Rigidbody stepperRb = stepper.GetComponent<Rigidbody>();
            Rigidbody ragdollRb = stepper.GetComponent<MoveenSkelWithBones>().bones[0].attachedGeometry.GetComponent<Rigidbody>();
            ragdollRb.velocity = stepperRb.velocity * 1.5f;
            ragdollRb.angularVelocity = stepperRb.angularVelocity * 1.5f;

            ragdoller.enabled = true;
            ragdoller.startRagdoll();
            turnOffStepper();

            animator.enabled = false;
            animator.SetBool("Dead", true);
            animationToRagdoll.enabled = false;
            allToStepper.enabled = false;
        }

        public void beginSimpleJump() {
            animator.SetTrigger("Jump2");
            allToStepper.enabled = true;
            allToStepper.progress = 1;
            ragdoller.enabled = false;
        }

        private MoveenSkelWithBones[] cache1;
        private MoveenTransformCopier[] cache2;

        private void Awake() {
            cache1 = stepper.gameObject.GetComponentsInChildren<MoveenSkelWithBones>();
            cache2 = stepper.gameObject.GetComponentsInChildren<MoveenTransformCopier>();
        }

        public void turnOnStepper() {
            for (int i = 0; i < cache1.Length; i++) cache1[i].transition = 0;
            for (int i = 0; i < cache2.Length; i++) cache2[i].transition = 0;
        }

        public bool stepperIsTurnedOn() {
            for (int i = 0; i < cache1.Length; i++) return cache1[i].transition == 0;
            for (int i = 0; i < cache2.Length; i++) return cache2[i].transition == 0;
            return true;
        }

        public void turnOffStepper() {
            for (int i = 0; i < cache1.Length; i++) cache1[i].transition = 1;
            for (int i = 0; i < cache2.Length; i++) cache2[i].transition = 1;
        }

    }
}
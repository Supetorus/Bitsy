using UnityEngine;

namespace moveen.descs {
    public class RagdollStarter : StateMachineBehaviour {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
            animator.gameObject.GetComponent<MoveenRagdollSwitch>().startRagdoll();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            animator.gameObject.GetComponent<MoveenRagdollSwitch>().stopRagdoll();
        }
    }
}
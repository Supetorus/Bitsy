using UnityEngine;

namespace moveen.example {
    public class AnimationTrigger : MonoBehaviour, Startable {

        public Animator animator;
        public string triggerName;

        private void OnEnable() {
            if (animator == null) animator = GetComponent<Animator>();
        }

        public void start() {
            animator.SetTrigger(triggerName);
        }
    }
}
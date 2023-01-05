using moveen.utils;
using UnityEngine;

namespace moveen.example {
    public class ProjectOnCamera : OrderedMonoBehaviour {
        public Transform projectWhom;
        public Transform projectTo;

        public override void OnEnable() {
            base.OnEnable();
            if (projectWhom == null) projectWhom = transform;
        }

        public override void tick(float dt) {
            if (projectWhom == null) return;
            if (projectTo == null) return;

            projectTo.position = Camera.main.WorldToScreenPoint(projectWhom.position);
        }

//        private void LateUpdate() {
//            if (projectWhom == null) return;
//            if (projectTo == null) return;
//
//            projectTo.position = Camera.main.WorldToScreenPoint(projectWhom.position);
//
//        }
    }
}
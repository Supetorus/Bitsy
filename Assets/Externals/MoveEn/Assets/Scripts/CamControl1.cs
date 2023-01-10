using UnityEngine;

namespace moveen.example {
    public class CamControl1 : MonoBehaviour {
        public Camera camera;
        public Transform input;
        private Vector3 dif;

        public void Start() {
            if (camera == null) return;
            if (input == null) return;
            dif = camera.transform.position - input.position;
        }

        public void Update() {
            if (camera == null) return;
            if (input == null) return;
            camera.transform.position = input.transform.position + dif;
        }

    }
}
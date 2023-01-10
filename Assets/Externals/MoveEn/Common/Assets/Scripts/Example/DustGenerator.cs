using moveen.descs;
using moveen.utils;
using UnityEngine;

namespace moveen.example {
    public class DustGenerator : MonoBehaviour, Startable {
        public Dust1 dust;
        
//        private MoveenStep2 step2;
//        [ReadOnly] public bool wasAtGround; 

        private void OnEnable() {
//            step2 = gameObject.GetComponent<MoveenStep2>();
//            if (step2 == null) return;
//            wasAtGround = step2.step.dockedState;
        }

//        private void FixedUpdate() {
//            if (step2 == null || dust == null) return;
//            if (!wasAtGround && step2.step.dockedState) {
//                for (int i = 0; i < 5; i++) {
//                    Vector2 pos = Random.insideUnitCircle;
//                    float h = Random.value * 0.4f;
//                    Vector3 vec = new Vector3(pos.x, h, pos.y);
//                    Dust1 d = Instantiate(dust, step2.step.posAbs + vec, Quaternion.identity);
//                    d.speed = vec * 2 + new Vector3(0, 2, 0);
//                    d.acceleration = new Vector3(0, -5, 0);
//                    d.localTime = 0;
//                    d.ttl = 4;
//                }
//            }
//            wasAtGround = step2.step.dockedState;
//        }

        private Vector3 lastPos;
        private Vector3 speed;
        private void FixedUpdate() {
            Vector3 curPos = transform.position;
            speed = curPos.sub(lastPos).div(Time.deltaTime);
            lastPos = curPos;
        }

        public void start() {
            for (int i = 0; i < 1; i++) {
                Dust1 d = Instantiate(dust, transform.position, transform.rotation);
                d.speed = speed + transform.rotation.rotate(new Vector3(0, 0, 5));
                d.angleSpeed = Random.onUnitSphere * Random.value;
                d.acceleration = new Vector3(0, -10, 0);
                d.localTime = 0;
                d.ttl = 2;
            }
        }
    }
}
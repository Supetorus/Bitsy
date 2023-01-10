using moveen.utils;
using UnityEngine;

namespace moveen.example {
    public class Dust1 : MonoBehaviour {
        public float localTime;
        public float ttl = -1;
        public Vector3 speed;
        public Quaternion rotSpeed;
        public Vector3 acceleration;
        public float scale = 1;
        public float scaleSpeed = -1;

        public Vector3 angleSpeed;

        private void Update() {
            if (ttl == -1) return;
            
            localTime += Time.deltaTime;
            speed += acceleration * Time.deltaTime;
            
            
//            angleSpeed += rotAccel * dt;
            Quaternion rot = transform.rotation;
            Quaternion rotForce = MUtil.toAngleAxis(angleSpeed.length() * Time.deltaTime, angleSpeed.normalized);
            rot = rotForce * rot;
            
            transform.SetPositionAndRotation(transform.position + speed * Time.deltaTime, rot);

//            scale += scaleSpeed * Time.deltaTime;
//            if (scale <= 0) Destroy(gameObject);
            
//            transform.localScale = new Vector3(scale, scale, scale);
            if (localTime >= ttl) Destroy(gameObject);
        }
    }
}
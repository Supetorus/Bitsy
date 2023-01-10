using UnityEngine;

namespace moveen.example {
    public class Instancer : MonoBehaviour, Startable {
        public Transform dust;
        public float ttl;
        
        public void start() {
            Transform instance = Instantiate(dust);
            instance.transform.SetPositionAndRotation(transform.position, transform.rotation);
            Destroy(instance.gameObject, ttl);
        }
    }
}
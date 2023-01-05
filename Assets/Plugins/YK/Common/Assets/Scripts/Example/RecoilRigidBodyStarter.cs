using UnityEngine;
using UnityEngine.Serialization;

namespace moveen.example {
    /// <summary>
    /// Used to just increase current speed of the Rigidbody. Example: recoil after a fire or hit.
    /// </summary>
    public class RecoilRigidBodyStarter : MonoBehaviour, Startable {
        [Tooltip("Speed to add")]
        [FormerlySerializedAs("accel")]//24.11.17
        public Vector3 speedToAdd = Vector3.left;
        [Tooltip("Target GameObject with Rigidbody. If null - this GameObject will be used")]
        [FormerlySerializedAs("motor1")]//24.11.17
        public Rigidbody rigidBody;

        private void OnEnable() {
            if (rigidBody == null) rigidBody = transform.GetComponent<Rigidbody>();
        }

        public void start() {
            if (rigidBody == null) return;
            rigidBody.AddForceAtPosition(transform.TransformVector(speedToAdd), transform.position, ForceMode.Acceleration);
        }
    }
}
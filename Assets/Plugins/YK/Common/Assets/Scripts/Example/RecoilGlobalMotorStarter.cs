using UnityEngine;
using UnityEngine.Serialization;

namespace moveen.example {
    /// <summary>
    /// Used to just increase current speed of the GlobalMotor. Example: recoil after a fire or hit.
    /// </summary>
    public class RecoilGlobalMotorStarter : MonoBehaviour, Startable {
        [Tooltip("Speed to add")]
        [FormerlySerializedAs("accel")]//24.11.17
        public Vector3 speedToAdd = Vector3.left;
        [Tooltip("Target GameObject with GlobalMotor. If null - this GameObject will be used")]
        [FormerlySerializedAs("motor1")]//24.11.17
        public GlobalMotor globalMotor;

        private void OnEnable() {
            if (globalMotor == null) globalMotor = transform.GetComponent<GlobalMotor>();
        }

        public void start() {
            if (globalMotor == null) return;
            //TODO consider rigid body
            if (globalMotor.rb == null) {
                globalMotor.speed += transform.TransformVector(speedToAdd);
            } else {
                globalMotor.rb.AddForce(transform.TransformVector(speedToAdd) * 50, ForceMode.Acceleration);
            }
        }
    }
}
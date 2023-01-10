using UnityEngine;
using UnityEngine.Serialization;

namespace moveen.example {
    /// <summary>
    /// Used to just increase current speed of the LocalMotor. Example: recoil after a fire.
    /// </summary>
    public class GlobalMotorPusher : MonoBehaviour, Startable {
        [Tooltip("Speed to add")]
        [FormerlySerializedAs("accel")]//24.11.17
        public Vector3 speedToAdd = Vector3.left;
        [FormerlySerializedAs("localMotor")]
        [Tooltip("Target GameObject with LocalMotor. If null - this GameObject will be used")]
        public GlobalMotor motor;
        
        //TODO push RigidBody if present

        private void OnEnable() {
            if (motor == null) motor = transform.GetComponent<GlobalMotor>();
        }

        public void start() {
            if (motor == null) return;
            motor.speed += transform.TransformVector(speedToAdd);
        }
    }
}
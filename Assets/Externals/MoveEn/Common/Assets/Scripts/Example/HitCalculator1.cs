using moveen.utils;
using UnityEngine;

namespace moveen.example {
    /// <summary>
    /// Example class that shows how to easily implement gunshot logic.
    /// </summary>
    public class HitCalculator1 : MonoBehaviour, Startable {
        [Tooltip("Hitscanner determines current target on the sight. If not stated - it will be searched for on this GameObject")]
        public Hitscanner hitscanner;
        [Tooltip("Amount of force to be applied to the target")]
        public float hitForce = 1;

        private void Awake() {
            if (hitscanner == null) hitscanner = GetComponent<Hitscanner>();
        }

        public void start() {
            Rigidbody rigidBody = hitscanner.raycastHit.rigidbody;
            if (rigidBody != null) {
                rigidBody.AddForceAtPosition(hitscanner.origin.sub(hitscanner.raycastHit.point).normalized().mul(-hitForce), hitscanner.raycastHit.point, ForceMode.Impulse);
            }

        }
    }
}
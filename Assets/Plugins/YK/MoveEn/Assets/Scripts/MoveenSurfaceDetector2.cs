using moveen.descs;
using UnityEngine;

namespace moveen.example {
    /// <summary>
    /// Implements surface detection for the MoveenStepper5. Must be located on the same GameObject.
    /// You can implement your own version with more simple or more complex detection which can utilize specifics of your game.
    /// For example, it can be 2D or utilize some sort of grid search.
    /// </summary>
    public class MoveenSurfaceDetector2 : MonoBehaviour {
        public SurfaceDetector2 detector = new SurfaceDetector2();
        
        public void OnEnable() {
            MoveenStepper5 stepper = gameObject.GetComponent<MoveenStepper5>();
            if (stepper != null) {
                stepper.engine.surfaceDetector = detector;
            }
        }

        private void Update() {
            detector.resetMaxFoundHits = true;
        }
    }
}
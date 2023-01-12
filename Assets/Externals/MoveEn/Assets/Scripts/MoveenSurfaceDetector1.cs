using moveen.descs;
using UnityEngine;

namespace moveen.example {
    /**
     * Implements surface detection for the MoveenStepper5. Must be located on the same GameObject.
     * You can implement your own version with more simple or more complex detection which can utilize specifics of your game.
     * For example, it can be 2D or utilize some sort of grid search. 
     */
    public class MoveenSurfaceDetector1 : MonoBehaviour {
        [Header("WARNING! Unoptimized. Use MoveenSurfaceDetector2 instead")]
        public SurfaceDetector1 detector = new SurfaceDetector1();

        public void OnEnable() {
            MoveenStepper5 stepper = gameObject.GetComponent<MoveenStepper5>();
            if (stepper != null) {
                stepper.engine.surfaceDetector = detector;
            }
        }
    }
}
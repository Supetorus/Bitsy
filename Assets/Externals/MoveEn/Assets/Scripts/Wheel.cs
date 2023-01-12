using UnityEngine;

namespace moveen.core {
    [System.Serializable]
    public class Wheel {
        public float maxSpeed;//TODO implement

        [Tooltip("TODO")] 
        public float wheelRotationByBodyDir = 1;
        [Tooltip("The ability of the wheel to get the leg into the comfort position by rotating itself. 0 - the only way is by stepping. 10 - pretty strong ability.")] 
        public float wheelRotationByComfort = 10;
        [Tooltip("TODO")] 
        public float wheelRotationByComfortSpeed = 5;
        [Tooltip("The ratio of the wheel to steer in the direction of movement. 0 - steer the way the body looks and leave other directions to stepping. 1 - wheel looks at speed direction.")] 
        public float wheelRotationByBodySpeed = 1;
        public float wheelAcceleration = 0.05f;//TODO value limits, meaning
        public float wheelBreak = 0.1f;//TODO value limits, meaning


    }
}
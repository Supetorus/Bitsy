using moveen.core;
using UnityEngine;

namespace moveen.descs {
    public class MoveenStep2 : MonoBehaviour {
        [Tooltip("Stepping engine")] 
        public Step2 step = new Step2();

        public static bool showInstrumentalInfo;  

        //This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only).
        public virtual void OnValidate() {
            if (transform.parent == null) return;
            step.reset(transform.parent.position, transform.parent.rotation);
        }

        private void Update() {//just to be able to disable it in editor
        }
    }
}
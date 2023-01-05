using moveen.utils;
using UnityEngine;

namespace moveen.descs {
    [ExecuteInEditMode]
    public class MoveenScaleHelper : MonoBehaviour {
        [ReadOnly] public float lastScale;
        [HideInInspector] public bool showWarning;
        
        public void OnEnable() {
            lastScale = transform.lossyScale.x;
        }

    }
}
using UnityEngine;

namespace moveen.utils {
    public class FloatWarningAttribute : PropertyAttribute {
        public float min = float.MinValue;
        public float max = float.MaxValue;
    }
}
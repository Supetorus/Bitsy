using System;
using moveen.utils;
using UnityEngine;

namespace moveen.core {
    public abstract class ISurfaceDetector {
        [NonSerialized]public bool found;
        [NonSerialized]public Vector3 pos;
        [NonSerialized]public Vector3 normal;
        public abstract Vector3 detect(Vector3 input, Vector3 normal);
    }
}
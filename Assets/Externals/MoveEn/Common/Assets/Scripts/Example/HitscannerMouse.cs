using System;
using UnityEngine;

namespace moveen.example {
    /// <summary>
    /// Determines target on the sight from camera, through mouse.
    /// </summary>
    public class HitscannerMouse : Hitscanner {
        public FocusGrabber focusGrabber;
        public Camera cam;
        [NonSerialized]public Ray lastRay;

        public override void OnEnable() {
            base.OnEnable();
            if (cam != null) cam = GetComponent<Camera>();
            if (focusGrabber == null) focusGrabber = GetComponent<FocusGrabber>();
        }

        public override Ray getRay() {
            bool grabInput = focusGrabber != null && focusGrabber.grab;
            if (grabInput) lastRay = (cam == null ? Camera.main : cam).ScreenPointToRay(Input.mousePosition);
            return lastRay;
        }
    }
}
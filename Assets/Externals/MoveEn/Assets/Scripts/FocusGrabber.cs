using System;
using UnityEngine;

namespace moveen.example {
    /// <summary>
    /// FocusGrabber contains one checkbox - IsActive and maintains it so there is only one FocusGrabber that is active at any given time.
    /// If you check this box - it will be unchecked on any other FocusGrabbers.
    /// You can use this component to control groups of FireControls and MoveControls.
    /// One game unit could require many components that react on user input so with FocusGrabber you can control them all at once.
    /// </summary>
    public class FocusGrabber : MonoBehaviour {
        public static FocusGrabber CURRENT_GRABBER;

        public bool grab;
        [NonSerialized] private bool wasGrabbing;

        private void Update() {
            if (grab && !wasGrabbing) {
                if (CURRENT_GRABBER != null) CURRENT_GRABBER.grab = false;
                CURRENT_GRABBER = this;
            }
            wasGrabbing = grab;
        }


    }
}
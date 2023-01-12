using System;
using UnityEngine;
using UnityEngine.UI;

namespace moveen.example {
    public class FpsText : MonoBehaviour {
        public float f = 0.02f;
        [NonSerialized]private Text text;
        [NonSerialized]private float deltaTime;


        private void OnEnable() {
            text = GetComponent<Text>();
        }

        private void Update() {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * f;
            text.text = "fps: " + 1f/deltaTime;
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace moveen.example {
    public class ButtonHighlight : MonoBehaviour {
        private Button button;
        public string keyCode;
        public bool isKey = true;

        public void Start() {
            button = gameObject.GetComponent<Button>();
        }

        public void Update() {
            if (button == null) return;

            var colors = GetComponent<Button> ().colors;
            string key = keyCode == "" ? button.GetComponentInChildren<Text>().text.ToLower() : keyCode;

            bool isDown = isKey ? Input.GetKey(key) : Input.GetButton(key);
            if (isDown) {
                colors.normalColor = Color.gray;
            } else {
                colors.normalColor = Color.white;
            }
            GetComponent<Button> ().colors = colors;

        }
    }
}
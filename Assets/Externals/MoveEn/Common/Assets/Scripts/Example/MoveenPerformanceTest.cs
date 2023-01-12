using System;
using moveen.utils;
using UnityEngine;
using UnityEngine.UI;

namespace moveen.example {
    public class MoveenPerformanceTest : MonoBehaviour {
        public Button btnPlus;
        public Button btnMinus;
        public Button nextModel;
        public Text txtInfo;
        public Transform[] models;
        public int selectedModel;

        public Vector3 pos;
        public int width;

        [NonSerialized]private float deltaTime;
        [ReadOnly] public int count;

        private void Update() {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.02f;
            txtInfo.text = "model: " + models[selectedModel].name + " count: " + count + " fps: " + 1f/deltaTime;
        }

        private void OnEnable() {
            btnPlus.onClick.AddListener(increase);
            btnMinus.onClick.AddListener(decrease);
            nextModel.onClick.AddListener(onNextModel);
            reset();
        }

        private void increase() {
            width++;
//            width *= 2;
            if (width == 0) width = 1;
            reset();
        }

        private void onNextModel() {
            if (models.Length == 0) return;
            selectedModel = (selectedModel + 1) % models.Length;
            reset();
        }

        private void reset() {
            count = width * width;

            transform.gameObject.removeAllChildrenImmediate(false);
            for (int x = 0; x < width; x++) {
                for (int z = 0; z < width; z++) {
                    Transform newObject = Instantiate(models[selectedModel], transform);
                    newObject.position = pos.mul(x, 1, z);
                }
            }


        }

        private void decrease() {
            width--;
//            width /= 2;
            if (width < 0) width = 0;
            reset();
        }
    }
}
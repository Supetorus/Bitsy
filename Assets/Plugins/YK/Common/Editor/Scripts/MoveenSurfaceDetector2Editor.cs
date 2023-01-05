using moveen.example;
using UnityEditor;

namespace moveen.editor {
    [CustomEditor(typeof(MoveenSurfaceDetector2))]
    [CanEditMultipleObjects]
    public class MoveenSurfaceDetector2Editor : Editor {
        private MoveenSurfaceDetector2 detector;

        public void OnEnable() {
            detector = (MoveenSurfaceDetector2) target;
        }

        public override void OnInspectorGUI() {
            if (detector.detector.maxFoundHits == detector.detector.bufferSize) {
                EditorGUILayout.HelpBox("Found hits == buffer size. Consider fixing surface geometry OR decreasing detect distance OR increasing buffer size.", MessageType.Error);
            }
            DrawDefaultInspector();
        }
    }
}
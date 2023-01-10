using moveen.utils;
using UnityEditor;
using UnityEngine;

namespace moveen.editor {
    [CustomPropertyDrawer(typeof(BindWarningAttribute))]
    public class BindWarningAttributeDrawer : PropertyDrawer {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            float result = EditorGUI.GetPropertyHeight(property, label, true);
            if (property.objectReferenceValue == null) result += 45;
            return result;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.PropertyField(position, property, label, true);

            Rect warningBox = new Rect(position);
            string warningText = "";

            Object value = property.objectReferenceValue;
            if (value == null) {
                warningText += "Must be bind";
            }

            if (warningText.Length > 0) {
                float propertyHeight = EditorGUI.GetPropertyHeight(property, label, true);
                warningBox.yMin += propertyHeight;
                warningBox.yMax -= 5;
                EditorGUI.HelpBox(warningBox, warningText, MessageType.Error);
            }
        }
    }
}
using moveen.utils;
using UnityEditor;
using UnityEngine;

namespace moveen.editor {
    [CustomPropertyDrawer(typeof(FloatWarningAttribute))]
    public class FloatWarningAttributeDrawer : PropertyDrawer {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            FloatWarningAttribute attr = ((FloatWarningAttribute)attribute);

            float result = EditorGUI.GetPropertyHeight(property, label, true);
            if (property.floatValue >= attr.max || property.floatValue <= attr.min) result += 45;
            return result;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.PropertyField(position, property, label, true);

            FloatWarningAttribute attr = ((FloatWarningAttribute)attribute);
            Rect warningBox = new Rect(position);
            string warningText = "";

            float value = property.floatValue;
            if (attr.max == float.MaxValue) {
                if (value <= attr.min) {
                    warningText += "Must be > " + attr.min + ". ";
                }
            } else if (attr.min == float.MinValue) {
                if (value >= attr.max) {
                    warningText += "Must be < " + attr.max + ". ";
                }
            } else {
                if (value >= attr.max || value <= attr.min) {
                    warningText += "Must be between " + attr.min + " and " + attr.max + ". ";
                }
            }

            if (warningText.Length > 0) {
                float propertyHeight = EditorGUI.GetPropertyHeight(property, label, true);
                warningBox.yMin += propertyHeight;
                warningBox.yMax -= 5;
                EditorGUI.HelpBox(warningBox, warningText, MessageType.Warning);
            }
        }
    }
}
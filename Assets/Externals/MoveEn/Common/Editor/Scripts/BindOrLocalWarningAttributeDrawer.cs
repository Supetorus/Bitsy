using moveen.utils;
using UnityEditor;
using UnityEngine;

namespace moveen.editor {
    [CustomPropertyDrawer(typeof(BindOrLocalWarningAttribute))]
    public class BindOrLocalWarningAttributeDrawer : PropertyDrawer {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            BindOrLocalWarningAttribute attr = ((BindOrLocalWarningAttribute)attribute);

            float result = EditorGUI.GetPropertyHeight(property, label, true);
            if (property.objectReferenceValue == null && getCandidate(property) == null) result += 45;
            
            return result;
        }

        private static Component getCandidate(SerializedProperty property) {
            return ((MonoBehaviour)property.serializedObject.targetObject).gameObject.GetComponent(getType(property));
        }

        //https://answers.unity.com/questions/1347203/a-smarter-way-to-get-the-type-of-serializedpropert.html
        public static System.Type getType(SerializedProperty property) {
            System.Type parentType = property.serializedObject.targetObject.GetType();
            System.Reflection.FieldInfo fi = parentType.GetField(property.propertyPath);
            return fi.FieldType;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.PropertyField(position, property, label, true);

            BindOrLocalWarningAttribute attr = ((BindOrLocalWarningAttribute)attribute);
            Rect warningBox = new Rect(position);
            string warningText = "";

            Object value = property.objectReferenceValue;
            if (value == null && getCandidate(property) == null) {
                warningText += "Should be either provided (bound) or be present in the same GameObject";
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
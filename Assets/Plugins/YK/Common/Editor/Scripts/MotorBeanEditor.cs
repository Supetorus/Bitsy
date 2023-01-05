using moveen.core;
using UnityEditor;
using UnityEngine;

namespace moveen.editor {
    [CustomPropertyDrawer(typeof(MotorBean))]
    public class MotorBeanEditor : PropertyDrawer {
        string[] _choices = new [] { "Robo heavy", "Robo precise", "Bio tidy", "Bio relaxed"};
        
        public static MotorBean[] beans = {
            new MotorBean {distanceToSpeed = 10, maxSpeed = 2, speedDifToAccel = 3, maxAccel = 10},
            new MotorBean {distanceToSpeed = 10, maxSpeed = 1, speedDifToAccel = 20, maxAccel = 100},
            new MotorBean {distanceToSpeed = 4, maxSpeed = 10, speedDifToAccel = 10, maxAccel = 50},
            new MotorBean {distanceToSpeed = 3, maxSpeed = 10, speedDifToAccel = 3, maxAccel = 20},
        };
        

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
//            EditorGUI.BeginProperty(position, label, property);
//            EditorGUI.PropertyField(position, property, label, true);
//            EditorGUI.EndProperty();
//            EditorGUILayout.Popup("Motor presets", 0, _choices);

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(position, property, label, true);
            EditorGUI.EndProperty();

            if (property.isExpanded) {
                Rect r2 = new Rect(position);
                r2.y += EditorGUI.GetPropertyHeight(property);
                r2.height = 20;
                int chosen = EditorGUI.Popup(r2, "Motor presets", -1, _choices);
                if (chosen > -1) {
                    property.FindPropertyRelative("distanceToSpeed").floatValue = beans[chosen].distanceToSpeed;
                    property.FindPropertyRelative("maxSpeed").floatValue = beans[chosen].maxSpeed;
                    property.FindPropertyRelative("speedDifToAccel").floatValue = beans[chosen].speedDifToAccel;
                    property.FindPropertyRelative("maxAccel").floatValue = beans[chosen].maxAccel;
                }
                
            }

//
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
                return EditorGUI.GetPropertyHeight(property) + 20f;
            return EditorGUI.GetPropertyHeight(property);
        }
        
//        public override void OnInspectorGUI() {
//            
//            
////            DrawDefaultInspector();
//            
//            EditorGUILayout.Popup("Motor presets", 0, _choices);
//            //EditorUtility.SetDirty(target);
//        }

    }
}
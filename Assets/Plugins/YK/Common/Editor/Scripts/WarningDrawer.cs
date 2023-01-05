using UnityEditor;
using UnityEngine;

namespace moveen.editor {
    [CustomPropertyDrawer(typeof (WarningAttribute))]
    internal sealed class WarningDrawer : DecoratorDrawer
    {
        public override void OnGUI(Rect position)
        {
            var text = ((WarningAttribute) attribute).header;
            EditorGUI.HelpBox(position, text, MessageType.Error);
        }

        public override float GetHeight() {
            return 36f;
            
        }
    }
}
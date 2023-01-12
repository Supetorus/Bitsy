using moveen.descs;
using moveen.utils;
using UnityEditor;
using UnityEngine;

namespace moveen.editor {
//    [CustomEditor(typeof(MoveenSkelSimplestTarget), true)]
//    [CanEditMultipleObjects]
    public static class MoveenSkelSimplestTargetEditor {

        [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
        private static void Gizmo(MoveenSkelSimplestTarget component, GizmoType gizmoType) {
            Gizmos.color = MoveenSkelWithBonesEditor.BONE_COLOR;
            Vector3 dir = component.targetPos.sub(component.transform.position);
            Vector3 minPos = dir.normalized(component.minLen).add(component.transform.position);
            if (component.minLen > 0) UnityEditorUtils.circle3d(minPos, Quaternion.FromToRotation(Vector3.up, dir), component.maxLen * 0.1f, 20);
            Vector3 maxPos = dir.normalized(component.maxLen).add(component.transform.position);
            UnityEditorUtils.circle3d(maxPos, Quaternion.FromToRotation(Vector3.up, dir), component.maxLen * 0.1f, 20);
            Gizmos.DrawLine(minPos, maxPos);
        }
    }
}
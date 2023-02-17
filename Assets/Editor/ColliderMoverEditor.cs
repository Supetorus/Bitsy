using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ColliderMover))]
public class MaterialReplacerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		ColliderMover mover = (ColliderMover) target;

		base.OnInspectorGUI();

		if (GUILayout.Button("Update All Children Of Current Object"))
		{
			mover.Move();
		}
	}
}

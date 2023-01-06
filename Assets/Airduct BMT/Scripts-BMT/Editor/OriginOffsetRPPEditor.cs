using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace BuildingMakerToolset.PropPlacer
{
    [CustomEditor(typeof(OriginOffsetRPP))]
    public class OriginOffsetRPPEditor : RowPropPlacerEditor
    {

        public override void OnInspectorGUI()
        {
            DrawToggleBar(curRowPropPlacerTarget);
            if (targets.Length == 1)
                DrawSpawnPivotEditor(curRowPropPlacerTarget);
        }
    }
}

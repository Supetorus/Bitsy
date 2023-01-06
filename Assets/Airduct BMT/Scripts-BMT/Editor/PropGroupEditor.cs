#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityObject = UnityEngine.Object;

namespace BuildingMakerToolset.PropPlacer
{
    [CustomEditor( typeof( PropGroup ) )]
    public class PropGroupEditor : Editor
    {
        SerializedObject serializeTgt;
        SerializedProperty prefabArrayProp;
        GUIContent tmpGUIContent;
        private void OnEnable()
        {
            PropGroup tgt = (PropGroup)target;

            if (tgt.prefabs == null || tgt.prefabs.Length == 0)
                dragAndDropInfo = true;
            doubleSamePrefabWarning = false; 
            tmpGUIContent = new GUIContent();
            serializeTgt = new SerializedObject( (PropGroup)target );
            prefabArrayProp = serializeTgt.FindProperty( "prefabs" );
        }
        public override void OnInspectorGUI()
        {
            var tgt = (target as PropGroup);
            serializeTgt.UpdateIfRequiredOrScript();
            DrawPrefabView( tgt );
            serializeTgt.ApplyModifiedProperties();
        }







        GUIStyle prefabBox;
        GUIStyle prefabViewName;
        GUIStyle prefabViewNameNotApplied;
        bool doubleSamePrefabWarning;
        bool dragAndDropInfo;
        Vector2 scrollPosition;
        void DrawPrefabView(PropGroup tgt)
        {

            if (prefabViewName == null || prefabViewNameNotApplied == null || prefabBox == null)
            {
                prefabViewName = new GUIStyle( "MiniLabel" );
                prefabViewName.alignment = TextAnchor.LowerCenter;
                prefabViewName.fontStyle = FontStyle.Normal;

                prefabViewNameNotApplied = new GUIStyle( "MiniLabel" );
                prefabViewNameNotApplied.alignment = TextAnchor.LowerCenter;
                prefabViewNameNotApplied.fontStyle = FontStyle.Bold;

                prefabBox = new GUIStyle( "HelpBox" );
            }






            int width = (int)EditorGUIUtility.currentViewWidth - 40;
            int hight;

            int spacing = 10;
            int tgtButtonWidth = 100;

            int vButtons = Mathf.FloorToInt( width / (tgtButtonWidth + spacing) );

            int buttonWidth = tgtButtonWidth + (width - spacing * vButtons - tgtButtonWidth * vButtons) / Mathf.Max( vButtons, 1 );
            int buttonHight = buttonWidth;

            float h = 1;
            if (tgt.prefabs != null && tgt.prefabs.Length > 0)
                h = tgt.prefabs.Length;
            hight = Mathf.Clamp( Mathf.CeilToInt( h / vButtons ) * (buttonHight + spacing), buttonHight + 2 * spacing, 300 );
            if (dragAndDropInfo)
            {
                EditorGUILayout.HelpBox( "To add preabs drag and drop them in the field below or use right mouse button to remove them from the list", MessageType.Info );
            }
            if (doubleSamePrefabWarning)
            {
                EditorGUILayout.HelpBox( "you cant add same prefab twice", MessageType.Info );
            }

            GUILayout.MaxWidth( width );
            GUILayout.Label( "Prefabs", prefabViewName );
            scrollPosition = GUILayout.BeginScrollView( scrollPosition, false, true, GUIStyle.none, "verticalScrollbar", GUIStyle.none, GUILayout.Width( width ), GUILayout.Height( hight ) );
            Event evt = Event.current;
            Rect drop_area = new Rect( 0, 0, width, hight );
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!drop_area.Contains( evt.mousePosition ))
                        return;
                    bool acceptDrag = true;
                    foreach (UnityObject dragged_object in DragAndDrop.objectReferences)
                    {
                        var go = dragged_object as GameObject;
                        if (go == null)
                            acceptDrag = false;
                        else if (!BHUtilities.IsPrefab( go ))
                            acceptDrag = false;
                        if (!acceptDrag)
                            break;
                    }

                    if (acceptDrag)
                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                    else
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        doubleSamePrefabWarning = false;
                        foreach (UnityObject dragged_object in DragAndDrop.objectReferences)
                        {
                            if (BHUtilities.ArrayContainsPrefab( prefabArrayProp, dragged_object ))
                            {
                                doubleSamePrefabWarning = true;
                                continue;
                            }
                            BHUtilities.ArrayAdd( prefabArrayProp, (GameObject)dragged_object );
                            dragAndDropInfo = false;
                        }
                    }
                    break;
            }


            tmpGUIContent.image = null;
            tmpGUIContent.tooltip = null;

            GUI.Box( drop_area, tmpGUIContent, prefabBox );

            if (tgt.prefabs == null || tgt.prefabs.Length == 0)
            {
                GUILayout.EndScrollView();
                return;
            }





            GUILayout.BeginHorizontal();
            int collum = 0;
            int row = 0;
            for (int i = 0; i < tgt.prefabs.Length; i++)
            {
                if (tgt.prefabs[i] == null)
                    continue;
                if (!BHUtilities.IsPrefab( tgt.prefabs[i] ))
                {
                    BHUtilities.ArrayRemoveAt( prefabArrayProp, i );
                    break;
                }
                Texture icon = AssetPreview.GetAssetPreview( tgt.prefabs[i] );




                float off = 20;
                Rect btnArea = new Rect( row * (buttonWidth + 3) + 3, collum * (buttonHight + 2), buttonWidth, buttonHight );
                GUILayout.BeginArea( btnArea );
                GUILayout.BeginVertical();

      
                GUILayout.Label( tgt.prefabs[i].name, prefabViewName );

                tmpGUIContent.image = icon;
                tmpGUIContent.tooltip = tgt.prefabs[i].name;
                
                if (GUILayout.Button( tmpGUIContent, GUILayout.Height( buttonHight - off ), GUILayout.Width( buttonWidth ), GUILayout.ExpandWidth( false ) ))
                {

                    Vector2 mousePos = evt.mousePosition;
                    RowPropPlacer.curArray = prefabArrayProp;
                    RowPropPlacer.curIndex = i;
                    EditorUtility.DisplayPopupMenu( new Rect( mousePos.x, mousePos.y, 0, 0 ), "CONTEXT/SpawnerEditor/", null );

                }
                
                GUILayout.EndVertical();
                GUILayout.EndArea();

                Rect PropertyArea = btnArea;
                PropertyArea.x += off;
                PropertyArea.height = off;


                int num = (int)Mathf.Repeat( i, vButtons );

                row++;
                if (num == vButtons - 1)
                {
                    row = 0;
                    collum++;
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
            EditorGUILayout.PropertyField( prefabArrayProp, true );
        }
    }


}
#endif
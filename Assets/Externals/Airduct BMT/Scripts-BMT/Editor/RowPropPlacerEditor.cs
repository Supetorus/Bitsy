//#define ISFULLVERSION
using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityObject = UnityEngine.Object;
using UnityEditor.UIElements;
using UnityEngine.UIElements;



namespace BuildingMakerToolset.PropPlacer
{


    [CanEditMultipleObjects]
    [CustomEditor( typeof( RowPropPlacer ) )]
    public class RowPropPlacerEditor : Editor
    {
        
        bool editNotPossible;
        bool doubleSamePrefabWarning;
        bool dragAndDropInfo;
        
        Vector2 groupViewscrollPosition;
        GUIContent tmpGUIContent;
        GUIContent tmpGUIContent2;


        SerializedObject serializeTgt;
        SerializedProperty prefabArrayProp;
        SerializedProperty prefabGroupArrayProp;
        SerializedProperty spawnPivotProp;
        SerializedProperty pivotPositionProp;
        SerializedProperty pivotRotationProp;
        SerializedProperty origenRotationProp;
        SerializedProperty originOffsetProp;


        GUIStyle prefabBox;
        GUIStyle prefabViewName;
        GUIStyle vertScrl;
        GUIStyle prefabFromGroupViewName;
        GUIStyle prefabViewNameNotApplied;
        static Dictionary<int, Texture2D> cachedIcons = new Dictionary<int, Texture2D>();
        Event curEvent;
        protected RowPropPlacer curRowPropPlacerTarget;
        static RowPropPlacerEditor curRowPropPlacerEditor;
        private void OnEnable()
        {
            curRowPropPlacerEditor = this;
            tmpGUIContent = new GUIContent();
            tmpGUIContent2 = new GUIContent();
            curRowPropPlacerTarget = (RowPropPlacer)target;
            if (curRowPropPlacerTarget.GetTotalPrefabLength() == 0)
                dragAndDropInfo = true;
            editNotPossible = false;
            doubleSamePrefabWarning = false;
            serializeTgt = new SerializedObject( (RowPropPlacer)target );
            Selection.selectionChanged += OnSelectionChanged;
            prefabArrayProp = serializeTgt.FindProperty( "prefabs" );
            prefabGroupArrayProp = serializeTgt.FindProperty( "prefabGroups" );
            spawnPivotProp = serializeTgt.FindProperty( "spawnPivot" );
            pivotPositionProp = spawnPivotProp.FindPropertyRelative( "pivPosition" );
            pivotRotationProp = spawnPivotProp.FindPropertyRelative( "pivRotation" );
            origenRotationProp = spawnPivotProp.FindPropertyRelative( "originRotation" );
            originOffsetProp = spawnPivotProp.FindPropertyRelative( "originOffset" );
            doubleSamePrefabWarning = false;
            curRowPropPlacerTarget.ResetOnInpectorEnable();
        }

        [CanEditMultipleObjects]
        public override void OnInspectorGUI()
        {

            if (curRowPropPlacerTarget == null)
                return;
            curEvent = Event.current;
            int controlID = GUIUtility.GetControlID( GetHashCode(), FocusType.Passive );
            EventType eventType = curEvent.GetTypeForControl( controlID );

            serializeTgt.UpdateIfRequiredOrScript();



#if ISFULLVERSION
            DrawToggleBar( curRowPropPlacerTarget );
            DrawSpawnPivotEditor( curRowPropPlacerTarget );
#endif
            DrawGroupView( curRowPropPlacerTarget );
            DrawPrefabView( curRowPropPlacerTarget );
            
            
            EditorGUILayout.PropertyField( prefabArrayProp, false );

            serializeTgt.ApplyModifiedProperties();

            if(targets.Length == 1)
                BuildingUnitEditor.DrawBuildingUnitEditor( curRowPropPlacerTarget as BuildingUnit );

        }











        bool AxisConstrainedVertexSnap = true;
        protected void DrawToggleBar(RowPropPlacer tgt)
        {
            if (targets.Length > 1)
                return;
            int barSpace = 10;
            int barHight = 38;
            int barWidth = (int)EditorGUIUtility.currentViewWidth - 43;
            int applyWarningExtraSpace = 0;

            Rect _rect = new Rect( barSpace * 2, barSpace + applyWarningExtraSpace, barWidth + barSpace, barHight + 100);
            
            GUILayout.BeginArea( _rect );
            GUILayout.BeginHorizontal( "box" );
            GUILayout.MaxWidth( barWidth );


            
            bool editSpawnPivot = GUILayout.Toggle( tgt.editSpawnPivot, tgt.editSpawnPivot ? "Editing pivot" : "Edit pivot", "button", GUILayout.Height( barHight - barSpace ), GUILayout.Width( barWidth / 3 ) );

            if(editSpawnPivot)
                AxisConstrainedVertexSnap = GUILayout.Toggle( AxisConstrainedVertexSnap,new GUIContent( "Axis Constrained Vertex Snapping", "Grab the Axis of the Move Gizmo that you want to move, drag the cursor to any Vertex of the selected Mesh and the Gizmo will snap to it, but only on the grabbed axis." ), GUILayout.Height( barHight - barSpace ), GUILayout.Width( barWidth /2 ) );
            if (editSpawnPivot != tgt.editSpawnPivot)
            {
                if (Selection.activeObject!= tgt.gameObject)
                {
                    editNotPossible = true;
                    tgt.editSpawnPivot = false;
                }
                else
                {
                    editNotPossible = false;
                    tgt.editSpawnPivot = editSpawnPivot;
                    Tools.hidden = tgt.editSpawnPivot;
                    if(editSpawnPivot)
                        SceneView.lastActiveSceneView.drawGizmos = true;

                    SceneView.RepaintAll();
                    
                }
            }
            
            if (editNotPossible)
            {
                if (Selection.Contains( tgt.gameObject ))
                    editNotPossible = false;
                EditorGUILayout.HelpBox( "You are locked to a different selection", MessageType.Warning, true );
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            GUILayout.Space( barHight + barSpace );
        }












        enum PivotEditTarget { originOffset, originRotation, spawnOffset,  spawnRotation }
        PivotEditTarget curPivotEditTarget;
        protected void DrawSpawnPivotEditor(RowPropPlacer tgt)
        {
            if (!tgt.editSpawnPivot || targets.Length > 1)
                return;
            float iconSize = 15;
            EditorGUILayout.BeginHorizontal();
            GUIContent pivotEnabled = new GUIContent( EditorGUIUtility.IconContent( "Transform Icon" ).image as Texture2D , "Toggle handle");
            GUIContent pivotDisabled = new GUIContent( EditorGUIUtility.IconContent( "Button Icon" ).image as Texture2D, "Toggle handle" );

            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button( curPivotEditTarget == PivotEditTarget.originOffset ? pivotEnabled : pivotDisabled, GUIStyle.none, GUILayout.ExpandWidth( false ), GUILayout.Width( iconSize ), GUILayout.Height( iconSize ) ))
                curPivotEditTarget = PivotEditTarget.originOffset;
            EditorGUILayout.PropertyField( originOffsetProp, new GUIContent( "Origin Position", "Pivot position Override for this object when placing with RowPropPlacer. This must be on a edge of your mesh." ) );
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button( curPivotEditTarget == PivotEditTarget.originRotation ? pivotEnabled : pivotDisabled, GUIStyle.none, GUILayout.ExpandWidth( false ), GUILayout.Width( iconSize ), GUILayout.Height( iconSize ) ))
                curPivotEditTarget = PivotEditTarget.originRotation;
            EditorGUILayout.PropertyField( origenRotationProp, new GUIContent( "Origin Rotation", "Rotation Override for this object when placing with RowPropPlacer" ) );
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (EditorGUI.EndChangeCheck())
            {
                curRowPropPlacerTarget.ResetPreview();
                //curRowPropPlacerTarget.forceTransformChanged = true;
            }

            if (GUILayout.Button( curPivotEditTarget == PivotEditTarget.spawnOffset ? pivotEnabled : pivotDisabled, GUIStyle.none, GUILayout.ExpandWidth( false ), GUILayout.Width( iconSize ), GUILayout.Height( iconSize ) ))
                curPivotEditTarget = PivotEditTarget.spawnOffset;
            EditorGUILayout.PropertyField( pivotPositionProp, new GUIContent( "Next Position", "Relative position for placed objects. This is affected by Origin Position Override. Make sure you have set your Origin Position, before editing this. This must be on a edge opposite the origin" ) );
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button( curPivotEditTarget == PivotEditTarget.spawnRotation ? pivotEnabled : pivotDisabled, GUIStyle.none, GUILayout.ExpandWidth( false ), GUILayout.Width( iconSize ), GUILayout.Height( iconSize ) ))
                curPivotEditTarget = PivotEditTarget.spawnRotation;
            EditorGUILayout.PropertyField( pivotRotationProp, new GUIContent("Next Rotation", "Relative rotation for placed objects" ) );
            EditorGUILayout.EndHorizontal();
        }











        void DrawGroupView(RowPropPlacer tgt)
        {
            if (prefabGroupArrayProp.prefabOverride)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button( "Apply" ))
                {
                    PrefabUtility.ApplyPropertyOverride( prefabGroupArrayProp, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot( tgt ), InteractionMode.AutomatedAction );
                }
                if (GUILayout.Button( "Revert" ))
                {
                    PrefabUtility.RevertPropertyOverride( prefabGroupArrayProp, InteractionMode.AutomatedAction );
                }
                GUILayout.EndHorizontal();
            }
            
            int btnH = 22;
            int btnW = 30;
            int arryLength = prefabGroupArrayProp.arraySize;

            if (arryLength > 0)
            {
                groupViewscrollPosition = GUILayout.BeginScrollView( groupViewscrollPosition, false, false, GUIStyle.none, "verticalScrollbar", GUIStyle.none, GUILayout.MaxHeight(Mathf.Min(190, arryLength * (btnH+2) ) ),GUILayout.ExpandHeight(false));
                tgt.UpdateGroupMask();
                for (int i = 0; i < arryLength; i++)
                {
                    GUILayout.BeginHorizontal();
                    tmpGUIContent2.image = EditorGUIUtility.IconContent( i< tgt.groupMask.Length && tgt.groupMask[i] ?   "scenevis_visible_hover":"SceneViewVisibility" ).image as Texture2D;
                    tmpGUIContent2.tooltip = i < tgt.groupMask.Length && tgt.groupMask[i] ? "Hide. Right mouse button to hide all except this" : "Show. right mouse button to hide all except this";
                    if(targets.Length == 1)
                        if (GUILayout.Button( tmpGUIContent2, GUILayout.Width( btnW ), GUILayout.Height( btnH ) ))
                        {
                            if (curEvent.button == 0)
                                tgt.groupMask[i] = !tgt.groupMask[i];
                            else if (curEvent.button == 1)
                            {
                                for(int y = 0; y< tgt.groupMask.Length; y++)
                                {
                                    tgt.groupMask[y] = false;
                                }
                                tgt.groupMask[i] = true;
                            }
                        }
                    GUILayout.Label( prefabGroupArrayProp.GetArrayElementAtIndex( i ).objectReferenceValue != null ? prefabGroupArrayProp.GetArrayElementAtIndex( i ).objectReferenceValue.name : "empty", "helpBox", GUILayout.Width( btnW ), GUILayout.Height( btnH ), GUILayout.ExpandWidth( true ) );
                    if(targets.Length > 1)
                    {
                        GUILayout.EndHorizontal();
                        continue;
                    }

                    tmpGUIContent2.image = EditorGUIUtility.IconContent( "UpArrow" ).image as Texture2D;
                    tmpGUIContent2.tooltip = "Move up. Right mouse button to move down";
                        
                    if (GUILayout.Button( tmpGUIContent2, GUILayout.Width( btnW ), GUILayout.Height( btnH ) ))
                    {
                        if (curEvent.button == 0)
                            prefabGroupArrayProp.MoveArrayElement( i, i == 0 ? prefabGroupArrayProp.arraySize - 1 : i - 1 );
                        else if (curEvent.button == 1)
                            prefabGroupArrayProp.MoveArrayElement( i, i == prefabGroupArrayProp.arraySize - 1 ? 0 : i + 1 );
                    }
                    tmpGUIContent2.image = EditorGUIUtility.IconContent( "Toolbar Minus" ).image as Texture2D;
                    tmpGUIContent2.tooltip = "Remove";
                    if (GUILayout.Button( tmpGUIContent2, GUILayout.Width( btnW ), GUILayout.Height( btnH ) ))
                    {
                        if (curEvent.button == 0)
                        {
                            prefabGroupArrayProp.MoveArrayElement( i, prefabGroupArrayProp.arraySize-1 );
                            prefabGroupArrayProp.arraySize--;
                            prefabGroupArrayProp.serializedObject.ApplyModifiedProperties();
                            GUILayout.EndHorizontal();
                            GUILayout.EndScrollView();
                            return;
                        }

                    }
                    GUILayout.EndHorizontal();
                    
                }
                GUILayout.EndScrollView();


            }
        }








   
        void DrawPrefabView(RowPropPlacer tgt)
        {

            if (prefabViewName == null || prefabViewNameNotApplied == null || prefabFromGroupViewName == null || prefabBox == null)
            {
                prefabViewName = new GUIStyle( "MiniLabel" );
                prefabViewName.alignment = TextAnchor.LowerCenter;
                prefabViewName.fontStyle = FontStyle.Normal;


                prefabFromGroupViewName = new GUIStyle( "MiniLabel" );
                prefabFromGroupViewName.alignment = TextAnchor.LowerCenter;
                prefabFromGroupViewName.fontStyle = FontStyle.Italic;
                prefabFromGroupViewName.normal.textColor = new Color( 1, 1, 1,0.25f );


                prefabViewNameNotApplied = new GUIStyle( "MiniLabel" );
                prefabViewNameNotApplied.alignment = TextAnchor.LowerCenter;
                prefabViewNameNotApplied.fontStyle = FontStyle.Bold;

                prefabBox = new GUIStyle( "HelpBox" );
            }






           

            int spacing = 4;
            int tgtButtonWidth = 100;

            int width = (int)EditorGUIUtility.currentViewWidth - 45;
            int maxHight = 500;

            int vButtons = Mathf.FloorToInt( width / (tgtButtonWidth + spacing) );

            int buttonWidth = tgtButtonWidth + (width - spacing * vButtons - tgtButtonWidth * vButtons) / Mathf.Max( vButtons, 1 );
            int buttonHight = buttonWidth;

            int totalPrefabCount =  Mathf.Max(tgt.GetTotalPrefabLength(),1);

            int hight = maxHight;// Mathf.Clamp( Mathf.CeilToInt( totalPrefabCount / vButtons ) * (buttonHight + spacing), buttonHight + 2 * spacing, maxHight);
            EditorGUIUtility.labelWidth = 100 ;
            tgt.operatorType = (RowPropPlacer.Operator)EditorGUILayout.EnumPopup( "Operation type", tgt.operatorType );

            if (dragAndDropInfo)
            {
                EditorGUILayout.HelpBox( "To add preabs drag and drop them in the field below or use right mouse button to remove them from the list", MessageType.Info );
            }
            if (doubleSamePrefabWarning)
            {
                EditorGUILayout.HelpBox( "You can't add same item twice", MessageType.Info );
            }


            if (prefabArrayProp.prefabOverride && targets.Length==1)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button( "Apply"))
                {
                    PrefabUtility.ApplyPropertyOverride( prefabArrayProp, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot( tgt ), InteractionMode.AutomatedAction );
                }
                if (GUILayout.Button( "Revert" ))
                {
                    PrefabUtility.RevertPropertyOverride( prefabArrayProp , InteractionMode.AutomatedAction );
                }
                GUILayout.EndHorizontal();
            }

            
            tgt.prefabViewscrollPosition = GUILayout.BeginScrollView(tgt.prefabViewscrollPosition, false, true, GUIStyle.none, "verticalScrollbar", prefabBox, GUILayout.Height( hight ),GUILayout.Width(width));
            Rect scrollArea = new Rect( 0, tgt.prefabViewscrollPosition.y, width, hight );
            int colum = 0;
            int row = 0;
            int prefabGroupLength = tgt.GetPrefabGroupLength();



            if (totalPrefabCount > 0)
            {
                GUILayout.BeginHorizontal();
                bool hasPreview = false;
                for (int i = 0; i < totalPrefabCount; i++)
                {


                    int prefabsIndex = i - prefabGroupLength;
                    bool drawingPrefabGroups = prefabsIndex < 0;
                    GameObject curPrefab = null;

                    if (prefabsIndex < 0)
                        curPrefab = tgt.combinedPrefabGroup[i];
                    else if (tgt.prefabs != null  && prefabsIndex< tgt.prefabs.Length)
                        curPrefab = tgt.prefabs[prefabsIndex];
                    else
                        break;
                    if (curPrefab == null)
                        continue;
                    if (drawingPrefabGroups && !BHUtilities.IsPrefab( curPrefab ))
                    {
                        BHUtilities.ArrayRemoveAt( prefabArrayProp, prefabsIndex );
                        break;
                    }
                    GameObject sourcePrefab = curPrefab;
                    if (sourcePrefab == null)
                        continue;
                    
                    Texture icon = GetIcon(sourcePrefab, buttonWidth, buttonHight);


                    
                    float off = 20;
                    Rect btnArea = new Rect( row * (buttonWidth + 3) + 3, colum * (buttonHight + 2), buttonWidth, buttonHight );
                    GUILayout.BeginArea( btnArea );
                    GUILayout.BeginVertical();

                    if (drawingPrefabGroups)
                        GUILayout.Label( sourcePrefab.name, prefabFromGroupViewName );
                    else if (prefabsIndex < prefabArrayProp.arraySize && prefabArrayProp.GetArrayElementAtIndex( prefabsIndex ).prefabOverride)
                        GUILayout.Label( sourcePrefab.name, prefabViewNameNotApplied );
                    else
                        GUILayout.Label( sourcePrefab.name, prefabViewName );

                    tmpGUIContent.image = icon;
                    tmpGUIContent.tooltip = sourcePrefab.name;

                    if (GUILayout.Button( tmpGUIContent, GUILayout.Height( buttonHight - off ), GUILayout.Width( buttonWidth ), GUILayout.ExpandWidth( false ) ) &&  targets.Length == 1)
                    {

                        Vector2 mousePos = curEvent.mousePosition;
                        if (curEvent.button == 0)
                            Spawn( sourcePrefab, tgt );
                        else if (curEvent.button == 1)
                        {
                            if (!drawingPrefabGroups)
                            {
                                curRowPropPlacerEditor = this;
                                RowPropPlacer.curArray = prefabArrayProp;
                                RowPropPlacer.curIndex = prefabsIndex;
                                EditorUtility.DisplayPopupMenu( new Rect( mousePos.x, mousePos.y, 0, 0 ), "CONTEXT/SpawnerEditor/", null );
                            }
                        }

                    }
                    GUILayout.EndVertical();
                    GUILayout.EndArea();


                 
                    if (btnArea.Contains( curEvent.mousePosition ) && scrollArea.Contains( curEvent.mousePosition ))
                    {
                        SceneView.lastActiveSceneView.drawGizmos = true;
                        tgt.SetPreviewMeshIndex( i );
                        hasPreview = true;
                    }



                    int num = (int)Mathf.Repeat( i, vButtons );

                    row++;
                    if (num == vButtons - 1)
                    {
                        row = 0;
                        colum++;
                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal();
                    }
                }
                if (!hasPreview)
                    tgt.SetPreviewMeshIndex( -1 );

            }




            int scrollerHight = colum * (buttonHight + 2) + buttonHight + 2;
            Rect drop_area = new Rect( 0, 0, width, scrollerHight );
            switch (curEvent.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!drop_area.Contains( curEvent.mousePosition ))
                        return;
                    bool acceptDrag = true;
                    foreach (UnityObject dragged_object in DragAndDrop.objectReferences)
                    {
                        var go = dragged_object as GameObject;
                        var pg = dragged_object as PropGroup;
                        if (go == null && pg == null)
                            acceptDrag = false;
                        else if (go != null && !BHUtilities.IsPrefab( go ))
                            acceptDrag = false;
                        if (!acceptDrag)
                            break;
                    }

                    if (acceptDrag)
                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                    else
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;

                    if (curEvent.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        doubleSamePrefabWarning = false;


                        foreach (UnityObject obj in targets)
                        {
                            if (obj is RowPropPlacer)
                            {
                                SerializedObject curObject = new SerializedObject( (RowPropPlacer)obj );

                                foreach (UnityObject dragged_object in DragAndDrop.objectReferences)
                                {
                                    dragAndDropInfo = false;

                                    if (dragged_object is GameObject)
                                    {
                                        if (BHUtilities.ArrayContainsPrefab( curObject.FindProperty( prefabArrayProp.name ), dragged_object ))
                                        {
                                            doubleSamePrefabWarning = true;
                                            continue;
                                        }
                                        BHUtilities.ArrayAdd( curObject.FindProperty(prefabArrayProp.name), dragged_object );
                                    }
                                    else if (dragged_object is PropGroup)
                                    {
                                        if (BHUtilities.ArrayContainsPrefab( curObject.FindProperty( prefabGroupArrayProp.name ), dragged_object ))
                                        {
                                            doubleSamePrefabWarning = true;
                                            continue;
                                        }
                                        BHUtilities.ArrayAdd( curObject.FindProperty( prefabGroupArrayProp.name), dragged_object );
                                       
                                    }
                                }
                            }
                        }
                    }
                    break;
            }

            GUILayout.BeginVertical();
            GUILayout.Space( scrollerHight );
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
        }



        static Texture2D GetIcon( GameObject asset, int width, int hight)
        {
            
            int key = asset.GetInstanceID();
            if (cachedIcons.ContainsKey(key))
            {
                return cachedIcons[key];
            }

            Vector3 camDir = Vector3.one * -1;
            if (asset.TryGetComponent<RowPropPlacer>(out RowPropPlacer rpp))
            {
                Vector3 fwd = Vector3.forward;
                camDir = rpp.OriginPivotLocalRotation *  -Vector3.one;
            }
            RuntimePreviewGenerator.BackgroundColor = Color.clear;
            RuntimePreviewGenerator.PreviewDirection = camDir;
            RuntimePreviewGenerator.OrthographicMode = true;
            RuntimePreviewGenerator.Padding = 0;
            Texture2D icon = RuntimePreviewGenerator.GenerateModelPreview(asset.transform, width, hight);
            cachedIcons.Add(key, icon);

            return icon;
        }




        void OnSelectionChanged()
        {
            RowPropPlacer tgt = (RowPropPlacer)target;
            if (tgt == null || !Selection.Contains( tgt.gameObject ))
                Selection.selectionChanged -= OnSelectionChanged;
            if (tgt != null)
            {
                tgt.editSpawnPivot = false;
                tgt.ResetPreview();
            }

            Tools.hidden = false;
        }












        bool mouseIsDownInSceneView = false;
        protected virtual void OnSceneGUI()
        {
            
            if (curRowPropPlacerTarget == null)
                curRowPropPlacerTarget = (RowPropPlacer)target;
            if (BuildingUnitEditor.ScenePickingMode( (BuildingUnit)curRowPropPlacerTarget ))
                return;

            Event evt = Event.current;
            int controlID = GUIUtility.GetControlID( GetHashCode(), FocusType.Passive );
            EventType eventType = evt.GetTypeForControl( controlID );

            if(eventType == EventType.MouseDown && evt.button == 0)
            {
                mouseIsDownInSceneView = true;
            }
            else if (eventType == EventType.MouseUp && evt.button == 0)
            {
                mouseIsDownInSceneView = false;
            }
            
            PivotEditor( evt );
        }

        List<Vector3> boundPositions;
        void PivotEditor(Event evt)
        {
            if (!curRowPropPlacerTarget.editSpawnPivot)
                return;

            Handles.DrawLine( curRowPropPlacerTarget.SpawnPivotWorldPosition, curRowPropPlacerTarget.OriginPivotWorldPosition );

            bool editingOrigin = curPivotEditTarget == PivotEditTarget.originRotation || curPivotEditTarget == PivotEditTarget.originOffset;


            Handles.Label( editingOrigin? curRowPropPlacerTarget.OriginPivotWorldPosition : curRowPropPlacerTarget.SpawnPivotWorldPosition, editingOrigin ?  "Origin Pivot": "NextObjectPivot" );

            EditorGUI.BeginChangeCheck();
            if (curPivotEditTarget == PivotEditTarget.spawnRotation)
            {
                EditorGUI.BeginChangeCheck();
                Quaternion rot = Handles.RotationHandle( curRowPropPlacerTarget.SpawnPivotWorldRotation, curRowPropPlacerTarget.SpawnPivotWorldPosition );
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject( target, "Rotate pivot" );
                    curRowPropPlacerTarget.SpawnPivotWorldRotation = rot;
                    EditorUtility.SetDirty( curRowPropPlacerTarget );
                }
                return;
            }
            if (curPivotEditTarget == PivotEditTarget.originRotation)
            {
                EditorGUI.BeginChangeCheck();
                Quaternion rot = Handles.RotationHandle( curRowPropPlacerTarget.OriginPivotWorldRotation, curRowPropPlacerTarget.OriginPivotWorldPosition );
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject( target, "Rotate pivot" );
                    curRowPropPlacerTarget.OriginPivotWorldRotation = rot;
                    EditorUtility.SetDirty( curRowPropPlacerTarget );
                }
                return;
            }

            bool editOriginOffset = curPivotEditTarget == PivotEditTarget.originOffset;
            Vector3 curTgtPosition = editOriginOffset ? curRowPropPlacerTarget.OriginPivotWorldPosition : curRowPropPlacerTarget.SpawnPivotWorldPosition;
            Vector3 newTargetPosition = Handles.PositionHandle( curTgtPosition, editOriginOffset ? curRowPropPlacerTarget.transform.rotation : curRowPropPlacerTarget.SpawnPivotWorldRotation );

            if (AxisConstrainedVertexSnap)
            {
                if (boundPositions == null)
                    boundPositions = BHUtilities.GetBoundPoints( curRowPropPlacerTarget.gameObject);
                if (mouseIsDownInSceneView && BHUtilities.FindNearestVertex( evt.mousePosition, curRowPropPlacerTarget.gameObject, out Vector3 detectedVertex, boundPositions ))
                {
                    Vector3 moveDirection = curRowPropPlacerTarget.transform.InverseTransformDirection( newTargetPosition - curTgtPosition );
                    bool moveAxis_x = Mathf.Abs( moveDirection.x ) > 0.001;
                    bool moveAxis_y = Mathf.Abs( moveDirection.y ) > 0.001;
                    bool moveAxis_z = Mathf.Abs( moveDirection.z ) > 0.001;
                    newTargetPosition = BHUtilities.AllignOnTransformAxis( curRowPropPlacerTarget.transform, newTargetPosition, detectedVertex, moveAxis_x, moveAxis_y, moveAxis_z );

                    Handles.DrawLine( detectedVertex, newTargetPosition );
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject( curRowPropPlacerTarget, "Move pivot" );
                if (editOriginOffset)
                {
                    curRowPropPlacerTarget.OriginPivotWorldPosition = newTargetPosition;
                }
                else
                    curRowPropPlacerTarget.SpawnPivotWorldPosition = newTargetPosition;
                EditorUtility.SetDirty( curRowPropPlacerTarget );
            }
        }

      

        void Spawn(GameObject go, RowPropPlacer spwn)
        {


            if (spwn == null || go == null)
                return;
            if (spwn.operatorType == RowPropPlacer.Operator.Normal)
                SpawnNext( go, spwn );
            else if (spwn.operatorType == RowPropPlacer.Operator.Reverse)
                SpawnReverse( go, spwn );
            else if (spwn.operatorType == RowPropPlacer.Operator.Turnaround)
                SpawnTurnaround( go, spwn );
            else if (spwn.operatorType == RowPropPlacer.Operator.ReverseTurnaround)
                SpawnReverseTurnaround( go, spwn );

            EditorUtility.SetDirty( spwn );
        }



        static GameObject InstantiateProp(GameObject go, RowPropPlacer spwn)
        {
            GameObject newGo = null;

            newGo = (GameObject)PrefabUtility.InstantiatePrefab(PrefabUtility.GetCorrespondingObjectFromSourceAtPath(go, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(go)));

            if (newGo == null)
            {
                newGo = Instantiate( go );
            }
            if (newGo.TryGetComponent<RowPropPlacer>( out RowPropPlacer rpp ))
            {
                rpp.TransferSettings( spwn.prefabGroups, spwn.groupMask, spwn.prefabViewscrollPosition);
            }

           

            Selection.activeObject = newGo.gameObject;
            newGo.transform.SetParent( spwn.transform.parent );
            newGo.transform.localScale = spwn.transform.localScale;
            Undo.RegisterCreatedObjectUndo(newGo, "Instantiate prefab");
            return newGo;
        }


        static GameObject SpawnNext(GameObject go, RowPropPlacer spwn)
        {

            var newGo = InstantiateProp( go, spwn );
        
            if (newGo.TryGetComponent<OriginOffsetRPP>( out OriginOffsetRPP ofrpp ))
            {
                RowPropPlacer.SetPositionFromParentToChild( spwn, ofrpp);
                spwn.AddChild(ofrpp);
                return newGo;
            }
            else if (newGo.TryGetComponent<RowPropPlacer>(out RowPropPlacer rpp))
            {
                RowPropPlacer.SetPositionFromParentToChild(spwn, rpp);
                spwn.AddChild(rpp);
                return newGo;
            }


            newGo.transform.SetPositionAndRotation( spwn.SpawnPivotWorldPosition, spwn.SpawnPivotWorldRotation );
            BuildingUnit bu;
            if (!newGo.TryGetComponent<BuildingUnit>( out bu ))
                bu = newGo.AddComponent<BuildingUnit>();

            spwn.AddChild( bu );
            
            return newGo;
        }


        static void SpawnReverseTurnaround(GameObject go, RowPropPlacer spwn)
        {
            var newGo = InstantiateProp( go, spwn );
            RowPropPlacer.SetTransformReverseTurnaround( newGo.transform, spwn );
        }

        static void SpawnTurnaround(GameObject go, RowPropPlacer spwn)
        {
            var newGo = InstantiateProp( go, spwn );
            RowPropPlacer.SetTransformTurnaround( newGo.transform, spwn );
            if (newGo.TryGetComponent(out RowPropPlacer rpp ))
            {
                rpp.operatorType = RowPropPlacer.Operator.Reverse;
            }
        }


        static void SpawnReverse(GameObject go, RowPropPlacer spwn)
        {

            var newGo = InstantiateProp( go, spwn );
            newGo.transform.SetPositionAndRotation( spwn.transform.position, spwn.transform.rotation );



            BuildingUnit bu;

            if (newGo.TryGetComponent<OriginOffsetRPP>(out OriginOffsetRPP ofrpp))
            {
                bu = (BuildingUnit)ofrpp;
            }
            else if (!newGo.TryGetComponent<BuildingUnit>( out bu ))
                bu = newGo.AddComponent<BuildingUnit>();
           

                
            if (spwn.pseudoParent != null)
            {
                spwn.pseudoParent.AddChild( bu );
            }
            if (bu.GetType() == typeof( RowPropPlacer )|| bu.GetType() == typeof(OriginOffsetRPP))
            {
                RowPropPlacer rpp = (RowPropPlacer)bu;
                RowPropPlacer.SetPositionFromChildToParent( spwn, rpp );
                rpp.operatorType = RowPropPlacer.Operator.Reverse;
            }
            bu.AddChild( (BuildingUnit)spwn );
            

        }





        public class SpwanerArrayInfo : EditorWindow
        {
            // Add menu item
            [MenuItem( "CONTEXT/SpawnerEditor/Remove from List" )]
            static void RemoveFromArray(MenuCommand command)
            {
                BHUtilities.ArrayRemoveAt(RowPropPlacer.curArray, RowPropPlacer.curIndex );
            }
            [MenuItem( "CONTEXT/SpawnerEditor/Select in Project" )]
            static void SelectInProject(MenuCommand command)
            {

                EditorGUIUtility.PingObject( RowPropPlacer.curArray.GetArrayElementAtIndex( RowPropPlacer.curIndex ).objectReferenceValue );
            }

        }
    }



    }



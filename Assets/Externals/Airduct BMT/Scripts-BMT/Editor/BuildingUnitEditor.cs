using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
namespace BuildingMakerToolset.PropPlacer {
    [CustomEditor( typeof(BuildingUnit))]
    public class BuildingUnitEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawBuildingUnitEditor( target as BuildingUnit );
        }
        static bool foldout = false;
        static bool foldoutChildren = false;


        public static void DrawBuildingUnitEditor(BuildingUnit unit)
        {
            
            
            foldout = EditorGUILayout.Foldout( foldout, "Transform Awareness" );
            if (!foldout)
                return;
            EditorGUIUtility.labelWidth = 210;
            GUIContent tmpContent = new GUIContent();
            EditorGUI.indentLevel = 1;
            SerializedObject serializedObject = new SerializedObject( unit );
            serializedObject.UpdateIfRequiredOrScript();
            SerializedProperty enableTransformationInheritance = serializedObject.FindProperty( "enableTransformationInheritance" );
            SerializedProperty parent = serializedObject.FindProperty( "pseudoParent" );
            SerializedProperty children = serializedObject.FindProperty( "pseudoChildren" );
            GUIContent pickerButton = new GUIContent();
            EditorGUI.BeginChangeCheck();
            

            tmpContent.text = "Enable Transform Awareness";
            EditorGUILayout.PropertyField( enableTransformationInheritance, tmpContent );
            enableTransformationInheritance.

            serializedObject.ApplyModifiedProperties();
            if (unit.enableTransformationInheritance)
            {
                EditorGUI.indentLevel = 2;
                serializedObject.ApplyModifiedProperties();


                Object curParent = unit.pseudoParent;
                
                Object newParent = curParent;
                EditorGUILayout.BeginHorizontal();
                pickerButton.image = EditorGUIUtility.IconContent( "eyeDropper.Large" ).image as Texture2D;
                pickerButton.tooltip = "pick object from scene";
                EditorGUIUtility.labelWidth = 1;
                if (GUILayout.Button( pickerButton, GUILayout.Width(20), GUILayout.Height(20), GUILayout.ExpandWidth(false) ))
                {
                    StartPicker( unit , -1);
                }
                newParent = EditorGUILayout.ObjectField( "Inherit from", newParent, typeof( BuildingUnit ), true );
                EditorGUI.BeginDisabledGroup( newParent == null );
                if (GUILayout.Button( "Remove" ))
                {
                    newParent = null;
                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.EndHorizontal();
                if (newParent != curParent)
                {
                    EditorUtility.SetDirty( unit );
                    if (curParent != null)
                    {
                        BuildingUnit par = curParent as BuildingUnit;
                        par.RemoveChild( unit );
                    }
                    if (newParent != null)
                    {
                        BuildingUnit par = newParent as BuildingUnit;
                        par.AddChild( unit );
                    }
                }
                if (EditorGUI.EndChangeCheck())
                {
                    unit.forceTransformChanged = true;
                }



                foldoutChildren = EditorGUILayout.Foldout( foldoutChildren, "Pass to:" );
                if (!foldoutChildren)
                {
                    DrawPseudoChildren( unit );
                    if (GUILayout.Button( "Add" ))
                    {
                        unit.pseudoChildren.Add(null);
                    }
                }


            

            }
            EditorGUI.indentLevel = 0;
        }

        static void DrawPseudoChildren(BuildingUnit unit)
        {
            EditorGUI.indentLevel = 3;
            if (unit.pseudoChildren == null || unit.pseudoChildren.Count == 0)
                return;
            GUIContent tmpGuiContent = new GUIContent();
            tmpGuiContent.image = EditorGUIUtility.IconContent( "eyeDropper.Large" ).image as Texture2D;
            tmpGuiContent.tooltip = "pick object from scene";
            bool breakOut = false;
            for (int i = 0; i < unit.pseudoChildren.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
               
                GUILayout.Space( EditorGUI.indentLevel * 15 );
                EditorGUIUtility.labelWidth = 1;
                if (GUILayout.Button( tmpGuiContent, GUILayout.Width( 20 ), GUILayout.Height( 20 ), GUILayout.ExpandWidth( false ) ))
                {
                    StartPicker( unit, i );
                }

                Object curChild = unit.pseudoChildren[i];
                Object newChild = EditorGUILayout.ObjectField( "child_" + i, curChild, typeof( BuildingUnit ), true );
                if (newChild != curChild)
                {
                    if (curChild != null)
                    {
                        unit.RemoveChild( curChild as BuildingUnit );
                    }
                    if (newChild != null)
                    {
                        unit.AddChild( newChild as BuildingUnit, i );
                    }
                    
                    EditorUtility.SetDirty( unit );
                }
                else if (GUILayout.Button( "Remove" ))
                {
                    if (curChild != null)
                    {
                        unit.RemoveChild( curChild as BuildingUnit );
                    }
                    else
                        unit.pseudoChildren.RemoveAt(i);
                    breakOut = true;
                }
                EditorGUILayout.EndHorizontal();
                if (breakOut)
                    break;
            }
            
        }

        static bool DrawObjectField(Object obj)
        {
            Object tmp = EditorGUILayout.ObjectField( "Transformation inherit from", obj, typeof( BuildingUnit ), true );
            if (tmp != obj)
            {
                obj = tmp;
                return true;
            }
            return false;
        }



        static bool picking = false;
        static bool sceneDirty = false;
        static BuildingUnit pickingForTarget;
        static int pickedToTarget;
        static void StartPicker(BuildingUnit target, int pickedToTgt)
        {
            if (picking)
            {
                StopPicker();
                return;
            }
            pickedToTarget = pickedToTgt;
            pickingForTarget = target;
            picking = true;
            Tools.hidden = true;
        }
        static void StopPicker()
        {
            pickingForTarget = null;
            picking = false;
            Tools.hidden = false;
        }
        
        private void OnSceneGUI()
        {
            ScenePickingMode( target as BuildingUnit );
        }
        
        public static bool ScenePickingMode(BuildingUnit tgtUnit)
        {
            if (!picking)
                return false;
            if(tgtUnit!= pickingForTarget)
            {
                return false;
            }

            
            Event guiEvent = Event.current;
            if (guiEvent.type == EventType.Repaint)
            {
                Vector3 fromPos = pickingForTarget.transform.position;
                if(tgtUnit.GetType() == typeof( RowPropPlacer ))
                {
                    RowPropPlacer rpp = tgtUnit as RowPropPlacer;
                    fromPos = pickedToTarget!=-1 ? rpp.SpawnPivotWorldPosition : rpp.OriginPivotWorldPosition;
                }

                Handles.DrawDottedLine( fromPos, HandleUtility.GUIPointToWorldRay( Event.current.mousePosition ).GetPoint(1), 3 );
                EditorGUIUtility.AddCursorRect( new Rect( SceneView.lastActiveSceneView.position ), MouseCursor.Link );
            }

            if (guiEvent.type == EventType.Layout)
            {
                HandleUtility.AddDefaultControl( GUIUtility.GetControlID( FocusType.Passive ) );
            }
            else
            {
                HandleInput( guiEvent );
                if (sceneDirty)
                {
                    HandleUtility.Repaint();
                }
            }
            return true;
        }

        static void HandleInput(Event guiEvent)
        {
            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
            {
                PickObject( pickingForTarget, pickedToTarget );
            }

            if(guiEvent.type == EventType.MouseDown && guiEvent.button == 1)
            {
                StopPicker();
            }
        }

        static void PickObject(BuildingUnit forTarget, int p2t)
        {
           
            List<BuildingUnit> ignoreList =  new List<BuildingUnit>(forTarget.pseudoChildren);
            ignoreList.Add( forTarget );
           
            GameObject picked = HandleUtility.PickGameObject( Event.current.mousePosition, true, ignoreList.Where(x=> x!=null).Select(x=>x.gameObject).ToArray());
            
            if (picked == null)
                return;
            if (picked.TryGetComponent( out BuildingUnit pickedBu ))
            {
                Vector3 curPos = forTarget.transform.position;
                Quaternion curRotation = forTarget.transform.rotation;
                if (p2t == -1)
                    pickedBu.AddChild( forTarget );
                else
                {
                    forTarget.AddChild( pickedBu, p2t );
                }
                forTarget.transform.position = curPos;
                forTarget.transform.rotation = curRotation;

                StopPicker();

            }
        }
    }
}

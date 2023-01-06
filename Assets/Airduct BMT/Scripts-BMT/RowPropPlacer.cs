using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using System.Linq;
using UnityEditor;
namespace BuildingMakerToolset.PropPlacer
{
    /// <summary>
    /// This tool helps placing props such as walls, pipes, railings etc.
    /// Place props by clicking associated buttons in the inspector.
    /// The position, rotation and scale of the new prop is that of this.transform plus the offset determined by spawnPivot.
    /// </summary>
    [SelectionBase]
    public class RowPropPlacer : BuildingUnit
    {
#if UNITY_EDITOR
        public static SerializedProperty curArray;
        public static int curIndex;
        [System.NonSerialized]
        public bool drawPreview;
        [System.NonSerialized]
        public bool editSpawnPivot;
        [System.NonSerialized]
        public int pivotPreviewMesh;


        [System.NonSerialized]
        public Vector2 prefabViewscrollPosition;
        [System.NonSerialized]
        public bool[] groupMask;
        [System.NonSerialized]
        bool skipMaskResetOnStart = false;

        public SpawnPivot spawnPivot;
        public GameObject[] prefabs;
        public PropGroup[] prefabGroups;
        [System.NonSerialized]
        public GameObject[] combinedPrefabGroup;




        [System.NonSerialized]
        public GameObject generatedPreviewGo;
        MeshFilter[] generatedPreviewRenederers;
        GameObject curPreviewPrefab;
        GameObject lastPreviewReference;
        public enum Operator
        {
            Normal, Reverse, Turnaround ,ReverseTurnaround
        }
        [System.NonSerialized]
        public Operator operatorType;





        [System.Serializable]
        public struct SpawnPivot
        {
            public Vector3 originOffset;

            public Vector3 originRotation;

            public Vector3 pivPosition;

            public Vector3 pivRotation;



            public Vector3 SpawnWorldPositionGet(RowPropPlacer parent)
            {
                return TranslateToWorldPosition( pivPosition + originOffset, parent.transform );
            }

            public void SpawnWorldPositionSet(Vector3 newPosition, RowPropPlacer parent)
            {
                float scaleFactor = BHUtilities.FindMeshScaleFactor( parent.transform );
                pivPosition = (parent.transform.InverseTransformPoint( newPosition ) - originOffset * scaleFactor) / scaleFactor;
            }


            public Quaternion SpawnLocalRotationGet(RowPropPlacer parent)
            {
                return Quaternion.Normalize( Quaternion.Euler( pivRotation ) * Quaternion.Inverse( Quaternion.Euler( originRotation ) ) );
            }


            public Quaternion SpawnWorldRotationGet(RowPropPlacer parent)
            {
                return Quaternion.Normalize( parent.transform.rotation * SpawnLocalRotationGet( parent ) );
            }

            public void SpawnWorldRotationSet(Quaternion newRotation, RowPropPlacer parent)
            {
                pivRotation = BHUtilities.ClampToRotation( Quaternion.Normalize( Quaternion.Inverse( parent.transform.rotation ) * newRotation ).eulerAngles );
            }

            public void OriginOverrideSet(Vector3 newPosition, RowPropPlacer parent)
            {
                originOffset = parent.transform.InverseTransformPoint( newPosition ) / BHUtilities.FindMeshScaleFactor( parent.transform );
            }

            public Vector3 OriginOverrideGet(RowPropPlacer parent)
            {
                return TranslateToWorldPosition( originOffset, parent.transform );
            }

            public Quaternion OriginLocalRotationGet(RowPropPlacer parent)
            {
                return Quaternion.Normalize( Quaternion.Euler( originRotation ) );
            }
            public Quaternion OriginWorldRotationGet(RowPropPlacer parent)
            {
                return Quaternion.Normalize( parent.transform.rotation * Quaternion.Euler( originRotation ));
            }
            public void OriginWorldRotationSet(Quaternion newRotation, RowPropPlacer parent)
            {
                originRotation = BHUtilities.ClampToRotation( Quaternion.Normalize( Quaternion.Inverse( parent.transform.rotation ) * newRotation ).eulerAngles );
            }

          

        }


        public Vector3 OriginWorldOffset{get { return (transform.position -spawnPivot.OriginOverrideGet( this )); } }

        public Vector3 OriginPivotWorldPosition { get { return spawnPivot.OriginOverrideGet( this ); } set { spawnPivot.OriginOverrideSet( value, this ); forceTransformChanged = true; ResetPreview(); } }

        public Quaternion OriginPivotLocalRotation { get { return spawnPivot.OriginLocalRotationGet( this ); } }

        public Quaternion OriginPivotWorldRotation { get { return spawnPivot.OriginWorldRotationGet( this ); } set { spawnPivot.OriginWorldRotationSet( value, this ); forceTransformChanged = true; ResetPreview(); } }


        public Vector3 SpawnPivotWorldPosition { get { return spawnPivot.SpawnWorldPositionGet( this ); } set { spawnPivot.SpawnWorldPositionSet( value, this ); forceTransformChanged = true; } }

        public Quaternion SpawnPivotWorldRotation { get { return spawnPivot.SpawnWorldRotationGet( this ); } set { spawnPivot.SpawnWorldRotationSet( value, this ); forceTransformChanged = true; } }

        public Quaternion SpawnPivotLocalRotation { get { return spawnPivot.SpawnLocalRotationGet( this ); } }


        public void TransferSettings(PropGroup[] fromGroups, bool[] fromMask, Vector2 scrollPos)
        {
            UpdateGroupMask();
            if (prefabGroups.Length == 0 || fromGroups == null || fromMask == null || fromGroups.Length == 0 || fromGroups.Length != fromMask.Length)
                return;
            prefabViewscrollPosition = scrollPos;
            skipMaskResetOnStart = true;
            for (int i = 0; i< fromGroups.Length; i++)
            {
                for(int j = 0; j < prefabGroups.Length; j++)
                {
                    if(fromGroups[i]== prefabGroups[j])
                    {
                        groupMask[j] = fromMask[i];
                    }
                }
                
            }
        }
       
        public void UpdateGroupMask()
        {
            if (prefabGroups == null)
                return;
         
            if (groupMask == null || groupMask.Length != prefabGroups.Length)
            {
                groupMask = new bool[prefabGroups.Length];
                for (int i = 0; i < groupMask.Length; i++)
                    groupMask[i] = true;
            }
        }

        public void ResetOnInpectorEnable()
        {
            if (prefabGroups == null)
                return;

            if (skipMaskResetOnStart)
            {
                skipMaskResetOnStart = false;
                return;
            }
            UpdateGroupMask();
            for (int i = 0; i < groupMask.Length; i++)
                groupMask[i] = true;
        }




        public void UpdateCombinedPrefabGroup()
        {
            int combinedLength = 0;

            if (prefabGroups == null || prefabGroups.Length == 0)
                combinedLength = 0;
            else
            {
                UpdateGroupMask();
                for (int i = 0; i< prefabGroups.Length; i++)
                {
                    if (groupMask[i] && prefabGroups[i] != null && prefabGroups[i].prefabs != null)
                        combinedLength += prefabGroups[i].prefabs.Length;
                }
            }
            if (combinedLength == 0)
            {
                combinedPrefabGroup = null;
                return;
            }
            if (combinedPrefabGroup == null || combinedPrefabGroup.Length != combinedLength)
                combinedPrefabGroup = new GameObject[combinedLength];

            int curGroup = 0;
            int lastGroupCut = 0;
            for(int i = 0; i< combinedPrefabGroup.Length; i++)
            {
                while (curGroup < groupMask.Length && (!groupMask[curGroup]|| prefabGroups[curGroup] == null))
                {
                    curGroup++;
                }

                int curPrefabIndexInGroup = i - lastGroupCut;
                combinedPrefabGroup[i] = prefabGroups[curGroup].prefabs[curPrefabIndexInGroup];

                if (curPrefabIndexInGroup +1 == prefabGroups[curGroup].prefabs.Length)
                {
                    curGroup++;
                    lastGroupCut = i + 1;
                }
                
            }
        }






        public int GetTotalPrefabLength()
        {
            int thisPrefabCount = 0;
            if (prefabs != null)
                thisPrefabCount = prefabs.Length;

            thisPrefabCount += GetPrefabGroupLength();
            return thisPrefabCount;

        }






        public int GetPrefabGroupLength()
        {
            UpdateCombinedPrefabGroup();
            if (combinedPrefabGroup == null)
                return 0;
            return combinedPrefabGroup.Length;
        }




        public override void OnPassOnTransformationToChild(BuildingUnit child)
        {
            if (!this.enabled)
                return;
            if (child.transform.IsChildOf( transform ))
            {
                base.OnPassOnTransformationToChild( child );
                return;
            }
            SetPositionFromParentToChild( this, child );
        }




        public override void OnInheritChildTransformation(BuildingUnit child)
        {
            if (!this.enabled)
                return;
            if (child.transform.IsChildOf( transform ))
            {
                base.OnInheritChildTransformation( child );
                return;
            }

            SetPositionFromChildToParent( child, this );
        }

        public static void SetPositionFromParentToChild(RowPropPlacer parent, BuildingUnit child)
        {

            if (child.GetType() == typeof( RowPropPlacer) || child.GetType() == typeof(OriginOffsetRPP))
            {
                RowPropPlacer rpp = (RowPropPlacer)child;
                rpp.transform.rotation =  parent.SpawnPivotWorldRotation  * rpp.OriginPivotLocalRotation;
                rpp.transform.position = parent.SpawnPivotWorldPosition + rpp.OriginWorldOffset;

            }
            else
            {
                child.transform.SetPositionAndRotation( parent.SpawnPivotWorldPosition, parent.SpawnPivotWorldRotation );
            }
        }


        public static void SetPositionFromChildToParent(BuildingUnit child, RowPropPlacer parent)
        {
            

            if (child.GetType() == typeof( RowPropPlacer ) || child.GetType() == typeof(OriginOffsetRPP))
            {
                
                RowPropPlacer rpp = (RowPropPlacer)child;
                parent.transform.rotation =  child.transform.rotation;
                parent.transform.rotation *= Quaternion.Inverse( parent.SpawnPivotWorldRotation * rpp.OriginPivotLocalRotation ) * child.transform.rotation;
                parent.transform.position = child.transform.position;
                parent.transform.position += (parent.transform.position - parent.SpawnPivotWorldPosition);
                parent.transform.position -= rpp.OriginWorldOffset;
            }
            else
            {
                parent.transform.rotation = child.transform.rotation;
                parent.transform.rotation *= Quaternion.Inverse( parent.SpawnPivotWorldRotation ) * child.transform.rotation;
                parent.transform.rotation = parent.transform.rotation.normalized;
                parent.transform.position = child.transform.position;
                parent.transform.position += (parent.transform.position - parent.SpawnPivotWorldPosition);
            }
        }



        public static void SetTransformReverseTurnaround(Transform child, RowPropPlacer orig)
        {
           


           
            if(child.gameObject.TryGetComponent( out RowPropPlacer childRPP ))
            {
                child.transform.SetPositionAndRotation( orig.OriginPivotWorldPosition, Quaternion.AngleAxis( 180, BHUtilities.ClosestTransformAxisToAxis( orig.transform, Vector3.up ) ) * orig.transform.rotation * Quaternion.Inverse(orig.OriginPivotLocalRotation) * childRPP.OriginPivotLocalRotation );
                child.transform.position += childRPP.OriginWorldOffset;
            }
            else
            {
                child.transform.SetPositionAndRotation( orig.OriginPivotWorldPosition, Quaternion.AngleAxis( 180, BHUtilities.ClosestTransformAxisToAxis( orig.transform, Vector3.up ) ) *  orig.transform.rotation * Quaternion.Inverse( orig.OriginPivotLocalRotation )  );
            }
           
        }


        public static void SetTransformTurnaround(Transform child, RowPropPlacer orig)
        {

            if (child.gameObject.TryGetComponent( out RowPropPlacer childRPP ))
            {
                child.transform.SetPositionAndRotation( orig.SpawnPivotWorldPosition, Quaternion.AngleAxis( 180, BHUtilities.ClosestTransformAxisToAxis( orig.transform, Vector3.up ) ) * orig.transform.rotation * Quaternion.Inverse( orig.OriginPivotLocalRotation ) * childRPP.OriginPivotLocalRotation * childRPP.SpawnPivotLocalRotation );
                child.transform.position += childRPP.OriginWorldOffset;
                child.transform.position += child.transform.position - childRPP.SpawnPivotWorldPosition;
            }
            else
            {
                child.transform.SetPositionAndRotation( orig.SpawnPivotWorldPosition, Quaternion.AngleAxis( 180, BHUtilities.ClosestTransformAxisToAxis( orig.transform, Vector3.up ) ) * orig.transform.rotation * Quaternion.Inverse( orig.OriginPivotLocalRotation ) );
            }

        }

        private void OnDrawGizmosSelected()
        {
            Color oldCol = Gizmos.color;
            if (Selection.activeObject != this && Selection.activeObject != this.gameObject || Selection.gameObjects.Length > 1)
                return;

            if (drawPreview && curPreviewPrefab != null)
                DrawPreviewMeshGizmo( curPreviewPrefab );
            else if (editSpawnPivot)
                DrawPivotPreview();
            else if (generatedPreviewGo != null)
                ResetPreview();

            Gizmos.color = oldCol;
        }





        public void DrawPivotPreview()
        {
            GameObject tgtPreview = null;
            if (prefabs != null && prefabs.Length > 0 && prefabs[0] != null)
            {
                tgtPreview = prefabs[0];
            }
            else if (combinedPrefabGroup != null && combinedPrefabGroup.Length > 0 && combinedPrefabGroup[0]!=null)
            {
                tgtPreview = combinedPrefabGroup[0];
            }
            


            if(tgtPreview == null)
            {
                DrawPreviewMeshGizmo( gameObject );
            }
            else
            {
                DrawPreviewMeshGizmo( tgtPreview );
            }
        }







        public void ResetPreview()
        {
            /* // causes crash with HDRP 
            if (generatedPreviewGo != null)
                DestroyImmediate( generatedPreviewGo );
            */
            
            if(generatedPreviewGo!=null)
                EditorUtility.SetDirty(generatedPreviewGo);
            
            forceTransformChanged = true;
            generatedPreviewGo = null;
            lastPreviewReference = null;
            generatedPreviewRenederers = null;
           // SceneView.RepaintAll();
        }





        

        public void SetPreviewMeshIndex(int index)
        {
           

            if (index == -1)
            {
                drawPreview = false;
                curPreviewPrefab = null;
                SceneView.RepaintAll();
                return;
            }

            drawPreview = true;
            pivotPreviewMesh = index;
            if (index >= GetPrefabGroupLength())
                curPreviewPrefab = prefabs[index - GetPrefabGroupLength()];
            else
                curPreviewPrefab = combinedPrefabGroup[index];
            SceneView.RepaintAll();
        }





        void DrawPreviewMeshGizmo(GameObject reference)
        {
            if (reference != lastPreviewReference)
            {
                ResetPreview();
                lastPreviewReference = reference;
                if (reference == null)
                {
                    return;
                }
                generatedPreviewGo = Instantiate( reference );
                generatedPreviewGo.name = "preview";
                generatedPreviewRenederers = generatedPreviewGo.GetComponentsInChildren<MeshFilter>();
                PreviewObjectTracker tracker = generatedPreviewGo.AddComponent<PreviewObjectTracker>();
                generatedPreviewGo.transform.SetParent( transform.parent );
                generatedPreviewGo.transform.localScale = transform.localScale;
                tracker.owner = this;
            }
            if (generatedPreviewGo == null)
                return;


  
            RowPropPlacer tmpPlacer = null;
            if(generatedPreviewGo.TryGetComponent(out OriginOffsetRPP ororpp))
            {
                tmpPlacer = (RowPropPlacer)ororpp;
            }
            else
            {
                tmpPlacer = generatedPreviewGo.GetComponent<RowPropPlacer>();
            }
            

            
            if (operatorType == Operator.Normal)
            {
                if (tmpPlacer != null)
                    SetPositionFromParentToChild( this, tmpPlacer );
                else
                    generatedPreviewGo.transform.SetPositionAndRotation( SpawnPivotWorldPosition, SpawnPivotWorldRotation );
            }
            else if (operatorType == Operator.Reverse && tmpPlacer!=null)
                SetPositionFromChildToParent( this, tmpPlacer );
            else if (operatorType == Operator.Turnaround)
                SetTransformTurnaround( generatedPreviewGo.transform, this );
            else if (operatorType == Operator.ReverseTurnaround)
                SetTransformReverseTurnaround( generatedPreviewGo.transform, this );
            else
                generatedPreviewGo.transform.SetPositionAndRotation( transform.position, transform.rotation );
        
            generatedPreviewGo.transform.SetParent( null );
            Transform tmpTransform;

            
            if (tmpPlacer!=null)
            {
                Gizmos.color = Color.green;
                BHUtilities.ArrowGizmo( tmpPlacer.SpawnPivotWorldPosition, tmpPlacer.SpawnPivotWorldRotation * Vector3.up *0.1f);
                Gizmos.color = Color.blue;
                BHUtilities.ArrowGizmo( tmpPlacer.SpawnPivotWorldPosition, tmpPlacer.SpawnPivotWorldRotation * Vector3.forward * 0.1f );
                Gizmos.color = Color.red;
                BHUtilities.ArrowGizmo( tmpPlacer.SpawnPivotWorldPosition, tmpPlacer.SpawnPivotWorldRotation * Vector3.right * 0.1f );
            }




         

            for (int i = 0; i < generatedPreviewRenederers.Length; i++)
            {
                if (generatedPreviewRenederers[i].sharedMesh == null)
                    continue;
                tmpTransform = generatedPreviewRenederers[i].GetComponent<Transform>();
                Gizmos.color = Color.green;
                Gizmos.DrawWireMesh( generatedPreviewRenederers[i].sharedMesh, generatedPreviewRenederers[i].transform.position, generatedPreviewRenederers[i].transform.rotation, generatedPreviewRenederers[i].transform.lossyScale );
            }
        }



        [ContextMenu( "Select Neighbors" )]
        public void SelectNeighbours()
        {
            Selection.objects = GetNeighborsWithScript<RowPropPlacer>().Select( x => x.gameObject ).ToArray();
        }



        [ContextMenu( "Repair Transform Awareness" )]
        public void RepairTransformAwareness()
        {
            List<RowPropPlacer> neighbours = GetNeighborsWithScript<RowPropPlacer>();
            for(int i = 0; i< neighbours.Count-1; i++)
            {
                neighbours[i].AddChild( neighbours[i+1] );
            }
        }
       
 

        public List<RowPropPlacer> GetNeighborsWithScript<T>() where T : MonoBehaviour
        {
            ResetPreview();
            var thisScriptOfT = gameObject.GetComponent<T>();
            if (thisScriptOfT == null)
            {
                return null;
            }


            List<T> allObjects = FindObjectsOfType<T>().ToList();

            List<RowPropPlacer> rppList = FindObjectsOfType<T>().Where(x => x.GetComponent<RowPropPlacer>() != null).Select(x => x.GetComponent<RowPropPlacer>()).ToList();
            List<RowPropPlacer> neighborObjects = SortedNeighbors( ref rppList, this);
            return neighborObjects;
        }


        class SubList {
            public float distanceToTarget = Mathf.Infinity;
            public List<RowPropPlacer> list;
        }


        public static List<RowPropPlacer> SortedNeighbors(ref List<RowPropPlacer> unsorted, RowPropPlacer startFrom = null, RowPropPlacer target = null, bool reverese = false )
        {
            List<SubList> branches = new List<SubList>();
            List<RowPropPlacer> returnList = null;
            if (startFrom == null)
                startFrom = unsorted[0];


            
            RowPropPlacer start = startFrom;
            if (target == null)
                target = start;

            RowPropPlacer cur = null;


            for (int i = 0; i < unsorted.Count; i++)
            {
                cur = unsorted[i];
                if (cur == null)
                    continue;

                if (IsPrentOfChild(reverese ? start : cur, reverese ? cur : start))
                {
                    unsorted.Remove(cur);
                    i--;
                    if (cur == target)//found Target
                    {
                        returnList = new List<RowPropPlacer>();
                        returnList.Add(cur);
                        returnList.Add(startFrom);
                        return returnList;
                    }


                    
                    List<RowPropPlacer> deep = SortedNeighbors(ref unsorted, cur, target, reverese);
                    if (deep != null)
                    {
                        SubList branch = new SubList();
                        branch.list = deep;
                        branch.list.Add(startFrom);

                        if (branch.list[0] == target)
                            return branch.list;


                        branch.distanceToTarget = Vector3.Distance(branch.list[0].SpawnPivotWorldPosition, target.OriginPivotWorldPosition);

                        branches.Add(branch);
                    }
                    else
                    {
                        SubList branch = new SubList();
                        branch.list = new List<RowPropPlacer>();
                        branch.list.Add(cur);
                        branch.list.Add(startFrom);
                        branch.distanceToTarget = Vector3.Distance(branch.list[0].SpawnPivotWorldPosition, target.OriginPivotWorldPosition);
                        branches.Add(branch);
                    }
                }
            }
            float closest = Mathf.Infinity;
            foreach (SubList branch in branches)
            {
                if (branch.distanceToTarget >= closest)
                    continue;
                returnList = branch.list;
                closest = branch.distanceToTarget;
            }

            if (!reverese && target == startFrom)
            {
                List<RowPropPlacer> otherDirection = SortedNeighbors(ref unsorted, startFrom, returnList[returnList.Count - 1], true);
                if (otherDirection == null || otherDirection.Count == 0)
                    return returnList;
                returnList.Reverse();
                returnList.AddRange(otherDirection);
                returnList.Reverse();
            }



            return returnList;
        }


        public static bool IsPrentOfChild(RowPropPlacer parent, RowPropPlacer child)
        {
            if (!parent.enableTransformationInheritance || !child.enableTransformationInheritance)
                return false;

            if(Vector3.Distance(parent.SpawnPivotWorldPosition, child.OriginPivotWorldPosition)>0.01f)
                return false;
            
            if(Quaternion.Angle(parent.SpawnPivotWorldRotation, child.OriginPivotWorldRotation)>0.01f)
                return false;
            
            //todo: check for rotation
            return true;
        }



        public static Vector3 TranslateToWorldPosition(Vector3 localPosition, Transform parent, float scale = 0)
        {
            if(scale == 0)
                return parent.TransformPoint( localPosition * BHUtilities.FindMeshScaleFactor( parent ) );
            return parent.TransformPoint( localPosition * scale );
        }







        [ExecuteInEditMode]
        public class PreviewObjectTracker : MonoBehaviour
        {
            public RowPropPlacer owner;
            [ExecuteInEditMode]
            private void Update()
            {
                CheckIfShouldExist();
            }
           

            void CheckIfShouldExist()
            {
                if (owner == null || owner.generatedPreviewGo != this.gameObject)
                {
                    DestroyImmediate( gameObject );
                    return;
                }
                if (!Selection.Contains( owner.gameObject ))
                {
                    owner.ResetPreview();
                    return;
                }
                if (!owner.editSpawnPivot && !owner.drawPreview)
                {
                    owner.ResetPreview();
                    return;
                }
            }
        }

#endif
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityObject = UnityEngine.Object;
namespace BuildingMakerToolset { 
    public class BHUtilities
    {
#if UNITY_EDITOR
        public static Vector3 RoundVector(Vector3 vec, int dp = 1)
        {
            vec *= dp;
            return new Vector3( Mathf.Round( vec.x ), Mathf.Round( vec.y ), Mathf.Round( vec.z ) ) / dp;
        }


        public static Vector3 ClampToRotation(Vector3 vec)
        {
            return new Vector3( Mathf.Repeat( vec.x + 180, 360 ) - 180, Mathf.Repeat( vec.y + 180, 360 ) - 180, Mathf.Repeat( vec.z + 180, 360 ) - 180 );
        }


        public static bool IsPrefab(GameObject go)
        {
            if (go == null)
                return false;
            if (PrefabUtility.GetPrefabAssetType( go ) == PrefabAssetType.Regular)
                return true;
            if (PrefabUtility.GetPrefabAssetType( go ) == PrefabAssetType.Variant)
                return true;
            if (PrefabUtility.GetPrefabAssetType( go ) == PrefabAssetType.Model)
                return true;
            return false;
        }

        public static float FindMeshScaleFactor(Transform transform)
        {
            return GetMeshScaleFactor( FindMeshRelativeToTransform( transform ) );
        }
       
        public static float GetMeshScaleFactor(MeshFilter mf)
        {
            if (mf == null)
                return 1;
           return GetMeshScaleFactor( mf.sharedMesh );
        }

        public static float GetMeshScaleFactor(Mesh m)
        {
            if (m == null)
                return 1;
            ModelImporter modelImporter = (ModelImporter)ModelImporter.GetAtPath( AssetDatabase.GetAssetPath( m ) );
            if (modelImporter == null)
            {
                return 1;
            }
            float scaleFactor = modelImporter.globalScale;
            return scaleFactor;
        }

        public static MeshFilter FindMeshRelativeToTransform(Transform transform)
        {
            if (transform == null)
                return null;
            MeshFilter mf = transform.gameObject.GetComponent<MeshFilter>();
            if (mf == null)
                mf = transform.gameObject.GetComponentInChildren<MeshFilter>();
            Transform parent = transform.parent;
            if (mf == null)
                mf = transform.gameObject.GetComponentInParent<MeshFilter>();

            return mf;
        }


        public static  Vector3 AvaragePosition(Vector3[] positions)
        {
            if (positions == null || positions.Length == 0)
                return Vector3.zero;

            Vector3 avaragePos = Vector3.zero;
            for (int i = 0; i < positions.Length; i++)
            {
                avaragePos += positions[i];
            }
            return avaragePos / positions.Length;
        }


        public static bool ArrayContainsPrefab(SerializedProperty array, UnityObject newObject)
        {
            if (array == null)
                return false;
            for (int i = 0; i < array.arraySize; i++)
            {
                if (array.GetArrayElementAtIndex( i ).objectReferenceValue == newObject)
                    return true;
            }

            return false;
        }



        public static void ArrayAdd(SerializedProperty arrayProp, UnityObject newObject)
        {
            arrayProp.serializedObject.UpdateIfRequiredOrScript();

            arrayProp.arraySize++;
            arrayProp.GetArrayElementAtIndex( arrayProp.arraySize - 1 ).objectReferenceValue = newObject;
            arrayProp.serializedObject.ApplyModifiedProperties();
        }




        public static void ArrayRemoveAt(SerializedProperty arrayProp, int atIndex)
        {

            arrayProp.serializedObject.UpdateIfRequiredOrScript();

            arrayProp.MoveArrayElement( atIndex, arrayProp.arraySize - 1 );
            arrayProp.arraySize--;
            arrayProp.serializedObject.ApplyModifiedProperties();
        }



        public static void ArrowGizmo(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.025f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.DrawRay( pos, direction );

            Vector3 right = Quaternion.LookRotation( direction ) * Quaternion.Euler( 0, 180 + arrowHeadAngle, 0 ) * new Vector3( 0, 0, 1 );
            Vector3 left = Quaternion.LookRotation( direction ) * Quaternion.Euler( 0, 180 - arrowHeadAngle, 0 ) * new Vector3( 0, 0, 1 );
            Gizmos.DrawRay( pos + direction, right * arrowHeadLength );
            Gizmos.DrawRay( pos + direction, left * arrowHeadLength );
        }
        public static void SetGlobalScale(Transform transform, Vector3 globalScale)
        {
            transform.localScale = Vector3.one;
            transform.localScale = new Vector3( globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z );
        }


        
        static GameObject cachedGo;
        static MeshFilter[] meshFilters;
        static List<Vector3> vertices;
        static public bool FindNearestVertex(Vector2 mousePosition,GameObject go , out Vector3 vertexposition,List<Vector3>additionalPointsToCheck = null,float minSnapDistance = 15, float minDisplayDistance = 80)
        {
            Color oHandleColor = Handles.color;
            vertexposition = Vector3.zero;
            mousePosition.y = Screen.height - mousePosition.y;
            if (cachedGo != go)
            {
                cachedGo = go;
                meshFilters = go.GetComponentsInChildren<MeshFilter>();
                int curVert = 0;
                
                if (meshFilters == null || meshFilters.Length == 0)
                    return false;

                vertices = new List<Vector3>();
                for (int i = 0; i < meshFilters.Length; i++)
                {
                    if (meshFilters[i].sharedMesh == null)
                        continue;
                    Mesh mesh = meshFilters[i].sharedMesh;

                    for (int j = 0; j < mesh.vertices.Length; j++)
                    {
                        Vector3 verextWorldPosition = meshFilters[i].transform.TransformPoint( mesh.vertices[j] );
                        if (vertices.Count > curVert)
                            vertices[curVert] = go.transform.InverseTransformPoint( verextWorldPosition);
                        else
                            vertices.Add( go.transform.InverseTransformPoint(verextWorldPosition) );
                        curVert++;
                    }
                }
                if (additionalPointsToCheck != null)
                    vertices.AddRange( additionalPointsToCheck.Select(x=> go.transform.InverseTransformPoint(x)) );

            }
            if (vertices == null || vertices.Count == 0)
                return false;

            bool foundCloseVertex = false;
            Camera szeneCamera = SceneView.lastActiveSceneView.camera;
            float smallestDistance = Mathf.Infinity;

            Handles.color = Color.white;
            for (int i = 0; i < vertices.Count; i++)
            {
                bool checkingInAdditionaPoints = additionalPointsToCheck != null && i > vertices.Count - additionalPointsToCheck.Count - 1;
                Vector3 VertexWorldPos = go.transform.TransformPoint( vertices[i] );
                Vector2 vertexOnScreen = szeneCamera.WorldToScreenPoint( VertexWorldPos );
                float distance = Vector2.Distance( vertexOnScreen, mousePosition );
                if (distance < smallestDistance)
                {
                    if(distance< minSnapDistance)
                        foundCloseVertex = true;
                    smallestDistance = distance;
                    vertexposition = VertexWorldPos;
                }
                if (checkingInAdditionaPoints)
                {
                    Handles.color = Color.yellow;
                    Handles.DrawWireCube( VertexWorldPos, Vector3.one * 0.03f );
                }
                else if (distance < minDisplayDistance)
                    Handles.DrawWireCube( VertexWorldPos, Vector3.one * 0.01f );
           
                    
            }

            if (foundCloseVertex)
            {
                Handles.color = Color.red;
                Handles.DrawWireCube( vertexposition, Vector3.one * 0.01f );
            }
            Handles.color = Color.yellow;
            float scale = szeneCamera.nearClipPlane;
            Vector3 dir = szeneCamera.transform.forward;
            Vector3 pos = szeneCamera.ScreenToWorldPoint( new Vector3( mousePosition.x, mousePosition.y, scale * 1.1f ));
            float size = minSnapDistance /Screen.height ;
            Handles.DrawWireDisc( pos, dir, size * scale);
            Handles.color = oHandleColor;


            return foundCloseVertex;
        }
        
        public static List<Vector3> GetBoundPoints(GameObject obj )
        {
            List<Vector3> boundList = new List<Vector3>();
            MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
            if (meshFilters.Length == 0)
                return boundList;

            List<Vector3> forward = new List<Vector3>();
            List<Vector3> back = new List<Vector3>();
            List<Vector3> left = new List<Vector3>();
            List<Vector3> right = new List<Vector3>();
            List<Vector3> up = new List<Vector3>();
            List<Vector3> down = new List<Vector3>();

            List<Vector3> allVerts = new List<Vector3>();

            
            for (int i = 0; i < meshFilters.Length; i++)
            {
                if ( meshFilters[i].sharedMesh == null)
                    continue;
                Mesh mesh = meshFilters[i].sharedMesh;
                Transform mfTr = meshFilters[i].transform;
                allVerts.AddRange( mesh.vertices.Select(x=> obj.transform.InverseTransformPoint( mfTr.TransformPoint(x))) );
            }
            Bounds bounds = EncloseInBounds( allVerts );
            forward.AddRange( SelectEdge( allVerts, bounds, Vector3.forward ) );
            back.AddRange( SelectEdge( allVerts, bounds, -Vector3.forward ) );
            left.AddRange( SelectEdge( allVerts, bounds, -Vector3.right ) );
            right.AddRange( SelectEdge( allVerts, bounds, Vector3.right ) );
            up.AddRange( SelectEdge( allVerts, bounds, Vector3.up ) );
            down.AddRange( SelectEdge( allVerts, bounds, -Vector3.up ) );


            boundList.Add( obj.transform.TransformPoint(AvarageVector( forward )) );
            boundList.Add( obj.transform.TransformPoint( AvarageVector( back )) );
            boundList.Add( obj.transform.TransformPoint( AvarageVector( left )) );
            boundList.Add( obj.transform.TransformPoint( AvarageVector( right )) );
            boundList.Add( obj.transform.TransformPoint( AvarageVector( up )) );
            boundList.Add( obj.transform.TransformPoint( AvarageVector( down )) );
            return boundList;

        }
        static List<Vector3> SelectEdge(List<Vector3> verts, Bounds bounds, Vector3 side, float maxDistance = 0.0001f)
        {
            Bounds removeBounds = bounds;
            removeBounds.Expand( maxDistance );
            removeBounds.center += side * (-maxDistance*2);
            List<Vector3> selectedList = new List<Vector3>( verts.Where( x => !removeBounds.Contains( x ) ) );
            return selectedList;
        }
        static Bounds EncloseInBounds(List<Vector3> vList)
        {
            Bounds bounds = new Bounds();
            Vector3 max = Vector3.zero;
            Vector3 min =Vector3.zero;
            MinMaxVector( vList, ref min, ref max );
            bounds.SetMinMax( min, max );
            return bounds;
        }
       
        public static Vector3 AvarageVector(List<Vector3> vectors)
        {
            if (vectors.Count == 0)
                return Vector3.zero;
            Vector3 max = Vector3.zero;
            Vector3 min = Vector3.zero;
            MinMaxVector( vectors, ref min, ref max );
            return Vector3.Lerp( max, min, 0.5f );
        }
        static void MinMaxVector(List<Vector3> vectors, ref Vector3 min, ref Vector3 max)
        {
            if (vectors == null || vectors.Count == 0)
                return;
            max = Vector3.one * -Mathf.Infinity;
            min = Vector3.one * Mathf.Infinity;
            for (int i = 0; i < vectors.Count; i++)
            {
                Vector3 v = vectors[i];
                if (v.x > max.x)
                    max.x = v.x;
                else if (v.x < min.x)
                    min.x = v.x;

                if (v.y > max.y)
                    max.y = v.y;
                else if (v.y < min.y)
                    min.y = v.y;

                if (v.z > max.z)
                    max.z = v.z;
                else if (v.z < min.z)
                    min.z = v.z;
            }
        }
        public static Vector3 AllignOnTransformAxis(Transform relativeToTransform, Vector3 position, Vector3 reference, bool x, bool y, bool z)
        {
            if (relativeToTransform == null)
                return new Vector3( x ? reference.x : position.x, y ? reference.y : position.y, z ? reference.z : position.z );
            Vector3 localPosition = relativeToTransform.InverseTransformPoint( position );
            Vector3 localReference = relativeToTransform.InverseTransformPoint( reference );
            return relativeToTransform.TransformPoint( new Vector3( x ? localReference.x : localPosition.x, y ? localReference.y : localPosition.y, z ? localReference.z : localPosition.z ) );
        }
        public static Vector3 ClosestTransformAxisToAxis(Transform transform, Vector3 targetAxis)
        {
            Vector3 bestAxis = transform.up;
            float bestAngl = Vector3.Angle( bestAxis, targetAxis );

            Vector3 testAxis;
            float testAngl;



            testAxis = transform.right;
            testAngl = Vector3.Angle( testAxis, targetAxis );
            if (testAngl < bestAngl)
            {
                bestAngl = testAngl;
                bestAxis = testAxis;
            }


            testAxis = transform.forward;
            testAngl = Vector3.Angle( testAxis, targetAxis );
            if (testAngl < bestAngl)
            {
                bestAngl = testAngl;
                bestAxis = testAxis;
            }


            testAxis = -transform.up;
            testAngl = Vector3.Angle( testAxis, targetAxis );
            if (testAngl < bestAngl)
            {
                bestAngl = testAngl;
                bestAxis = testAxis;
            }


            testAxis = -transform.right;
            testAngl = Vector3.Angle( testAxis, targetAxis );
            if (testAngl < bestAngl)
            {
                bestAngl = testAngl;
                bestAxis = testAxis;
            }


            testAxis = -transform.forward;
            testAngl = Vector3.Angle( testAxis, targetAxis );
            if (testAngl < bestAngl)
            {
                bestAngl = testAngl;
                bestAxis = testAxis;
            }

            return bestAxis;
        }


#endif
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/*
 * Processes array of shapes into a single mesh
 * Automatically determines which shapes are solid, and which are holes
 * Ignores invalid shapes (contain self-intersections, too few points, overlapping holes)
 */

namespace BuildingMakerToolset.Geometry
{
    public partial class CompositeShape
    {
        
        public List<ShapeMeshData> shapeMeshes;
        public class ShapeMeshData
        {
            public Mesh backedMesh;
            public Material material;
            public int[] triangles;
            public Vector3[] vertices;
            public Vector2[] uv;
        }

        Shape[] shapes;
        Transform transform;
        float height = 0;

        public CompositeShape(IEnumerable<Shape> shapes)
        {
            this.shapes = shapes.ToArray();
        }

        public ShapeMeshData[] GetShapeMeshData(Transform tr = null, bool generateUV2 = true)
        {
            transform = tr;

            if (!Process())
                return null;
           
           



            
            for(int i = 0; i< shapeMeshes.Count; i++)
            {
                Mesh mesh = new Mesh();
                mesh.SetVertices( shapeMeshes[i].vertices );
                mesh.SetUVs(0, shapeMeshes[i].uv );
                mesh.SetTriangles( shapeMeshes[i].triangles,0 );
                mesh.RecalculateNormals();
                mesh.RecalculateTangents();
#if UNITY_EDITOR
                if(generateUV2)
                    UnityEditor.Unwrapping.GenerateSecondaryUVSet( mesh );
#endif
                shapeMeshes[i].backedMesh = mesh;
                
            }
            return shapeMeshes.ToArray();



       
        }
        public bool Process()
        {
            // Generate array of valid shape data
            List<CompositeShapeData> eligibleShapes = shapes.Select(shape => new CompositeShapeData(  Maths2D.ToXZ( shape.points.ToArray()), transform, shape ) ).Where(x => x.IsValidShape).ToList();
            List < CompositeShapeData >wallShapes = new List<CompositeShapeData>();
            for (int i = 0; i < eligibleShapes.Count; i++)
            {
                if (eligibleShapes[i].baseShape.thickness == 0 || eligibleShapes[i].isOtherSideOfThickShape)
                    continue;

                if (eligibleShapes[i].baseShape.wall)
                {
                    wallShapes.Add( eligibleShapes[i] );
                }

                if (!eligibleShapes[i].baseShape.hole )
                {
                    CompositeShapeData newShape = new CompositeShapeData( eligibleShapes[i].points, transform , eligibleShapes[i].baseShape );
                    newShape.isOtherSideOfThickShape = true;
                    eligibleShapes.Add( newShape );
                }
            }

                // Set parents for all shapes. A parent is a shape which completely contains another shape.
                for (int i = 0; i < eligibleShapes.Count; i++)
            {
                for (int j = 0; j < eligibleShapes.Count; j++)
                {
                    if (i == j)
                        continue;

                    if (eligibleShapes[i].IsParentOf(eligibleShapes[j]))
                    {
                        eligibleShapes[j].parents.Add(eligibleShapes[i]);
                    }
                }
            }

            // Holes are shapes with an odd number of parents.
            //CompositeShapeData[] holeShapes = eligibleShapes.Where( x => x.parents.Count % 2 != 0 ).ToArray();
            List<CompositeShapeData> holeShapes = eligibleShapes.Where(x => x.baseShape.hole ).ToList();
            List<CompositeShapeData> badHoles = new List<CompositeShapeData>();
            foreach (CompositeShapeData holeShape in holeShapes)
            {
                for(int i = 0; i< holeShapes.Count; i++)
                {
                    if(holeShapes[i].CheckIfSameShape( holeShape ) && !badHoles.Contains( holeShapes[i] ))
                    {
                        badHoles.Add( holeShapes[i] );
                    }
                }
                /*
                // The most immediate parent (i.e the smallest parent shape) will be the one that has the highest number of parents of its own. 
                if (holeShape.parents == null || holeShape.parents.Count == 0)
                    continue;
                CompositeShapeData immediateParent = holeShape.parents.OrderByDescending(x => x.parents.Count).First();
                //immediateParent.holes.Add(holeShape);
                */
            }
            foreach (CompositeShapeData badHole in badHoles)
            {
                holeShapes.Remove( badHole );
            }


            // Solid shapes have an even number of parents
            //
            //CompositeShapeData[] solidShapes = eligibleShapes.Where( x => true).ToArray();
            CompositeShapeData[] solidShapes = eligibleShapes.Where( x => !x.baseShape.hole ).ToArray();
            for(int i = 0; i< solidShapes.Length; i++)
            {
                for(int y = 0; y< holeShapes.Count; y++)
                {
                    if (solidShapes[i].IsInCutrangeOfHoleShape( holeShapes[y] ) &&  solidShapes[i].IsParentOf( holeShapes[y] ))
                         solidShapes[i].holes.Add( holeShapes[y] );
                }
                
            }
            foreach (CompositeShapeData solidShape in solidShapes)
            {
                solidShape.ValidateHoles();

            }
            shapeMeshes = new List<ShapeMeshData>();
            for(int i = 0; i< solidShapes.Length; i++)
            {

                ShapeMeshData submesh = new ShapeMeshData();
               
                Polygon poly = new Polygon( solidShapes[i].polygon.points, solidShapes[i].holes.Select( h => h.polygon.points ).ToArray());
                submesh.vertices = poly.points.Select( v2 => new Vector3( v2.x,height + solidShapes[i].baseShape.hightOffset  + (solidShapes[i].isOtherSideOfThickShape ? solidShapes[i].baseShape.thickness: 0), v2.y )).ToArray();
                Vector2 uvOffset = (!solidShapes[i].isOtherSideOfThickShape? solidShapes[i].baseShape.uvOffset : solidShapes[i].baseShape.uvOffset2 ) * 0.3f;
                Vector2 uvScale = (!solidShapes[i].isOtherSideOfThickShape ? solidShapes[i].baseShape.uvScale : solidShapes[i].baseShape.uvScale2) * 0.3f;
                submesh.uv = poly.points.Select( v2 => new Vector2( uvOffset.x + v2.x * uvScale.x , uvOffset.y + v2.y * uvScale.y ) ).ToArray();

                List<int> allTri = new List<int>();
                Triangulator triangulator = new Triangulator( poly);
                int[] polygonTriangles = triangulator.Triangulate();
                
                if ((!solidShapes[i].isOtherSideOfThickShape &&  solidShapes[i].baseShape.flip) || (solidShapes[i].isOtherSideOfThickShape && !solidShapes[i].baseShape.flip))
                {
                    System.Array.Reverse( polygonTriangles );
                }
                
                


                    
                if (polygonTriangles == null)
                    return false;
                for (int j = 0; j < polygonTriangles.Length; j++)
                {
                    allTri.Add( polygonTriangles[j]);
                }

                submesh.material = solidShapes[i].isOtherSideOfThickShape ? solidShapes[i].baseShape.downMaterial : solidShapes[i].baseShape.upMaterial;
                submesh.triangles = allTri.ToArray();
                shapeMeshes.Add( submesh );
            }


            for (int i = 0; i < wallShapes.Count; i++)
            {
                shapeMeshes.Add( EdgeMesh( wallShapes[i]) );
            }

            for (int i = 0; i < shapeMeshes.Count; i++)
            {
                for (int y = 0; y < shapeMeshes.Count; y++)
                {
                    if (y == i)
                        continue;
                    if (shapeMeshes[i].material == shapeMeshes[y].material)
                    {
                        MergeShapeMeshData( i, y );
                        shapeMeshes.RemoveAt( y );
                        y--;
                    }
                }
            }
   



            return true;
        }



        ShapeMeshData EdgeMesh(CompositeShapeData wallData)
        {
            Vector3[] verticies = new Vector3[wallData.points.Length*4 +4];
            Vector2[] uv = new Vector2[verticies.Length];
            int trianglecount = (verticies.Length / 2) +2;
            int[] tris = new int[trianglecount * 3];
            int vertIndex = 0;
            int triIndex = 0;

            Vector2 uvOffset = wallData.baseShape.wallUvOffset * 0.3f;
            Vector2 uvScale = wallData.baseShape.wallUvScale * 0.3f;

            float uvY= 0;
            float uvX =0;

            for (int i = 0; i< wallData.points.Length + 1;i++)
            {
                Vector3 cur = wallData.points[i % wallData.points.Length];

                verticies[vertIndex] = new Vector3( cur.x, height + wallData.baseShape.hightOffset, cur.y );
                uv[vertIndex] = new Vector2( uvOffset.x + uvX * uvScale.x, uvOffset.y + uvY * uvScale.y );
                vertIndex++;
                verticies[vertIndex] = new Vector3( cur.x, height + wallData.baseShape.hightOffset + wallData.baseShape.thickness, cur.y );
                uv[vertIndex] = new Vector2( uvOffset.x + uvX * uvScale.x, uvOffset.y + (uvY + Mathf.Abs( wallData.baseShape.thickness ) * uvScale.y) ); 
                vertIndex++;



                if (i< wallData.points.Length)
                {
                    tris[triIndex] = vertIndex;
                    tris[triIndex+1] = (vertIndex+2) % verticies.Length;
                    tris[triIndex+2] = vertIndex+1;

                    tris[triIndex + 3] = vertIndex + 1;
                    tris[triIndex + 4] = (vertIndex + 2) % verticies.Length;
                    tris[triIndex + 5] = (vertIndex + 3) % verticies.Length;
                    
                }
   

                triIndex += 6;
                
                
                
                verticies[vertIndex] = new Vector3( cur.x, height + wallData.baseShape.hightOffset, cur.y );
                uv[vertIndex] = new Vector2( uvOffset.x + uvX * uvScale.x, uvOffset.y + uvY * uvScale.y );
                vertIndex++;
                verticies[vertIndex] = new Vector3( cur.x, height + wallData.baseShape.hightOffset + wallData.baseShape.thickness, cur.y );
                uv[vertIndex] = new Vector2( uvOffset.x + uvX * uvScale.x, uvOffset.y + (uvY + Mathf.Abs( wallData.baseShape.thickness ) * uvScale.y) );
                vertIndex++;

         





                uvX += Vector3.Distance( cur, wallData.points[(i + 1) % wallData.points.Length] );

            }

            if (Polygon.PointsAreCounterClockwise( wallData.points ))
            {
                if (wallData.baseShape.hole && !wallData.baseShape.flip || !wallData.baseShape.hole && wallData.baseShape.flip)
                {
                    System.Array.Reverse( tris );
                }
            }

            else if (!wallData.baseShape.hole && !wallData.baseShape.flip || wallData.baseShape.hole && wallData.baseShape.flip)
            {
                System.Array.Reverse( tris );
            }

            ShapeMeshData newWallMesh = new ShapeMeshData();
            newWallMesh.material = wallData.baseShape.sideMaterial;
            newWallMesh.vertices = verticies;
            newWallMesh.uv = uv;
            newWallMesh.triangles = tris;

           
            return newWallMesh;
        }

        void MergeShapeMeshData(int addTo, int remove)
        {
            ShapeMeshData taker = shapeMeshes[addTo];
            ShapeMeshData giver = shapeMeshes[remove];

            Vector3[] newVert = new Vector3[taker.vertices.Length + giver.vertices.Length];
            Vector2[] newUv = new Vector2[newVert.Length];

            int mergeIndex = taker.vertices.Length;
            for (int i = 0; i< newVert.Length; i++)
            {
                if (i < mergeIndex) {
                    newVert[i] = taker.vertices[i];
                    newUv[i] = taker.uv[i];
                }
                else
                {
                    newVert[i] = giver.vertices[i - mergeIndex];
                    newUv[i] = giver.uv[i - mergeIndex];
                }
            }

            int[] newTris = new int[taker.triangles.Length + giver.triangles.Length];

            for (int i = 0; i < newTris.Length; i++)
            {
                if (i < taker.triangles.Length)
                {
                    newTris[i] = taker.triangles[i];
                }
                else
                {
                    newTris[i] = giver.triangles[i- taker.triangles.Length] + mergeIndex;
                }
            }
            taker.vertices = newVert;
            taker.uv = newUv;
            taker.triangles = newTris;
        }
    }
    

}
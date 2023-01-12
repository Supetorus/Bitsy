using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace BuildingMakerToolset.Geometry
{
    public partial class CompositeShape
    {

        /*
         * Holds data for each shape needed when calculating composite shapes.
         */

        public class CompositeShapeData
        {
            public Vector2[] points;
            public Polygon polygon;
            public int[] triangles;

            public Shape baseShape;
                        
            public Material material;

            public bool isOtherSideOfThickShape;

            public List<CompositeShapeData> parents = new List<CompositeShapeData>();
            public List<CompositeShapeData> holes = new List<CompositeShapeData>();
            public bool IsValidShape { get; private set; }

            public CompositeShapeData(Vector2[] points, Transform transform, Shape shape)
            {
                SetData( points, transform, shape );
            }
            void SetData(Vector2[] points, Transform transform, Shape shape)
            {
                this.baseShape = shape;
                this.points = points;
                IsValidShape = points.Length >= 3 && !IntersectsWithSelf();

                if (IsValidShape)
                {
                    polygon = new Polygon( this.points );
                    Triangulator t = new Triangulator( polygon );
                    triangles = t.Triangulate();
                }
            }

            // Removes any holes which overlap with another hole
            public void ValidateHoles()
            {
                for (int i = 0; i < holes.Count; i++)
                {
                    for (int j = i + 1; j < holes.Count; j++)
                    {
                        bool overlap = holes[i].OverlapsPartially(holes[j]);

                        if (overlap)
                        {
                            holes[i].IsValidShape = false;
                            break;
                        }
                    }
                }

                for (int i = holes.Count - 1; i >= 0; i--)
                {

                    if (!holes[i].IsValidShape)
                    {
                        holes.RemoveAt(i);
                    }
                }
                bool clear = false;
                while (!clear)
                {
                    bool foundBadHole = false;
                    for (int i= 0; i< holes.Count; i++ )
                    {
                        if (holes[i].parents == null || holes[i].parents.Count == 0)
                            continue;

                        for (int y = 0; y < holes[i].parents.Count; y++)
                        {
                            if(holes.Contains( holes[i].parents[y] ))
                            {
                                holes.Remove( holes[i]);
                                foundBadHole = true;
                                break;
                            }
                        }
                    }
                    if (!foundBadHole)
                        clear = true;

                }


            }
            public bool IsInCutrangeOfHoleShape(CompositeShapeData holeShape)
            {
                float hightOffset = isOtherSideOfThickShape ? baseShape.hightOffset + baseShape.thickness : baseShape.hightOffset;
                if (Mathf.Min( holeShape.baseShape.hightOffset, holeShape.baseShape.hightOffset + holeShape.baseShape.thickness ) <= hightOffset && hightOffset <= Mathf.Max( holeShape.baseShape.hightOffset, holeShape.baseShape.hightOffset + holeShape.baseShape.thickness ))
                    return true;
                return false;
                
            }
            // A parent is a shape which fully contains another shape
            public bool IsParentOf(CompositeShapeData otherShape)
            {
                if (triangles == null || triangles.Length == 0)
                    return false;
                if (otherShape.parents.Contains(this))
                {
                    return true;
                }
                if (parents.Contains(otherShape))
                {
                    return false;
                }

                // check if first point in otherShape is inside this shape. If not, parent test fails.
                // if yes, then continue to line seg intersection test between the two shapes

                // (this point test is important because without it, if all line seg intersection tests fail,
                // we wouldn't know if otherShape is entirely inside or entirely outside of this shape)
                bool pointInsideShape = false;
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    if (Maths2D.PointInTriangle(polygon.points[triangles[i]], polygon.points[triangles[i + 1]], polygon.points[triangles[i + 2]], otherShape.points[0]))
                    {
                        pointInsideShape = true;
                        break;
                    }
                }

                if (!pointInsideShape)
                {
                    return false;
                }

                // Check for intersections between line segs of this shape and otherShape (any intersections will fail the parent test)
                for (int i = 0; i < points.Length; i++)
                {
                    LineSegment parentSeg = new LineSegment(points[i], points[(i + 1) % points.Length]);
                    for (int j = 0; j < otherShape.points.Length; j++)
                    {
                        LineSegment childSeg = new LineSegment(otherShape.points[j], otherShape.points[(j + 1) % otherShape.points.Length]);
                        if (Maths2D.LineSegmentsIntersect(parentSeg.a, parentSeg.b, childSeg.a, childSeg.b))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            public bool CheckIfSameShape(CompositeShapeData otherShape)
            {
                if(otherShape == this)
                    return false;
                if (points.Length != otherShape.points.Length)
                    return false;
                for(int i = 0;i < points.Length; i++)
                {
                    if (points[i] != otherShape.points[i])
                        return false;
                }
                return true;
            }

            // Test if the shapes overlap partially (test will fail if one shape entirely contains other shape, i.e. one is parent of the other).
            public bool OverlapsPartially(CompositeShapeData otherShape)
            {

                // Check for intersections between line segs of this shape and otherShape (any intersection will validate the overlap test)
                for (int i = 0; i < points.Length; i++)
                {
                    LineSegment segA = new LineSegment(points[i], points[(i + 1) % points.Length]);
                    for (int j = 0; j < otherShape.points.Length; j++)
                    {
                        LineSegment segB = new LineSegment(otherShape.points[j], otherShape.points[(j + 1) % otherShape.points.Length]);
                        if (Maths2D.LineSegmentsIntersect(segA.a, segA.b, segB.a, segB.b))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            // Checks if any of the line segments making up this shape intersect
            public bool IntersectsWithSelf()
            {

                for (int i = 0; i < points.Length; i++)
                {
                    LineSegment segA = new LineSegment(points[i], points[(i + 1) % points.Length]);
                    for (int j = i + 2; j < points.Length; j++)
                    {
                        if ((j + 1) % points.Length == i)
                        {
                            continue;
                        }
                        LineSegment segB = new LineSegment(points[j], points[(j + 1) % points.Length]);
                        if (Maths2D.LineSegmentsIntersect(segA.a, segA.b, segB.a, segB.b))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            public struct LineSegment
            {
                public readonly Vector2 a;
                public readonly Vector2 b;

                public LineSegment(Vector2 a, Vector2 b)
                {
                    this.a = a;
                    this.b = b;
                }
            }
        }

    
    }
}
using System;
using moveen.utils;
using UnityEngine;

namespace moveen.descs {
    [RequireComponent(typeof (MeshFilter))]
    [RequireComponent(typeof (MeshRenderer))]
    public class MoveenSkelBezierMesh : MoveenSkelBezier {
        //TODO consider composition instead of inheritance

        [Range(3, 50)]
        public int radiusSteps = 5;
        public float radius = 0.5f;
        public bool unityNormalCalculation;

        public bool startCap;
        public bool finishCap;
        public bool useDifferentFinishRadius;
        public float finishRadius;
        
        private MeshFilter mf;
        private Vector3[] vertices;
        private Vector3[] normals;
        private Vector2[] uv;
        private int[] tri;

        [ReadOnly] public int trianglesCount;

        public override void updateData() {
            base.updateData();

            mf = gameObject.GetComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            mf.mesh = mesh;

            int tempVerticesCount = poss.Count * radiusSteps;
            int verticesCount = tempVerticesCount;
            int trianglesCount = radiusSteps * 2 * (poss.Count - 1);
            if (startCap) {
                verticesCount += 1 + radiusSteps;
                trianglesCount += radiusSteps;
            }
            if (finishCap) {
                verticesCount += 1 + radiusSteps;
                trianglesCount += radiusSteps;
            }
            this.trianglesCount = trianglesCount;
            
            vertices = new Vector3[verticesCount];
            normals = new Vector3[verticesCount];
            uv  = new Vector2[verticesCount];
            tri = new int[trianglesCount * 3];

            int curTriangle = 0;
            for (int stepIndex = 0; stepIndex < poss.Count - 1; stepIndex++) {
                for (int rIndex = 0; rIndex < radiusSteps; rIndex++) {
                    int rIndexNext = (rIndex + 1) % radiusSteps;

                    tri[curTriangle * 3 + 0] = (stepIndex + 0) * radiusSteps + rIndex + 0;
                    tri[curTriangle * 3 + 1] = (stepIndex + 1) * radiusSteps + rIndex + 0;
                    tri[curTriangle * 3 + 2] = (stepIndex + 0) * radiusSteps + rIndexNext;
                    curTriangle++;

                    tri[curTriangle * 3 + 0] = (stepIndex + 1) * radiusSteps + rIndex + 0;
                    tri[curTriangle * 3 + 1] = (stepIndex + 1) * radiusSteps + rIndexNext;
                    tri[curTriangle * 3 + 2] = (stepIndex + 0) * radiusSteps + rIndexNext;
                    curTriangle++;

                }
            }

            if (startCap) {
                for (int i = 0; i < radiusSteps; i++) {
                    int i2 = (i + 1) % radiusSteps;
                    tri[curTriangle * 3 + 0] = tempVerticesCount;
                    tri[curTriangle * 3 + 1] = tempVerticesCount + 1 + i;
                    tri[curTriangle * 3 + 2] = tempVerticesCount + 1 + i2;
                    curTriangle++;
                }
                tempVerticesCount += radiusSteps + 1;
            }
            
            if (finishCap) {
                for (int i = 0; i < radiusSteps; i++) {
                    int i2 = (i + 1) % radiusSteps;
                    tri[curTriangle * 3 + 0] = tempVerticesCount;
                    tri[curTriangle * 3 + 1] = tempVerticesCount + 1 + i2;
                    tri[curTriangle * 3 + 2] = tempVerticesCount + 1 + i;
                    curTriangle++;
                }
            }

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uv;
            mesh.triangles = tri;
            
        }

        public override void tick(float dt) {
            base.tick(dt);
            Mesh mesh = mf.sharedMesh;
            mesh.Clear();
            
            int curVertex = 0;
            for (int stepIndex = 0; stepIndex < poss.Count; stepIndex++) {
                for (int rIndex = 0; rIndex < radiusSteps; rIndex++) {
                    curVertex = fillForStep(rIndex, stepIndex, curVertex);
                }
            }

            if (startCap) {
                vertices[curVertex] = new Vector3(0, 0, 0);
                normals[curVertex] = new Vector3(-1, 0, 0);
                uv[curVertex] = new Vector2(0, 0);
                curVertex++;
                for (int rIndex = 0; rIndex < radiusSteps; rIndex++) {
                    curVertex = fillForStep(rIndex, 0, curVertex);
                }
            }
            if (finishCap) {
                vertices[curVertex] = transform.InverseTransformPoint(poss[poss.Count - 1]);
                normals[curVertex] = normals[curVertex] = transform.InverseTransformVector(rots[rots.Count - 1].rotate(new Vector3(1, 0, 0)));
                uv[curVertex] = new Vector2(1, 0);
                curVertex++;
                for (int rIndex = 0; rIndex < radiusSteps; rIndex++) {
                    curVertex = fillForStep(rIndex, poss.Count - 1, curVertex);
                }
            }
            

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uv;
            mesh.triangles = tri;

            if (unityNormalCalculation) {
                mesh.RecalculateNormals();
            }
        }

        private int fillForStep(int rIndex, int stepIndex, int curVertex) {
            float angle = 2 * (float) Math.PI * rIndex / radiusSteps;
            Vector3 normal = rots[stepIndex].rotate(new Vector3(0, MyMath.sin(angle), MyMath.cos(angle)));
            if (!unityNormalCalculation) {
                normals[curVertex] = transform.InverseTransformVector(normal);
            }
            vertices[curVertex] = transform.InverseTransformPoint(displacement(normal, stepIndex, rIndex) + poss[stepIndex]);
            //normals[curIndex] = normal;
            //vertices[curIndex] = normal * radius + poss[stepIndex]; 
            uv[curVertex] = new Vector2((float) stepIndex / (poss.Count - 1), (float) rIndex / radiusSteps);
            curVertex++;
            return curVertex;
        }

        //override this method for radius modification or smthng
        //  don't forget to use 'unityNormalCalculation' then
        //  iU, iV - is the indices within [0, stepsCount + 2] and [0, radiusSteps]
        public virtual Vector3 displacement(Vector3 normal, int iU, int iV) {
            float r = useDifferentFinishRadius ? MyMath.mix(radius, finishRadius, (float)iU / (poss.Count - 1)) : radius;
            return normal * r;
        }
    }
}
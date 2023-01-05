using System.Collections.Generic;
using moveen.utils;
using UnityEngine;

namespace moveen.utils {
    public class GeoData {
        public List<Vector3> pos = new List<Vector3>();
        public List<Vector3> normal = new List<Vector3>();
        public List<Vector2> uv = new List<Vector2>();
        
        public List<int> indices = new List<int>();

        public void addTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 normal) {
            int newIndex = indices.Count;
            pos.Add(a);
            pos.Add(b);
            pos.Add(c);
            this.normal.Add(normal);
            this.normal.Add(normal);
            this.normal.Add(normal);
            indices.Add(newIndex);
            indices.Add(newIndex + 1);
            indices.Add(newIndex + 2);
        }

        public void addBacks() {
            int max = indices.Count;
            for (int i = 0; i < max; i += 3) {
                indices.Add(indices[i]);
                indices.Add(indices[i + 2]);
                indices.Add(indices[i + 1]);
            }
        }

        public Mesh getMesh() {
            Mesh mesh = new Mesh();
            mesh.vertices = pos.ToArray();
            mesh.normals = normal.ToArray();
            mesh.triangles = indices.ToArray();
            return mesh;
        }
    }
}
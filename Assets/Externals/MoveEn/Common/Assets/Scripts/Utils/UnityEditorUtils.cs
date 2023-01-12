using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace moveen.utils {
    public static class UnityEditorUtils {

        public static void fillCircle2D(Vector2 position, float r) {
            Vector3 center = Camera.current.ScreenToWorldPoint(position.withZ(0.5f));
            Vector3 normal = Camera.current.transform.rotation.rotate(new Vector3(0, 0, 1));
            List<Vector3> steps = new List<Vector3>(); 
            for (float angle = 0; angle < Math.PI * 2; angle += (float)Math.PI / 5f) {
                Vector2 newPos = position + new Vector2(MyMath.cos(angle), MyMath.sin(angle)) * r;
                Vector3 sec = Camera.current.ScreenToWorldPoint(newPos.withZ(0.5f));
                steps.Add(sec);
            }

            GeoData gd = new GeoData();

            for (int i = 0; i < steps.Count; i++) {
                Vector3 m1 = steps[i];
                Vector3 m2 = steps[(i + 1) % steps.Count];
                gd.addTriangle(center, m2, m1, normal);
            }
            
            Gizmos.DrawMesh(gd.getMesh());
        }

        public static void fillCircle3D(Vector3 position, Vector3 dir, float r) {
            GL.PushMatrix();
            GL.MultMatrix(Matrix4x4.TRS(position, Quaternion.FromToRotation(Vector3.forward, dir), new Vector3(1, 1, 1)));
            fillStripe(0, r, 20);
            GL.PopMatrix();
        }

        public static void fillStripe2D(Vector2 position, float r1, float r2, int stepsCount) {
            List<Vector3> steps = new List<Vector3>(); 
            List<Vector3> steps2 = new List<Vector3>(); 
            for (float angle = 0; angle < Math.PI * 2; angle += (float)Math.PI * 2 / stepsCount) {
                steps.Add((position + new Vector2(MyMath.cos(angle), MyMath.sin(angle)) * r1).withZ(0.5f));
                steps2.Add((position + new Vector2(MyMath.cos(angle), MyMath.sin(angle)) * r2).withZ(0.5f));
            }
            steps.Add(steps[0]);
            steps2.Add(steps2[0]);

            drawStripOnScreen(steps, steps2);
        }

        public static void fillStripe(float r1, float r2, int stepsCount) {
            List<Vector3> steps = new List<Vector3>(); 
            List<Vector3> steps2 = new List<Vector3>(); 
            for (float angle = 0; angle < Math.PI * 2; angle += (float)Math.PI * 2 / stepsCount) {
                steps.Add(new Vector3(MyMath.cos(angle) * r1, MyMath.sin(angle) * r1, 0));
                steps2.Add(new Vector3(MyMath.cos(angle)* r2, MyMath.sin(angle) * r2, 0));
            }
            steps.Add(steps[0]);
            steps2.Add(steps2[0]);

            drawStrip(steps, steps2);
        }


        public static void fillArcStripe2D(Vector2 position, float r1, float r2, int stepsCount, float from, float to) {
            List<Vector3> steps = new List<Vector3>(); 
            List<Vector3> steps2 = new List<Vector3>();
            float stepSize = (to - from) / stepsCount;
            for (float angle = from; angle < to; angle += stepSize) {
                steps.Add((position + new Vector2(MyMath.cos(angle), MyMath.sin(angle)) * r1).withZ(0.5f));
                steps2.Add((position + new Vector2(MyMath.cos(angle), MyMath.sin(angle)) * r2).withZ(0.5f));
            }
            steps.Add((position + new Vector2(MyMath.cos(to), MyMath.sin(to)) * r1).withZ(0.5f));
            steps2.Add((position + new Vector2(MyMath.cos(to), MyMath.sin(to)) * r2).withZ(0.5f));

            drawStripOnScreen(steps, steps2);
        }
        
        public static void drawStripOnScreen(List<Vector3> steps, List<Vector3> steps2) {
            initMat();
            GL.PushMatrix();
            GL.LoadPixelMatrix();
            mat.SetPass(0);
            GL.Begin(GL.TRIANGLES);
            GL.Color(Gizmos.color);

            for (int i = 0; i < steps.Count - 1; i++) {
                Vector3 m1 = steps[i];
                Vector3 m2 = steps[(i + 1) % steps.Count];
                Vector3 m3 = steps2[i];
                Vector3 m4 = steps2[(i + 1) % steps2.Count];
                GL.Vertex(m2);
                GL.Vertex(m1);
                GL.Vertex(m3);
                GL.Vertex(m4);
                GL.Vertex(m3);
                GL.Vertex(m2);
            }
            GL.End();
            GL.PopMatrix();
        }

        private static void drawStrip(List<Vector3> steps, List<Vector3> steps2) {
            initMat();
            mat.SetPass(0);
            GL.Begin(GL.TRIANGLES);
            GL.Color(Gizmos.color);

            for (int i = 0; i < steps.Count - 1; i++) {
                Vector3 m1 = steps[i];
                Vector3 m2 = steps[(i + 1) % steps.Count];
                Vector3 m3 = steps2[i];
                Vector3 m4 = steps2[(i + 1) % steps2.Count];
                GL.Vertex(m2);
                GL.Vertex(m1);
                GL.Vertex(m3);
                GL.Vertex(m4);
                GL.Vertex(m3);
                GL.Vertex(m2);
            }
            GL.End();
        }

        public static Material mat;

        public static void initMat() {
//            Shader.SetGlobalColor("_HandleColor", Handles.color * new Color(1f, 1f, 1f, 0.5f));
//            Shader.SetGlobalFloat("_HandleSize", 1f);
//            HandleUtility.ApplyWireMaterial(Handles.zTest);

            if (!mat) {
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                mat = new Material(shader);
                mat.hideFlags = HideFlags.HideAndDontSave;
//                mat.SetColor("_Color", new Color(0.6f, 1, 1, 0.4f));
                mat.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_Cull", (int) UnityEngine.Rendering.CullMode.Off);
                //mat.SetInt("_ZWrite", 0);
                mat.SetInt("_ZTest", (int) UnityEngine.Rendering.CompareFunction.Disabled);
            }
            mat.SetColor("_Color", Gizmos.color);
        }

        public static void line2D(Vector2 from, Vector2 to, float thickness) {
            Vector2 normal2 = (to - from).normalized;
            normal2 = new Vector2(-normal2.y, normal2.x);
            Vector3 p1 = (from + normal2 * (thickness / 2)).withZ(0.5f);
            Vector3 p2 = (from - normal2 * (thickness / 2)).withZ(0.5f);
            Vector3 p3 = (to + normal2 * (thickness / 2)).withZ(0.5f);
            Vector3 p4 = (to - normal2 * (thickness / 2)).withZ(0.5f);
            drawStripOnScreen(new List<Vector3>{p1, p2}, new List<Vector3>{p3, p4});
        }

        public static void diamond(Vector3 v1, Vector3 v2) {
            float size = (v2 - v1).length();
            Gizmos.matrix = Matrix4x4.TRS(v1, Quaternion.FromToRotation(Vector3.right, v2 - v1), new Vector3(size, size, size));
            diamond();
            Gizmos.matrix = Matrix4x4.identity;
        }

        public static void diamond(Vector3 v1, Quaternion rot, float r) {
            float norm = rot.norm();
            if (norm > 1.1 || norm < 0.9f) return;
            Gizmos.matrix = Matrix4x4.TRS(v1, rot, new Vector3(r, r, r));
            diamond();
            Gizmos.matrix = Matrix4x4.identity;
        }

        public static void diamond() {
            float x1 = 0.2f;
            float d1 = 0.1f;

            Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(1, 0, 0));
            int m = 5;
            float fm = 5f;
            for (int i = 0; i < m; i++) {
                Vector3 pos = new Vector3(x1, (float) Math.Cos(i / fm * 2 * Math.PI) * d1, (float) Math.Sin(i / fm * 2 * Math.PI) * d1);
                Vector3 pos2 = new Vector3(x1, (float) Math.Cos((i + 1) / fm * 2 * Math.PI) * d1, (float) Math.Sin((i + 1) / fm * 2 * Math.PI) * d1);
                Gizmos.DrawLine(new Vector3(0, 0, 0), pos);
                Gizmos.DrawLine(pos, new Vector3(1, 0, 0));
                Gizmos.DrawLine(pos, pos2);
            }


//            Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(x1, d1, 0));
//            Gizmos.DrawLine(new Vector3(x1, d1, 0), new Vector3(1, 0, 0));
//            Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(x1, -d1, 0));
//            Gizmos.DrawLine(new Vector3(x1, -d1, 0), new Vector3(1, 0, 0));
//            Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(x1, 0, d1));
//            Gizmos.DrawLine(new Vector3(x1, 0, d1), new Vector3(1, 0, 0));
//            Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(x1, 0, -d1));
//            Gizmos.DrawLine(new Vector3(x1, 0, -d1), new Vector3(1, 0, 0));
//
//            Gizmos.DrawLine(new Vector3(x1, d1, 0), new Vector3(x1, 0, d1));
//            Gizmos.DrawLine(new Vector3(x1, 0, d1), new Vector3(x1, -d1, 0));
//            Gizmos.DrawLine(new Vector3(x1, -d1, 0), new Vector3(x1, 0, -d1));
//            Gizmos.DrawLine(new Vector3(x1, 0, -d1), new Vector3(x1, d1, 0));
        }

        public static void diamond2(float addAngle, int steps, float rx1, float ry, float rz) {

            Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(1, 0, 0));
            float fm = steps;
            for (int i = 0; i < steps; i++) {
                float a1 = i / fm * 2 * MyMath.PI + addAngle;
                float a2 = (i + 1) / fm * 2 * MyMath.PI + addAngle;

                Vector3 pos = new Vector3(rx1, (float) Math.Cos(a1) * ry, (float) Math.Sin(a1) * rz);
                Vector3 pos2 = new Vector3(rx1, (float) Math.Cos(a2) * ry, (float) Math.Sin(a2) * rz);
                Gizmos.DrawLine(new Vector3(rx1, 0, 0), pos);
                Gizmos.DrawLine(pos, new Vector3(1, 0, 0));
                Gizmos.DrawLine(pos, pos2);
            }
        }

        public static void arrow(Vector3 v1, Vector3 v2) {
            float size = (v2 - v1).length();
            Gizmos.matrix = Matrix4x4.TRS(v1, Quaternion.FromToRotation(Vector3.right, v2 - v1), new Vector3(size, size, size));
            arrow();
            Gizmos.matrix = Matrix4x4.identity;
        }

        public static void arrow() {
            float x1 = 0.8f;
            float d1 = 0.1f;

            Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(1, 0, 0));
            int m = 5;
            float fm = 5f;
            for (int i = 0; i < m; i++) {
                Vector3 pos = new Vector3(x1, (float) Math.Cos(i / fm * 2 * Math.PI) * d1, (float) Math.Sin(i / fm * 2 * Math.PI) * d1);
                Vector3 pos2 = new Vector3(x1, (float) Math.Cos((i + 1) / fm * 2 * Math.PI) * d1, (float) Math.Sin((i + 1) / fm * 2 * Math.PI) * d1);
//                Gizmos.DrawLine(new Vector3(0, 0, 0), pos);
                Gizmos.DrawLine(pos, new Vector3(1, 0, 0));
                Gizmos.DrawLine(pos, pos2);
            }
        }

        public static void cylinder(Vector3 pos, Vector3 X, Vector3 Y, float size, int count) {
            Gizmos.matrix = Matrix4x4.TRS(pos, MUtil.qToAxes(X, Y), new Vector3(size, size, size));
            cylinder(count);
            Gizmos.matrix = Matrix4x4.identity;
        }

        public static void cylinder(Vector3 pos, Vector3 X, Vector3 Y, float size, int count, float x1, float x2, float r1, float r2) {
            Gizmos.matrix = Matrix4x4.TRS(pos, MUtil.qToAxes(X, Y), new Vector3(size, size, size));
            cylinder(count, x1, x2, r1, r2);
            Gizmos.matrix = Matrix4x4.identity;
        }

        public static void cylinder(Vector3 pos, Quaternion rot, float size, int count) {
            Gizmos.matrix = Matrix4x4.TRS(pos, rot, new Vector3(size, size, size));
            cylinder(count);
            Gizmos.matrix = Matrix4x4.identity;
        }

        public static void cylinder(Vector3 pos, Quaternion rot, float size, int count, float x1, float x2, float r1, float r2) {
            Gizmos.matrix = Matrix4x4.TRS(pos, rot, new Vector3(size, size, size));
            cylinder(count, x1, x2, r1, r2);
            Gizmos.matrix = Matrix4x4.identity;
        }

        public static void cylinder(int count) {
            cylinder(count, -1, 1, 1, 1);
        }

        public static void cylinder(int count, float x1, float x2, float r1, float r2) {
            int m = count;
            float fm = count;
            for (int i = 0; i < m; i++) {
                Vector3 pos = new Vector3(x2, (float) Math.Cos(i / fm * 2 * Math.PI) * r2, (float) Math.Sin(i / fm * 2 * Math.PI) * r2);
                Vector3 pos2 = new Vector3(x2, (float) Math.Cos((i + 1) / fm * 2 * Math.PI) * r2, (float) Math.Sin((i + 1) / fm * 2 * Math.PI) * r2);
                Vector3 pos3 = new Vector3(x1, (float) Math.Cos(i / fm * 2 * Math.PI) * r1, (float) Math.Sin(i / fm * 2 * Math.PI) * r1);
                Vector3 pos4 = new Vector3(x1, (float) Math.Cos((i + 1) / fm * 2 * Math.PI) * r1, (float) Math.Sin((i + 1) / fm * 2 * Math.PI) * r1);
//                Gizmos.DrawLine(new Vector3(0, 0, 0), pos);
//                Gizmos.DrawLine(pos, new Vector3(1, 0, 0));
                Gizmos.DrawLine(pos, pos2);
                Gizmos.DrawLine(pos3, pos4);
                Gizmos.DrawLine(pos, pos3);
            }
        }

        public static void circle3d(Vector3 pos, Quaternion rot, float r, int count) {
            Gizmos.matrix = Matrix4x4.TRS(pos, rot, new Vector3(1, 1, 1));
            circle3d(count, r, r);
            Gizmos.matrix = Matrix4x4.identity;
        }

        public static void circle3d(int count, float r1, float r2) {
            int m = count;
            float fm = count;
            for (int i = 0; i < m; i++) {
                Vector3 pos = new Vector3((float) Math.Cos(i / fm * 2 * Math.PI) * r1, 0, (float) Math.Sin(i / fm * 2 * Math.PI) * r2);
                Vector3 pos2 = new Vector3((float) Math.Cos((i + 1) / fm * 2 * Math.PI) * r1, 0, (float) Math.Sin((i + 1) / fm * 2 * Math.PI) * r2);
                Gizmos.DrawLine(pos, pos2);
            }
        }

        public static void arc3d(Vector3 pos, Quaternion rot, int count, float r, float from, float to) {
            Gizmos.matrix = Matrix4x4.TRS(pos, rot, new Vector3(1, 1, 1));
            arc3d(count, r, @from, to);
            Gizmos.matrix = Matrix4x4.identity;
        }

        public static void arc3d(int count, float r, float from, float to) {
            float arcLen = to - from;
            for (float i = 0; i < count; i++) {
                float angle = from + arcLen / count * i;
                float angle2 = from + arcLen / count * (i + 1);
                Vector3 pos1 = new Vector3((float) Math.Cos(angle) * r, 0, (float) Math.Sin(angle) * r);
                Vector3 pos2 = new Vector3((float) Math.Cos(angle2) * r, 0, (float) Math.Sin(angle2) * r);
                Gizmos.DrawLine(pos1, pos2);
            }
        }

        public static void fillArc3d(int count, float r, float from, float to) {
            initMat();
            mat.SetPass(0);

            GL.PushMatrix();
            GL.LoadProjectionMatrix(Camera.current.projectionMatrix);
            GL.MultMatrix(Gizmos.matrix);

            GL.Begin(GL.TRIANGLES);
            GL.Color(Gizmos.color);
            float arcLen = to - from;
            for (float i = 0; i < count; i++) {
                float angle = from + arcLen / count * i;
                float angle2 = from + arcLen / count * (i + 1);
                Vector3 pos1 = new Vector3((float) Math.Cos(angle) * r, 0, (float) Math.Sin(angle) * r);
                Vector3 pos2 = new Vector3((float) Math.Cos(angle2) * r, 0, (float) Math.Sin(angle2) * r);
                //Gizmos.DrawLine(pos1, pos2);
                
                
                GL.Vertex(Vector3.zero);
                GL.Vertex(pos1);
                GL.Vertex(pos2);
                
            }
            GL.End();
            GL.PopMatrix();
        }

        public static void removeAllChildrenImmediate(this GameObject THIS, bool undo) {//TODO fix undo
            List<GameObject> children = new List<GameObject>();
            foreach (Transform child in THIS.transform) children.Add(child.gameObject);
            children.ForEach(child => {
                if (undo) {
//                    Undo.DestroyObjectImmediate(child);
                } else {
                    Object.DestroyImmediate(child);
                }
            });
        }
    }
}
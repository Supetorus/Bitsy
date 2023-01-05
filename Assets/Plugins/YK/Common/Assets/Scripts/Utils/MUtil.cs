using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace moveen.utils {
    public static class MUtil {
        private static bool UNITY_EVENTS_LOG = false;
        private static bool UNITY_EVENTS_LOG_FILE = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <param name="eventName"></param>
        /// <param name="simple">if true, then we don't request .transform from MonoBehavior (it is constructor event, for example)</param>
        public static void logEvent(object component, String eventName, bool simple = false) {
            if (UNITY_EVENTS_LOG) {
                log(component, eventName, simple, "event: ");
            }
        }

        public static void log(object component, string eventName, bool simple = false, string prefix = "") {
            if (component is MonoBehaviour && !simple) {
                GameObject go = ((MonoBehaviour) component).gameObject;

                string path = composeHierarchyPath(go);
                log(prefix + "<b><color=blue>" + eventName + "</color></b> " + component.GetType().Name + " <color=green>" + path + "</color>" + "        <color=grey>" + (Application.isPlaying ? "playing" : "editing") + "</color>",
                    prefix + eventName + " " + component.GetType().Name + " " + path
                    );
            } else if (component is string) {
                log(prefix + "<b><color=blue>" + eventName + "</color></b> " + component + "        <color=grey>" + (Application.isPlaying ? "playing" : "editing") + "</color>",
                    prefix + eventName + " " + component);
            } else {
                log(prefix + "<b><color=blue>" + eventName + "</color></b> " + component.GetType().Name,
                    prefix + eventName + " " + component.GetType().Name);
            }
        }

//        public static System.IO.StreamWriter file = new System.IO.StreamWriter("/home/yuri/unity.log.txt");

        private static string lastString;
        private static bool repeatShowed;

        public static void log(string console, string s) {
            if (s.Equals(lastString)) {
                if (!repeatShowed) {
                    Debug.Log("...");
//                    file.WriteLine("...");
//                    file.Flush();
                }
                repeatShowed = true;
            } else {
                repeatShowed = false;
                lastString = s;
                Debug.Log(console);
//                file.WriteLine(s);
//                file.Flush();
            }
        }

        private static string composeHierarchyPath(GameObject go) {
            string path = "";
            while (go != null) {
                path += "<-" + go.name;
                go = go.transform.parent == null ? null : go.transform.parent.gameObject;
            }
            return path;
        }


//fan
//            if (v1l > 0.01f && v2l > 0.01f) {
//                Vector3 X = vec1.crossProduct(vec2).normalized * v2l * 0.2f;
//                Vector3 Y = vec1.crossProduct(X).normalized * v2l * 0.2f;
//                for (int i = 0; i <= 10; i++) {
//                    Vector3 pos = transform.position + X * (float)Math.Cos(i/10f * Math.PI) - Y * (float)Math.Sin(i/10f * Math.PI);
//                    Gizmos.DrawLine(transform.position, pos);
//                    if (i < 10) {
//                        Vector3 pos2 = transform.position + X * (float)Math.Cos((i+1)/10f * Math.PI) - Y * (float)Math.Sin((i+1)/10f * Math.PI);
//                        Gizmos.DrawLine(pos, pos2);
//                    }
//                }
//            }

        public static Quaternion toAngleAxis(float angle, Vector3 axis) {
            return Quaternion.AngleAxis((float) (angle / Math.PI * 180), axis);
        }

        public static float circleCurve(float progress) {
            var result = (float) Math.Sin(Math.Acos(MyMath.clamp(progress, 0, 1) * 2 - 1));
            if (Single.IsNaN(result)) {
                Debug.Log("circleCurve nan for " + progress);
            }
            return result;
        }

        public static Quaternion qToAxes(Vector3 X, Vector3 Y, Vector3 Z) {
            return qToAxes(X.x, X.y, X.z, Y.x, Y.y, Y.z, Z.x, Z.y, Z.z);
        }

        public static Quaternion qToAxes(Vector3 X, Vector3 Y) {
            return qToAxesXY(X, Y);
        }

        //Calculate rotation needed to achieve origin built by these X and Y (Z is delivered automatically)
        //  notice that Y may change direction as it's used only for Z calculation
        public static Quaternion qToAxesXY(Vector3 X, Vector3 Y) {
            X = X.normalized();
            Vector3 Z = X.crossProduct(Y).normalized();
            Y = Z.crossProduct(X).normalized();
            return qToAxes(X.x, X.y, X.z, Y.x, Y.y, Y.z, Z.x, Z.y, Z.z);
        }

        //Just the same as qToAxesXY, but Y is main here (Y don't changes direction and X used only for Z calculation)
        public static Quaternion qToAxesYX(Vector3 Y, Vector3 X) {
            Y = Y.normalized();
            Vector3 Z = X.crossProduct(Y).normalized();
            X = Y.crossProduct(Z).normalized();
            return qToAxes(X.x, X.y, X.z, Y.x, Y.y, Y.z, Z.x, Z.y, Z.z);
        }

        public static Quaternion qToAxesXZ(Vector3 X, Vector3 Z) {
            X = X.normalized();
            Vector3 Y = Z.crossProduct(X).normalized();
            Z = X.crossProduct(Y).normalized();
            return qToAxes(X.x, X.y, X.z, Y.x, Y.y, Y.z, Z.x, Z.y, Z.z);
        }

        public static Quaternion qToAxesZY(Vector3 Z, Vector3 Y) {
            Z = Z.normalized();
            Vector3 X = Y.crossProduct(Z).normalized();
            Y = Z.crossProduct(X).normalized();
            return qToAxes(X.x, X.y, X.z, Y.x, Y.y, Y.z, Z.x, Z.y, Z.z);
        }

        public static Quaternion qToAxes(float xx, float xy, float xz, float yx, float yy, float yz, float zx, float zy, float zz) {
//            float trace = xx + yy + zz;
//            if (trace >= 0) {
//                float s1 = MyMath.sqrt(trace + 1);
//                float s2 = 0.5f / s1;
//                return new Quaternion(-(zy - yz) * s2, -(xz - zx) * s2, -(yx - xy) * s2, 0.5f * s1);
//            }
//            else if (xx > yy && xx > zz) {
//                float s1 = MyMath.sqrt(1 + xx - yy - zz);
//                float s2 = 0.5f / s1;
//                return new Quaternion(-s1 * 0.5f, -(yx + xy) * s2, -(xz + zx) * s2, (zy - yz) * s2);
//            }
//            else if (yy > zz) {
//                float s1 = MyMath.sqrt(1 + yy - xx - zz);
//                float s2 = 0.5f / s1;
//                return new Quaternion(-(yx + xy) * s2, -s1 * 0.5f, -(zy + yz) * s2, (xz - zx) * s2);
//            }
//            else {
//                float s1 = MyMath.sqrt(1 + zz - xx - yy);
//                float s2 = 0.5f / s1;
//                return new Quaternion(-(xz + zx) * s2, -(zy + yz) * s2, -s1 * 0.5f, (yx - xy) * s2);
////                return new Quaternion((yx - xy) * s2, (xz + zx) * s2, (zy + yz) * s2, s1 * 0.5f);
//            }


//            Quaternion q = new Quaternion();
//            q.w = Mathf.Sqrt( Mathf.Max( 0, 1 + xx + yy + zz ) ) / 2;
//            q.x = Mathf.Sqrt( Mathf.Max( 0, 1 + xx - yy - zz ) ) / 2;
//            q.y = Mathf.Sqrt( Mathf.Max( 0, 1 - xx + yy - zz ) ) / 2;
//            q.z = Mathf.Sqrt( Mathf.Max( 0, 1 - xx - yy + zz ) ) / 2;
//            q.x *= -Mathf.Sign( q.x * ( zy - yz ) );
//            q.y *= -Mathf.Sign( q.y * ( xz - zx ) );
//            q.z *= -Mathf.Sign( q.z * ( yx - xy ) );
//            return q;


            Vector3 forward = new Vector3(zx, zy, zz);
            Vector3 upwards = new Vector3(yx, yy, yz);
            //TODO find and fix

            if (forward.length() < 0.0001f || upwards.length() < 0.0001f) return Quaternion.identity;
            return Quaternion.LookRotation(forward, upwards);
        }

        //DON'T USE, unity warns about wrong quaternions (0.9999 len, not close enough to 1) 
        public static Quaternion matrix2quaternion(Vector3 X, Vector3 Y, Vector3 Z) {
            return matrix2quaternion(X.x, Y.x, Z.x, X.y, Y.y, Z.y, X.z, Y.z, Z.z);
        }

        //http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/
        public static Quaternion matrix2quaternion(float m00, float m01, float m02,
            float m10, float m11, float m12,
            float m20, float m21, float m22) {
            float tr = m00 + m11 + m22;

            float qw, qx, qy, qz;
            if (tr > 0) {
                float S = (float) (Math.Sqrt(tr + 1.0) * 2); // S=4*qw
                qw = 0.25f * S;
                qx = (m21 - m12) / S;
                qy = (m02 - m20) / S;
                qz = (m10 - m01) / S;
            } else if ((m00 > m11) & (m00 > m22)) {
                float S = (float) (Math.Sqrt(1.0 + m00 - m11 - m22) * 2); // S=4*qx
                qw = (m21 - m12) / S;
                qx = 0.25f * S;
                qy = (m01 + m10) / S;
                qz = (m02 + m20) / S;
            } else if (m11 > m22) {
                float S = (float) Math.Sqrt(1.0 + m11 - m00 - m22) * 2; // S=4*qy
                qw = (m02 - m20) / S;
                qx = (m01 + m10) / S;
                qy = 0.25f * S;
                qz = (m12 + m21) / S;
            } else {
                float S = (float) Math.Sqrt(1.0 + m22 - m00 - m11) * 2; // S=4*qz
                qw = (m10 - m01) / S;
                qx = (m02 + m20) / S;
                qy = (m12 + m21) / S;
                qz = 0.25f * S;
            }
            return new Quaternion(qx, qy, qz, qw);
        }

        public static Vector3 torqueFromQuaternions(Quaternion currentRot, Quaternion wantedRot, Vector3 currentRotSpeed, float angleToSpeed, float speedDifToAccel, float dt) {
            Quaternion rotSub = wantedRot.rotSubDeprecated(currentRot);
            if (rotSub.w < 0) rotSub = rotSub.mul(-1); //big jump without it eventually
            Vector3 wantedAngularVelocity = rotSub.imaginary() * angleToSpeed;
            return (wantedAngularVelocity - currentRotSpeed) *
                   speedDifToAccel; // * 0.1 - to give rotation more weight (?)
        }

        public static Quaternion quaternionFromTorque(Vector3 angular) {
            float len = angular.length();
            if (len == 0) return Quaternion.identity;
            return new Quaternion(angular.x, angular.y, angular.z, (float) Math.Cos(len / 2)).normalizeWithFixedW();
        }

        public static void madeCount<T>(List<T> tt, int needed) where T : new() {
            if (tt.Count > needed) tt.RemoveRange(needed, tt.Count - needed);
            while (tt.Count < needed) tt.Add(new T());
        }

        public static void madeCount<T>(List<T> tt, int needed, T t) {
            if (tt.Count > needed) tt.RemoveRange(needed, tt.Count - needed);
            while (tt.Count < needed) tt.Add(t);
        }

        private static readonly Dictionary<Type, Dictionary<Type, List<FieldInfo>>> fieldByAttribute = new Dictionary<Type, Dictionary<Type, List<FieldInfo>>>();

        public static List<FieldInfo> getFieldsWhereAttributes(Type type, Type attributeType) {
            return fieldByAttribute
                .getOrPut(type, () => new Dictionary<Type, List<FieldInfo>>())
                .getOrPut(attributeType, () => extractFields(type, attributeType));
        }

        private static List<FieldInfo> extractFields(Type type, Type attributeType) {
            return type.GetFields().Where(fi => fi.GetCustomAttributes(attributeType, true).Length != 0).ToList();
        }
    }
}

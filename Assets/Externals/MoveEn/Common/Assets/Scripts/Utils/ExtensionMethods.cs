using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace moveen.utils {
    //TODO provide unit tests
    public static class ExtensionMethods {

        /// <summary>
        /// Get the value by key from Dictionary. If it doesn't contain the key, initialize (Add) it with a call 'defaultValueProvider' and return its result.
        /// Each subsequent call with the same key will return once calculated value.
        /// </summary>
        public static TValue getOrPut<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> defaultValueProvider) {
            if (dictionary.ContainsKey(key)) return dictionary[key];
            TValue v = defaultValueProvider();
            dictionary.Add(key, v);
            return v;
        }
        
        public static Vector3 limit(this Vector3 THIS, float max) {
            float l = THIS.length();
            return l > max ? THIS.normalized(max) : THIS;
        }

        public static Vector3 minRadius(this Vector3 THIS, float max) {
            float l = THIS.length();
            return l < max && l > 0.01f ? THIS.normalized(max) : THIS;
        }

        public static Vector3 clamp(this Vector3 THIS, float min, float max) {
            float l = THIS.length();
            if (l > max) return THIS.normalized(max);
            if (l < min && l > 0.001f) return THIS.normalized(min);
            return THIS;
        }

        public static Vector3 clampAround(this Vector3 THIS, Vector3 center, float min, float max) {
            return THIS.sub(center).clamp(min, max).add(center);
        }

        public static Vector3 limit(this Vector3 THIS, Vector3 center, float max) {
            return (THIS - center).limit(max) + center;
        }

        public static Vector3 getXForHorizontalAxis(this Vector3 THIS, Vector3 normalizedAxis) {
            return normalizedAxis.mul(normalizedAxis.scalarProduct(THIS));
        }

        public static Vector3 getYForHorizontalAxis(this Vector3 THIS, Vector3 normalizedAxis) {
            return THIS.sub(THIS.getXForHorizontalAxis(normalizedAxis));
        }

        public static Vector3 getXForVerticalAxis(this Vector3 THIS, Vector3 normal) {
            return THIS.getYForHorizontalAxis(normal);
        }

        public static Vector3 getYForVerticalAxis(this Vector3 THIS, Vector3 normal) {
            return THIS.getXForHorizontalAxis(normal);
        }

        public static Vector3 normalTo(this Vector3 THIS, Vector3 normalizedAxis) {
            Vector3 normal = THIS.getYForHorizontalAxis(normalizedAxis);
            if (normal.sqrMagnitude > 0.00001f) {
                return normal.normalized();
            }
            throw BadException.die("");
        }

        public static Vector2 getXy(this Vector3 v3) {
            return new Vector2(v3.x, v3.y);
        }

        public static Vector2 getXz(this Vector3 v3) {
            return new Vector2(v3.x, v3.z);
        }

        public static Vector3 withY(this Vector2 v2, float y) {
            return new Vector3(v2.x, y, v2.y);
        }

        public static Vector3 withZ(this Vector2 v2, float z) {
            return new Vector3(v2.x, v2.y, z);
        }

        public static Vector3 add(this Vector3 a, Vector3 b) {
            return a + b;
        }

        public static Vector3 add(this Vector3 a, float x, float y, float z) {
            return new Vector3(a.x + x, a.y + y, a.z + z);
        }

        public static Vector3 sub(this Vector3 a, Vector3 b) {
            return a - b;
        }

        public static Vector3 sub(this Vector3 a, float x, float y, float z) {
            return new Vector3(a.x - x, a.y - y, a.z - z);
        }

        public static float dot(this Vector3 a, Vector3 b) {
            return Vector3.Dot(a, b);
        }

        public static Vector3 normalized(this Vector3 a) {
            return Vector3.Normalize(a);
        }

        public static Vector3 normalized(this Vector3 a, float newLen) {
            return Vector3.Normalize(a) * newLen;
        }

        public static float length(this Vector3 a) {
            return Vector3.Magnitude(a);
        }

        public static Vector3 withSetX(this Vector3 a, float value) {
            return new Vector3(value, a.y, a.z);
        }

        public static Vector3 withSetY(this Vector3 a, float value) {
            return new Vector3(a.x, value, a.z);
        }

        public static Vector3 withSetZ(this Vector3 a, float value) {
            return new Vector3(a.x, a.y, value);
        }

        public static float scalarProduct(this Vector3 a, Vector3 b) {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static Vector3 mul(this Vector3 a, Vector3 b) {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static Vector3 mul(this Vector3 a, float x, float y, float z) {
            return new Vector3(a.x * x, a.y * y, a.z * z);
        }

        public static Vector3 pow(this Vector3 a, float x, float y, float z) {
            return new Vector3(MyMath.pow(a.x, x), MyMath.pow(a.y, y), MyMath.pow(a.z, z));
        }

        public static Vector3 mul(this Vector3 a, float b) {
            return a * b;
        }

        public static Vector3 div(this Vector3 a, float b) {
            return a / b;
        }

        public static Vector3 div(this Vector3 a, Vector3 b) {
            return new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);
        }

        public static Vector3 crossProduct(this Vector3 a, Vector3 b) {
            return Vector3.Cross(a, b);
        }

        public static float dist(this Vector3 a, Vector3 b) {
            return Vector3.Distance(a, b);
        }

        public static Vector3 mix(this Vector3 a, Vector3 b, float progress) {
            return (b - a) * progress + a;
        }


        public static void setTranslation(this Matrix4x4 from, Vector3 tr) {
            BadException.notImplemented();
        }

        public static T car<T>(this List<T> list) {
            return list[0];
        }

        public static T cadr<T>(this List<T> list) {
            return list[1];
        }

//        public delegate R Function<T, R>(T t);
//
//        public delegate int Comp<T>(T t1, T t2);
//
//        public static T max<T, R>(this List<T> source, Function<T, R> evaluator) where R : IComparable {
//            return source.max((t1, t2) => evaluator(t1).CompareTo(evaluator(t2)));
//        }

//        public static T max<T>(this List<T> source, Comp<T> comparator) {
////                if (source.isEmpty()) throw new RuntimeException("can't get max on empty collection");
//            T result = default(T);
//            foreach (T t in source) {
//                if (result == null || comparator(t, result) > 0) result = t;
//            }
//            return result;
//        }


        //     https://github.com/Kent-H/blue3D/blob/master/Blue3D/src/blue3D/type/QuaternionF.java
        public static Quaternion ln(this Quaternion THIS) {
            Quaternion result = new Quaternion();
            float r = (float) Math.Sqrt(THIS.x * THIS.x + THIS.y * THIS.y + THIS.z * THIS.z);
            float t = r > 0.00001f ? (float) Math.Atan2(r, THIS.w) / r : 0.0f;
            return new Quaternion(THIS.x * t, THIS.y * t, THIS.z * t,
                0.5f * (float) Math.Log(THIS.w * THIS.w + THIS.x * THIS.x + THIS.y * THIS.y + THIS.z * THIS.z));
        }

        public static Quaternion exp(this Quaternion THIS) {
            float r = (float) Math.Sqrt(THIS.x * THIS.x + THIS.y * THIS.y + THIS.z * THIS.z);
            float et = (float) Math.Exp(THIS.w);
            float s = r >= 0.00001f ? et * (float) Math.Sin(r) / r : 0f;
            return new Quaternion(THIS.x * s, THIS.y * s, THIS.z * s, et * (float) Math.Cos(r));
        }

        public static Quaternion pow(this Quaternion THIS, float n) {
            return THIS.ln().mul(n).exp();
        }

        public static Quaternion rotSubDeprecated(this Quaternion THIS, Quaternion from) {
            return THIS.mul(from.conjug());
//                return from.conjug().mul(THIS);
        }

        public static Quaternion rotSub(this Quaternion THIS, Quaternion from) {
            return from.conjug().mul(THIS);
        }

        public static Quaternion mul(this Quaternion THIS, float scale) {
            return new Quaternion(THIS.x * scale, THIS.y * scale, THIS.z * scale, THIS.w * scale);
        }

        public static Quaternion normalizeWithFixedW(this Quaternion THIS) {
            if (THIS.w > 1 || THIS.w < -1) throw new BadException("wrong w in " + THIS + " (" + THIS.w + ")");
            Vector3 xyz = new Vector3(THIS.x, THIS.y, THIS.z);
            xyz = xyz.normalized((float) Math.Sqrt(1 - THIS.w * THIS.w));
            return new Quaternion(xyz.x, xyz.y, xyz.z, THIS.w);
        }

        public static Quaternion normalizeWithFixedW(this Quaternion THIS, float w) {
            return THIS.withSetW(w).normalizeWithFixedW();
        }

        public static Quaternion withSetW(this Quaternion THIS, float w) {
            return new Quaternion(THIS.x, THIS.y, THIS.z, w);
        }

        public static Quaternion conjug(this Quaternion THIS) {
            return new Quaternion(-THIS.x, -THIS.y, -THIS.z, THIS.w);
        }
        
        public static Quaternion sub(this Quaternion THIS, Quaternion b) {
            return new Quaternion(THIS.x - b.x, THIS.y - b.y, THIS.z - b.z, THIS.w - b.w);
        }

        public static Quaternion add(this Quaternion THIS, Quaternion b) {
            return new Quaternion(THIS.x + b.x, THIS.y + b.y, THIS.z + b.z, THIS.w + b.w);
        }

        public static Quaternion mix(this Quaternion a, Quaternion b, float progress) {
            return b.sub(a).mul(progress).add(a);
        }

        public static Vector3 imaginary(this Quaternion q) {
            return new Vector3(q.x, q.y, q.z);
        }

        public static Vector3 rotate(this Quaternion THIS, Vector3 vector) {
//            float newVar_7 = -(THIS.x * vector.x) + -(THIS.y * vector.y) + -(THIS.z * vector.z);
//            float newVar_22 = THIS.w * vector.x + THIS.y * vector.z + -(THIS.z * vector.y);
//            float newVar_37 = THIS.w * vector.y + THIS.z * vector.x + -(THIS.x * vector.z);
//            float newVar_52 = THIS.w * vector.z + THIS.x * vector.y + -(THIS.y * vector.x);
//            float mx = -THIS.x;
//            float my = -THIS.y;
//            float mz = -THIS.z;
//            return new Vector3(
//                newVar_7 * mx + newVar_22 * THIS.w + newVar_37 * mz + -(newVar_52 * my),
//                newVar_7 * my + newVar_37 * THIS.w + newVar_52 * mx + -(newVar_22 * mz),
//                newVar_7 * mz + newVar_52 * THIS.w + newVar_22 * my + -(newVar_37 * mx));
            
            
            return THIS * vector;
        }

        public static float magnitude(this Quaternion q) {
            return (float) Math.Sqrt(q.norm());
        }

        public static float norm(this Quaternion q) {
            return q.w * q.w + q.x * q.x + q.y * q.y + q.z * q.z;
        }

        public static Quaternion normalized(this Quaternion q) {
            float mag = 1.0f / q.magnitude();
            return new Quaternion(q.x * mag, q.y * mag, q.z * mag, q.w * mag);
        }

        public static Quaternion nlerp(this Quaternion from, Quaternion to, float blend) {
            return Quaternion.Lerp(from, to, blend);
        }

        public static Quaternion mul(this Quaternion a, Quaternion b) {
            return a * b;
        }

        public static void fillMatrix4(this Quaternion q, Matrix4x4 target) {
            BadException.notImplemented();
        }
    }
}
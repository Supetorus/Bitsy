using UnityEngine;
using System;
namespace moveen.utils {
    public class MyMath {

        public static float PI = (float)(Math.PI);

        public static int module(int v, int m) {//TODO remove
            int res = v % m;
            return res < 0 ? res + m : res;
        }
        public static int cycle(int v, int m) {
            int res = v % m;
            return res < 0 ? res + m : res;
        }
        public static float module(float v, float m) {//TODO rename, get rid
            float res = v % m;
            return res < 0 ? res + m : res;
        }
        public static float cycle(float v, float m) {
            float res = v % m;
            return res < 0 ? res + m : res;
        }
        public static int floorBy(int x, int by) {
            return (int)((Math.Floor((float)x / by) * by));
        }
        public static int floorBy(float x, float by) {
            return (int)(((Math.Floor(x / by) * by)));
        }
        public static int max(int arg0, int arg1) {
            return arg0 > arg1 ? arg0 : arg1;
        }
        public static int min(int arg0, int arg1) {
            return arg0 < arg1 ? arg0 : arg1;
        }
        public static float to01(float value, float from, float to) {
            return ((value - from)) / ((to - from));
        }

        public static float toRadians(float deg) {
            return deg * PI / 180f;
        }

        public static float regionMap(float value, float srcFrom, float srcTo, float dstFrom, float dstTo) {
            return (value - srcFrom) / (srcTo - srcFrom) * (dstTo - dstFrom) + dstFrom;
        }
        
        public static float regionMapClamp(float value, float srcFrom, float srcTo, float dstFrom, float dstTo) {
            float res = regionMap(value, srcFrom, srcTo, dstFrom, dstTo);
            return dstFrom < dstTo ? clamp(res, dstFrom, dstTo) : clamp(res, dstTo, dstFrom);
        }
        
        public static void mix(Vector3 from, Vector3 to, float progress, Vector3 res) {
            res.x = mix(from.x, to.x, progress);
            res.y = mix(from.y, to.y, progress);
            res.z = mix(from.z, to.z, progress);
        }
        public static float angleNormalize02PI(float a) {
            return module(a, (2 * PI));
        }
        public static float angleNormalizeSigned(float a) {
            a = angleNormalize02PI(a);
            if ((a > PI))  {
                a = (a - (2 * PI));
            }
            return a;
        }
        public static float mixAngle02PI(float a, float b, float progress) {
            if ((b > a))  {
                return ((((b - a) > PI)) ? (angleNormalize02PI(mix((a + (2 * PI)), b, progress))) : (mix(a, b, progress)));
            }
            return ((((a - b) > PI)) ? (angleNormalize02PI(mix(a, (b + (2 * PI)), progress))) : (mix(a, b, progress)));
        }
        public static float angleDif02PI(float a, float b) {
            if ((b > a))  {
                return ((((b - a) > PI)) ? (((a + (2 * PI)) - b)) : ((b - a)));
            }
            return ((((a - b) > PI)) ? (((b + (2 * PI)) - a)) : ((a - b)));
        }
        public static float circleCurve(float progress) {
            return (float)(sin(acos(((clamp(progress, 0, 1) * 2) - 1))));
        }
        public static Vector2 plus(Vector2 arg0, Vector2 arg1) {
            return new Vector2(((float)(arg0.x) + (float)(arg1.x)), ((float)(arg0.y) + (float)(arg1.y)));
        }
        public static Vector3 plus(Vector3 arg0, Vector3 arg1) {
            return new Vector3(((float)(arg0.x) + (float)(arg1.x)), ((float)(arg0.y) + (float)(arg1.y)), ((float)(arg0.z) + (float)(arg1.z)));
        }
        public static Vector4 plus(Vector4 arg0, Vector4 arg1) {
            return new Vector4(
                ((float)(arg0.x) + (float)(arg1.x)), 
                ((float)(arg0.y) + (float)(arg1.y)), 
                ((float)(arg0.z) + (float)(arg1.z)), 
                ((float)(arg0.w) + (float)(arg1.w)));
        }
        public static Vector2 plus(float arg0, Vector2 arg1) {
            return new Vector2(((float)(arg0) + (float)(arg1.x)), ((float)(arg0) + (float)(arg1.y)));
        }
        public static Vector3 plus(float arg0, Vector3 arg1) {
            return new Vector3(((float)(arg0) + (float)(arg1.x)), ((float)(arg0) + (float)(arg1.y)), ((float)(arg0) + (float)(arg1.z)));
        }
        public static Vector4 plus(float arg0, Vector4 arg1) {
            return new Vector4(
                ((float)(arg0) + (float)(arg1.x)), 
                ((float)(arg0) + (float)(arg1.y)), 
                ((float)(arg0) + (float)(arg1.z)), 
                ((float)(arg0) + (float)(arg1.w)));
        }
        public static Vector2 plus(Vector2 arg0, float arg1) {
            return new Vector2(((float)(arg0.x) + (float)(arg1)), ((float)(arg0.y) + (float)(arg1)));
        }
        public static Vector3 plus(Vector3 arg0, float arg1) {
            return new Vector3(((float)(arg0.x) + (float)(arg1)), ((float)(arg0.y) + (float)(arg1)), ((float)(arg0.z) + (float)(arg1)));
        }
        public static Vector4 plus(Vector4 arg0, float arg1) {
            return new Vector4(
                ((float)(arg0.x) + (float)(arg1)), 
                ((float)(arg0.y) + (float)(arg1)), 
                ((float)(arg0.z) + (float)(arg1)), 
                ((float)(arg0.w) + (float)(arg1)));
        }
        public static Vector2 minus(Vector2 arg0, Vector2 arg1) {
            return new Vector2(((float)(arg0.x) - (float)(arg1.x)), ((float)(arg0.y) - (float)(arg1.y)));
        }
        public static Vector3 minus(Vector3 arg0, Vector3 arg1) {
            return new Vector3(((float)(arg0.x) - (float)(arg1.x)), ((float)(arg0.y) - (float)(arg1.y)), ((float)(arg0.z) - (float)(arg1.z)));
        }
        public static Vector4 minus(Vector4 arg0, Vector4 arg1) {
            return new Vector4(
                ((float)(arg0.x) - (float)(arg1.x)), 
                ((float)(arg0.y) - (float)(arg1.y)), 
                ((float)(arg0.z) - (float)(arg1.z)), 
                ((float)(arg0.w) - (float)(arg1.w)));
        }
        public static Vector2 minus(float arg0, Vector2 arg1) {
            return new Vector2(((float)(arg0) - (float)(arg1.x)), ((float)(arg0) - (float)(arg1.y)));
        }
        public static Vector3 minus(float arg0, Vector3 arg1) {
            return new Vector3(((float)(arg0) - (float)(arg1.x)), ((float)(arg0) - (float)(arg1.y)), ((float)(arg0) - (float)(arg1.z)));
        }
        public static Vector4 minus(float arg0, Vector4 arg1) {
            return new Vector4(
                ((float)(arg0) - (float)(arg1.x)), 
                ((float)(arg0) - (float)(arg1.y)), 
                ((float)(arg0) - (float)(arg1.z)), 
                ((float)(arg0) - (float)(arg1.w)));
        }
        public static Vector2 minus(Vector2 arg0, float arg1) {
            return new Vector2(((float)(arg0.x) - (float)(arg1)), ((float)(arg0.y) - (float)(arg1)));
        }
        public static Vector3 minus(Vector3 arg0, float arg1) {
            return new Vector3(((float)(arg0.x) - (float)(arg1)), ((float)(arg0.y) - (float)(arg1)), ((float)(arg0.z) - (float)(arg1)));
        }
        public static Vector4 minus(Vector4 arg0, float arg1) {
            return new Vector4(
                ((float)(arg0.x) - (float)(arg1)), 
                ((float)(arg0.y) - (float)(arg1)), 
                ((float)(arg0.z) - (float)(arg1)), 
                ((float)(arg0.w) - (float)(arg1)));
        }
        public static Vector2 multiply(Vector2 arg0, Vector2 arg1) {
            return new Vector2(((float)(arg0.x) * (float)(arg1.x)), ((float)(arg0.y) * (float)(arg1.y)));
        }
        public static Vector3 multiply(Vector3 arg0, Vector3 arg1) {
            return new Vector3(((float)(arg0.x) * (float)(arg1.x)), ((float)(arg0.y) * (float)(arg1.y)), ((float)(arg0.z) * (float)(arg1.z)));
        }
        public static Vector4 multiply(Vector4 arg0, Vector4 arg1) {
            return new Vector4(
                ((float)(arg0.x) * (float)(arg1.x)), 
                ((float)(arg0.y) * (float)(arg1.y)), 
                ((float)(arg0.z) * (float)(arg1.z)), 
                ((float)(arg0.w) * (float)(arg1.w)));
        }
        public static Vector2 multiply(float arg0, Vector2 arg1) {
            return new Vector2(((float)(arg0) * (float)(arg1.x)), ((float)(arg0) * (float)(arg1.y)));
        }
        public static Vector3 multiply(float arg0, Vector3 arg1) {
            return new Vector3(((float)(arg0) * (float)(arg1.x)), ((float)(arg0) * (float)(arg1.y)), ((float)(arg0) * (float)(arg1.z)));
        }
        public static Vector4 multiply(float arg0, Vector4 arg1) {
            return new Vector4(
                ((float)(arg0) * (float)(arg1.x)), 
                ((float)(arg0) * (float)(arg1.y)), 
                ((float)(arg0) * (float)(arg1.z)), 
                ((float)(arg0) * (float)(arg1.w)));
        }
        public static Vector2 multiply(Vector2 arg0, float arg1) {
            return new Vector2(((float)(arg0.x) * (float)(arg1)), ((float)(arg0.y) * (float)(arg1)));
        }
        public static Vector3 multiply(Vector3 arg0, float arg1) {
            return new Vector3(((float)(arg0.x) * (float)(arg1)), ((float)(arg0.y) * (float)(arg1)), ((float)(arg0.z) * (float)(arg1)));
        }
        public static Vector4 multiply(Vector4 arg0, float arg1) {
            return new Vector4(
                ((float)(arg0.x) * (float)(arg1)), 
                ((float)(arg0.y) * (float)(arg1)), 
                ((float)(arg0.z) * (float)(arg1)), 
                ((float)(arg0.w) * (float)(arg1)));
        }
        public static Vector2 div(Vector2 arg0, Vector2 arg1) {
            return new Vector2((float)(arg0.x) / (float)(arg1.x), (float)(arg0.y) / (float)(arg1.y));
        }
        public static Vector3 div(Vector3 arg0, Vector3 arg1) {
            return new Vector3((float)(arg0.x) / (float)(arg1.x), (float)(arg0.y) / (float)(arg1.y), (float)(arg0.z) / (float)(arg1.z));
        }
        public static Vector4 div(Vector4 arg0, Vector4 arg1) {
            return new Vector4(
                (float)(arg0.x) / (float)(arg1.x), 
                (float)(arg0.y) / (float)(arg1.y), 
                (float)(arg0.z) / (float)(arg1.z), 
                (float)(arg0.w) / (float)(arg1.w));
        }
        public static Vector2 div(float arg0, Vector2 arg1) {
            return new Vector2((float)(arg0) / (float)(arg1.x), (float)(arg0) / (float)(arg1.y));
        }
        public static Vector3 div(float arg0, Vector3 arg1) {
            return new Vector3((float)(arg0) / (float)(arg1.x), (float)(arg0) / (float)(arg1.y), (float)(arg0) / (float)(arg1.z));
        }
        public static Vector4 div(float arg0, Vector4 arg1) {
            return new Vector4(
                (float)(arg0) / (float)(arg1.x), 
                (float)(arg0) / (float)(arg1.y), 
                (float)(arg0) / (float)(arg1.z), 
                (float)(arg0) / (float)(arg1.w));
        }
        public static Vector2 div(Vector2 arg0, float arg1) {
            return new Vector2((float)(arg0.x) / (float)(arg1), (float)(arg0.y) / (float)(arg1));
        }
        public static Vector3 div(Vector3 arg0, float arg1) {
            return new Vector3((float)(arg0.x) / (float)(arg1), (float)(arg0.y) / (float)(arg1), (float)(arg0.z) / (float)(arg1));
        }
        public static Vector4 div(Vector4 arg0, float arg1) {
            return new Vector4(
                (float)(arg0.x) / (float)(arg1), 
                (float)(arg0.y) / (float)(arg1), 
                (float)(arg0.z) / (float)(arg1), 
                (float)(arg0.w) / (float)(arg1));
        }
        public static float radians(float arg0) {
            return (float)(((arg0 / 180 * Math.PI)));
        }
        public static Vector2 radians(Vector2 arg0) {
            return new Vector2((float)(((arg0.x / 180 * Math.PI))), (float)(((arg0.y / 180 * Math.PI))));
        }
        public static Vector3 radians(Vector3 arg0) {
            return new Vector3((float)(((arg0.x / 180 * Math.PI))), (float)(((arg0.y / 180 * Math.PI))), (float)(((arg0.z / 180 * Math.PI))));
        }
        public static Vector4 radians(Vector4 arg0) {
            return new Vector4(
                (float)(((arg0.x / 180 * Math.PI))), 
                (float)(((arg0.y / 180 * Math.PI))), 
                (float)(((arg0.z / 180 * Math.PI))), 
                (float)(((arg0.w / 180 * Math.PI))));
        }
        public static float degrees(float arg0) {
            return (float)(((arg0 / Math.PI * 180)));
        }
        public static Vector2 degrees(Vector2 arg0) {
            return new Vector2((float)(((arg0.x / Math.PI * 180))), (float)(((arg0.y / Math.PI * 180))));
        }
        public static Vector3 degrees(Vector3 arg0) {
            return new Vector3((float)(((arg0.x / Math.PI * 180))), (float)(((arg0.y / Math.PI * 180))), (float)(((arg0.z / Math.PI * 180))));
        }
        public static Vector4 degrees(Vector4 arg0) {
            return new Vector4(
                (float)(((arg0.x / Math.PI * 180))), 
                (float)(((arg0.y / Math.PI * 180))), 
                (float)(((arg0.z / Math.PI * 180))), 
                (float)(((arg0.w / Math.PI * 180))));
        }
        public static float sin(float arg0) {
            return (float)(Math.Sin(arg0));
        }
        public static Vector2 sin(Vector2 arg0) {
            return new Vector2((float)(Math.Sin(arg0.x)), (float)(Math.Sin(arg0.y)));
        }
        public static Vector3 sin(Vector3 arg0) {
            return new Vector3((float)(Math.Sin(arg0.x)), (float)(Math.Sin(arg0.y)), (float)(Math.Sin(arg0.z)));
        }
        public static Vector4 sin(Vector4 arg0) {
            return new Vector4((float)(Math.Sin(arg0.x)), (float)(Math.Sin(arg0.y)), (float)(Math.Sin(arg0.z)), (float)(Math.Sin(arg0.w)));
        }
        public static float cos(float arg0) {
            return (float)(Math.Cos(arg0));
        }
        public static Vector2 cos(Vector2 arg0) {
            return new Vector2((float)(Math.Cos(arg0.x)), (float)(Math.Cos(arg0.y)));
        }
        public static Vector3 cos(Vector3 arg0) {
            return new Vector3((float)(Math.Cos(arg0.x)), (float)(Math.Cos(arg0.y)), (float)(Math.Cos(arg0.z)));
        }
        public static Vector4 cos(Vector4 arg0) {
            return new Vector4((float)(Math.Cos(arg0.x)), (float)(Math.Cos(arg0.y)), (float)(Math.Cos(arg0.z)), (float)(Math.Cos(arg0.w)));
        }
        public static float tan(float arg0) {
            return (float)(Math.Tan(arg0));
        }
        public static Vector2 tan(Vector2 arg0) {
            return new Vector2((float)(Math.Tan(arg0.x)), (float)(Math.Tan(arg0.y)));
        }
        public static Vector3 tan(Vector3 arg0) {
            return new Vector3((float)(Math.Tan(arg0.x)), (float)(Math.Tan(arg0.y)), (float)(Math.Tan(arg0.z)));
        }
        public static Vector4 tan(Vector4 arg0) {
            return new Vector4((float)(Math.Tan(arg0.x)), (float)(Math.Tan(arg0.y)), (float)(Math.Tan(arg0.z)), (float)(Math.Tan(arg0.w)));
        }
        public static float asin(float arg0) {
            return (float)(Math.Asin(arg0));
        }
        public static Vector2 asin(Vector2 arg0) {
            return new Vector2((float)(Math.Asin(arg0.x)), (float)(Math.Asin(arg0.y)));
        }
        public static Vector3 asin(Vector3 arg0) {
            return new Vector3((float)(Math.Asin(arg0.x)), (float)(Math.Asin(arg0.y)), (float)(Math.Asin(arg0.z)));
        }
        public static Vector4 asin(Vector4 arg0) {
            return new Vector4((float)(Math.Asin(arg0.x)), (float)(Math.Asin(arg0.y)), (float)(Math.Asin(arg0.z)), (float)(Math.Asin(arg0.w)));
        }
        public static float acos(float arg0) {
            return (float)(Math.Acos(arg0));
        }
        public static Vector2 acos(Vector2 arg0) {
            return new Vector2((float)(Math.Acos(arg0.x)), (float)(Math.Acos(arg0.y)));
        }
        public static Vector3 acos(Vector3 arg0) {
            return new Vector3((float)(Math.Acos(arg0.x)), (float)(Math.Acos(arg0.y)), (float)(Math.Acos(arg0.z)));
        }
        public static Vector4 acos(Vector4 arg0) {
            return new Vector4((float)(Math.Acos(arg0.x)), (float)(Math.Acos(arg0.y)), (float)(Math.Acos(arg0.z)), (float)(Math.Acos(arg0.w)));
        }
        public static float atan(float arg0) {
            return (float)(Math.Atan(arg0));
        }
        public static Vector2 atan(Vector2 arg0) {
            return new Vector2((float)(Math.Atan(arg0.x)), (float)(Math.Atan(arg0.y)));
        }
        public static Vector3 atan(Vector3 arg0) {
            return new Vector3((float)(Math.Atan(arg0.x)), (float)(Math.Atan(arg0.y)), (float)(Math.Atan(arg0.z)));
        }
        public static Vector4 atan(Vector4 arg0) {
            return new Vector4((float)(Math.Atan(arg0.x)), (float)(Math.Atan(arg0.y)), (float)(Math.Atan(arg0.z)), (float)(Math.Atan(arg0.w)));
        }
        public static float atan(float y, float x) {
            return (float)(Math.Atan2(y, x));
        }
        public static Vector2 atan(Vector2 y, Vector2 x) {
            return new Vector2((float)(Math.Atan2(y.x, x.x)), (float)(Math.Atan2(y.y, x.y)));
        }
        public static Vector3 atan(Vector3 y, Vector3 x) {
            return new Vector3((float)(Math.Atan2(y.x, x.x)), (float)(Math.Atan2(y.y, x.y)), (float)(Math.Atan2(y.z, x.z)));
        }
        public static Vector4 atan(Vector4 y, Vector4 x) {
            return new Vector4(
                (float)(Math.Atan2(y.x, x.x)), 
                (float)(Math.Atan2(y.y, x.y)), 
                (float)(Math.Atan2(y.z, x.z)), 
                (float)(Math.Atan2(y.w, x.w)));
        }
        public static float pow(float value, float power) {
            return (float)(Math.Pow(value, power));
        }
        public static Vector2 pow(Vector2 value, Vector2 power) {
            return new Vector2((float)(Math.Pow(value.x, power.x)), (float)(Math.Pow(value.y, power.y)));
        }
        public static Vector3 pow(Vector3 value, Vector3 power) {
            return new Vector3((float)(Math.Pow(value.x, power.x)), (float)(Math.Pow(value.y, power.y)), (float)(Math.Pow(value.z, power.z)));
        }
        public static Vector4 pow(Vector4 value, Vector4 power) {
            return new Vector4(
                (float)(Math.Pow(value.x, power.x)), 
                (float)(Math.Pow(value.y, power.y)), 
                (float)(Math.Pow(value.z, power.z)), 
                (float)(Math.Pow(value.w, power.w)));
        }
        public static float exp(float arg0) {
            return (float)(Math.Exp(arg0));
        }
        public static Vector2 exp(Vector2 arg0) {
            return new Vector2((float)(Math.Exp(arg0.x)), (float)(Math.Exp(arg0.y)));
        }
        public static Vector3 exp(Vector3 arg0) {
            return new Vector3((float)(Math.Exp(arg0.x)), (float)(Math.Exp(arg0.y)), (float)(Math.Exp(arg0.z)));
        }
        public static Vector4 exp(Vector4 arg0) {
            return new Vector4((float)(Math.Exp(arg0.x)), (float)(Math.Exp(arg0.y)), (float)(Math.Exp(arg0.z)), (float)(Math.Exp(arg0.w)));
        }
        public static float log(float arg0) {
            return (float)(Math.Log(arg0));
        }
        public static Vector2 log(Vector2 arg0) {
            return new Vector2((float)(Math.Log(arg0.x)), (float)(Math.Log(arg0.y)));
        }
        public static Vector3 log(Vector3 arg0) {
            return new Vector3((float)(Math.Log(arg0.x)), (float)(Math.Log(arg0.y)), (float)(Math.Log(arg0.z)));
        }
        public static Vector4 log(Vector4 arg0) {
            return new Vector4((float)(Math.Log(arg0.x)), (float)(Math.Log(arg0.y)), (float)(Math.Log(arg0.z)), (float)(Math.Log(arg0.w)));
        }
        public static float sqrt(float arg0) {
            return (float)(Math.Sqrt(arg0));
        }
        public static Vector2 sqrt(Vector2 arg0) {
            return new Vector2((float)(Math.Sqrt(arg0.x)), (float)(Math.Sqrt(arg0.y)));
        }
        public static Vector3 sqrt(Vector3 arg0) {
            return new Vector3((float)(Math.Sqrt(arg0.x)), (float)(Math.Sqrt(arg0.y)), (float)(Math.Sqrt(arg0.z)));
        }
        public static Vector4 sqrt(Vector4 arg0) {
            return new Vector4((float)(Math.Sqrt(arg0.x)), (float)(Math.Sqrt(arg0.y)), (float)(Math.Sqrt(arg0.z)), (float)(Math.Sqrt(arg0.w)));
        }
        public static float abs(float arg0) {
            return Math.Abs(arg0);
        }
        public static Vector2 abs(Vector2 arg0) {
            return new Vector2(Math.Abs(arg0.x), Math.Abs(arg0.y));
        }
        public static Vector3 abs(Vector3 arg0) {
            return new Vector3(Math.Abs(arg0.x), Math.Abs(arg0.y), Math.Abs(arg0.z));
        }
        public static Vector4 abs(Vector4 arg0) {
            return new Vector4(Math.Abs(arg0.x), Math.Abs(arg0.y), Math.Abs(arg0.z), Math.Abs(arg0.w));
        }
        public static float sign(float arg0) {
            return Math.Sign(arg0);
        }
        public static Vector2 sign(Vector2 arg0) {
            return new Vector2(Math.Sign(arg0.x), Math.Sign(arg0.y));
        }
        public static Vector3 sign(Vector3 arg0) {
            return new Vector3(Math.Sign(arg0.x), Math.Sign(arg0.y), Math.Sign(arg0.z));
        }
        public static Vector4 sign(Vector4 arg0) {
            return new Vector4(Math.Sign(arg0.x), Math.Sign(arg0.y), Math.Sign(arg0.z), Math.Sign(arg0.w));
        }
        public static float floor(float arg0) {
            return (float)(Math.Floor(arg0));
        }
        public static Vector2 floor(Vector2 arg0) {
            return new Vector2((float)(Math.Floor(arg0.x)), (float)(Math.Floor(arg0.y)));
        }
        public static Vector3 floor(Vector3 arg0) {
            return new Vector3((float)(Math.Floor(arg0.x)), (float)(Math.Floor(arg0.y)), (float)(Math.Floor(arg0.z)));
        }
        public static Vector4 floor(Vector4 arg0) {
            return new Vector4((float)(Math.Floor(arg0.x)), (float)(Math.Floor(arg0.y)), (float)(Math.Floor(arg0.z)), (float)(Math.Floor(arg0.w)));
        }
        public static float ceil(float arg0) {
            return (float)(Math.Ceiling(arg0));
        }
        public static Vector2 ceil(Vector2 arg0) {
            return new Vector2((float)(Math.Ceiling(arg0.x)), (float)(Math.Ceiling(arg0.y)));
        }
        public static Vector3 ceil(Vector3 arg0) {
            return new Vector3((float)(Math.Ceiling(arg0.x)), (float)(Math.Ceiling(arg0.y)), (float)(Math.Ceiling(arg0.z)));
        }
        public static Vector4 ceil(Vector4 arg0) {
            return new Vector4((float)(Math.Ceiling(arg0.x)), (float)(Math.Ceiling(arg0.y)), (float)(Math.Ceiling(arg0.z)), (float)(Math.Ceiling(arg0.w)));
        }
        public static float fract(float arg0) {
            return (arg0 - (float)(Math.Floor(arg0)));
        }
        public static Vector2 fract(Vector2 arg0) {
            return new Vector2((arg0.x - (float)(Math.Floor(arg0.x))), (arg0.y - (float)(Math.Floor(arg0.y))));
        }
        public static Vector3 fract(Vector3 arg0) {
            return new Vector3((arg0.x - (float)(Math.Floor(arg0.x))), (arg0.y - (float)(Math.Floor(arg0.y))), (arg0.z - (float)(Math.Floor(arg0.z))));
        }
        public static Vector4 fract(Vector4 arg0) {
            return new Vector4(
                (arg0.x - (float)(Math.Floor(arg0.x))), 
                (arg0.y - (float)(Math.Floor(arg0.y))), 
                (arg0.z - (float)(Math.Floor(arg0.z))), 
                (arg0.w - (float)(Math.Floor(arg0.w))));
        }
        public static float mod(float value, float by) {
            return (float)(((value - (by * Math.Floor(value / by)))));
        }
        public static Vector2 mod(Vector2 value, Vector2 by) {
            return new Vector2(
                (float)(((value.x - (by.x * Math.Floor(value.x / by.x))))), 
                (float)(((value.y - (by.y * Math.Floor(value.y / by.y))))));
        }
        public static Vector3 mod(Vector3 value, Vector3 by) {
            return new Vector3(
                (float)(((value.x - (by.x * Math.Floor(value.x / by.x))))), 
                (float)(((value.y - (by.y * Math.Floor(value.y / by.y))))), 
                (float)(((value.z - (by.z * Math.Floor(value.z / by.z))))));
        }
        public static Vector4 mod(Vector4 value, Vector4 by) {
            return new Vector4(
                (float)(((value.x - (by.x * Math.Floor(value.x / by.x))))), 
                (float)(((value.y - (by.y * Math.Floor(value.y / by.y))))), 
                (float)(((value.z - (by.z * Math.Floor(value.z / by.z))))), 
                (float)(((value.w - (by.w * Math.Floor(value.w / by.w))))));
        }
        public static float min(float arg0, float arg1) {
            return arg0 < arg1 ? arg0 : arg1;
        }
        public static Vector2 min(Vector2 arg0, Vector2 arg1) {
            return new Vector2(min(arg0.x, arg1.x), min(arg0.y, arg1.y));
        }
        public static Vector3 min(Vector3 arg0, Vector3 arg1) {
            return new Vector3(min(arg0.x, arg1.x), min(arg0.y, arg1.y), min(arg0.z, arg1.z));
        }
        public static Vector4 min(Vector4 arg0, Vector4 arg1) {
            return new Vector4(min(arg0.x, arg1.x), min(arg0.y, arg1.y), min(arg0.z, arg1.z), min(arg0.w, arg1.w));
        }
        public static Vector2 min(Vector2 arg0, float arg1) {
            return new Vector2(min(arg0.x, arg1), min(arg0.y, arg1));
        }
        public static Vector3 min(Vector3 arg0, float arg1) {
            return new Vector3(min(arg0.x, arg1), min(arg0.y, arg1), min(arg0.z, arg1));
        }
        public static Vector4 min(Vector4 arg0, float arg1) {
            return new Vector4(min(arg0.x, arg1), min(arg0.y, arg1), min(arg0.z, arg1), min(arg0.w, arg1));
        }
        public static float max(float arg0, float arg1) {
            return arg0 > arg1 ? arg0 : arg1;
        }
        public static float max(float arg0, float arg1, float arg2) {
            return max(arg2, max(arg0, arg1));
        }
        public static float max(float arg0, float arg1, float arg2, float arg3) {
            return max(max(arg2, arg3), max(arg0, arg1));
        }
        public static Vector2 max(Vector2 arg0, Vector2 arg1) {
            return new Vector2(max(arg0.x, arg1.x), max(arg0.y, arg1.y));
        }
        public static Vector3 max(Vector3 arg0, Vector3 arg1) {
            return new Vector3(max(arg0.x, arg1.x), max(arg0.y, arg1.y), max(arg0.z, arg1.z));
        }
        public static Vector4 max(Vector4 arg0, Vector4 arg1) {
            return new Vector4(max(arg0.x, arg1.x), max(arg0.y, arg1.y), max(arg0.z, arg1.z), max(arg0.w, arg1.w));
        }
        public static Vector2 max(Vector2 arg0, float arg1) {
            return new Vector2(max(arg0.x, arg1), max(arg0.y, arg1));
        }
        public static Vector3 max(Vector3 arg0, float arg1) {
            return new Vector3(max(arg0.x, arg1), max(arg0.y, arg1), max(arg0.z, arg1));
        }
        public static Vector4 max(Vector4 arg0, float arg1) {
            return new Vector4(max(arg0.x, arg1), max(arg0.y, arg1), max(arg0.z, arg1), max(arg0.w, arg1));
        }
        public static float clamp(float value, float minMax) {
            return max(-minMax, min(value, minMax));
        }
        public static float clamp(float value, float min, float max) {
            return MyMath.max(min, MyMath.min(value, max));
        }
        public static int clamp(int value, int minMax) {
            return max(-minMax, min(value, minMax));
        }
        public static int clamp(int value, int min, int max) {
            return MyMath.max(min, MyMath.min(value, max));
        }
        public static Vector2 clamp(Vector2 value, Vector2 min, Vector2 max) {
            return new Vector2(MyMath.max(min.x, MyMath.min(value.x, max.x)), MyMath.max(min.y, MyMath.min(value.y, max.y)));
        }
        public static Vector3 clamp(Vector3 value, Vector3 min, Vector3 max) {
            return new Vector3(
                MyMath.max(min.x, MyMath.min(value.x, max.x)), 
                MyMath.max(min.y, MyMath.min(value.y, max.y)), 
                MyMath.max(min.z, MyMath.min(value.z, max.z)));
        }
        public static Vector4 clamp(Vector4 value, Vector4 min, Vector4 max) {
            return new Vector4(
                MyMath.max(min.x, MyMath.min(value.x, max.x)), 
                MyMath.max(min.y, MyMath.min(value.y, max.y)), 
                MyMath.max(min.z, MyMath.min(value.z, max.z)), 
                MyMath.max(min.w, MyMath.min(value.w, max.w)));
        }
        public static Vector2 clamp(Vector2 value, float min, float max) {
            return new Vector2(MyMath.max(min, MyMath.min(value.x, max)), MyMath.max(min, MyMath.min(value.y, max)));
        }
        public static Vector3 clamp(Vector3 value, float min, float max) {
            return new Vector3(MyMath.max(min, MyMath.min(value.x, max)), MyMath.max(min, MyMath.min(value.y, max)), MyMath.max(min, MyMath.min(value.z, max)));
        }
        public static Vector4 clamp(Vector4 value, float min, float max) {
            return new Vector4(
                MyMath.max(min, MyMath.min(value.x, max)), 
                MyMath.max(min, MyMath.min(value.y, max)), 
                MyMath.max(min, MyMath.min(value.z, max)), 
                MyMath.max(min, MyMath.min(value.w, max)));
        }
        public static float mix(float from, float to, float progress) {
            return ((from * ((1 - progress))) + (to * progress));
        }
        public static Vector2 mix(Vector2 from, Vector2 to, Vector2 progress) {
            return new Vector2(((from.x * ((1 - progress.x))) + (to.x * progress.x)), ((from.y * ((1 - progress.y))) + (to.y * progress.y)));
        }
        public static Vector3 mix(Vector3 from, Vector3 to, Vector3 progress) {
            return new Vector3(
                ((from.x * ((1 - progress.x))) + (to.x * progress.x)), 
                ((from.y * ((1 - progress.y))) + (to.y * progress.y)), 
                ((from.z * ((1 - progress.z))) + (to.z * progress.z)));
        }
        public static Vector4 mix(Vector4 from, Vector4 to, Vector4 progress) {
            return new Vector4(
                ((from.x * ((1 - progress.x))) + (to.x * progress.x)), 
                ((from.y * ((1 - progress.y))) + (to.y * progress.y)), 
                ((from.z * ((1 - progress.z))) + (to.z * progress.z)), 
                ((from.w * ((1 - progress.w))) + (to.w * progress.w)));
        }
        public static Vector2 mix(Vector2 from, Vector2 to, float progress) {
            return new Vector2(((from.x * ((1 - progress))) + (to.x * progress)), ((from.y * ((1 - progress))) + (to.y * progress)));
        }
        public static Vector3 mix(Vector3 from, Vector3 to, float progress) {
            return new Vector3(
                ((from.x * ((1 - progress))) + (to.x * progress)), 
                ((from.y * ((1 - progress))) + (to.y * progress)), 
                ((from.z * ((1 - progress))) + (to.z * progress)));
        }
        public static Vector4 mix(Vector4 from, Vector4 to, float progress) {
            return new Vector4(
                ((from.x * ((1 - progress))) + (to.x * progress)), 
                ((from.y * ((1 - progress))) + (to.y * progress)), 
                ((from.z * ((1 - progress))) + (to.z * progress)), 
                ((from.w * ((1 - progress))) + (to.w * progress)));
        }
        public static float step(float edge, float value) {
            return (((value < edge)) ? (0) : (1));
        }
        public static Vector2 step(Vector2 edge, Vector2 value) {
            return new Vector2((((value.x < edge.x)) ? (0) : (1)), (((value.y < edge.y)) ? (0) : (1)));
        }
        public static Vector3 step(Vector3 edge, Vector3 value) {
            return new Vector3((((value.x < edge.x)) ? (0) : (1)), (((value.y < edge.y)) ? (0) : (1)), (((value.z < edge.z)) ? (0) : (1)));
        }
        public static Vector4 step(Vector4 edge, Vector4 value) {
            return new Vector4(
                (((value.x < edge.x)) ? (0) : (1)), 
                (((value.y < edge.y)) ? (0) : (1)), 
                (((value.z < edge.z)) ? (0) : (1)), 
                (((value.w < edge.w)) ? (0) : (1)));
        }
        public static Vector2 step(float edge, Vector2 value) {
            return new Vector2((((value.x < edge)) ? (0) : (1)), (((value.y < edge)) ? (0) : (1)));
        }
        public static Vector3 step(float edge, Vector3 value) {
            return new Vector3((((value.x < edge)) ? (0) : (1)), (((value.y < edge)) ? (0) : (1)), (((value.z < edge)) ? (0) : (1)));
        }
        public static Vector4 step(float edge, Vector4 value) {
            return new Vector4(
                (((value.x < edge)) ? (0) : (1)), 
                (((value.y < edge)) ? (0) : (1)), 
                (((value.z < edge)) ? (0) : (1)), 
                (((value.w < edge)) ? (0) : (1)));
        }
        public static float smoothstep(float from, float to, float progress) {
            return (((progress < from)) ? (0) : ((((progress > to)) ? (1) : (((progress * progress) * ((3 - (2 * progress))))))));
        }
        public static Vector2 smoothstep(Vector2 from, Vector2 to, Vector2 progress) {
            return new Vector2(
                (((progress.x < from.x)) ? (0) : ((((progress.x > to.x)) ? (1) : (((progress.x * progress.x) * ((3 - (2 * progress.x)))))))), 
                (((progress.y < from.y)) ? (0) : ((((progress.y > to.y)) ? (1) : (((progress.y * progress.y) * ((3 - (2 * progress.y)))))))));
        }
        public static Vector3 smoothstep(Vector3 from, Vector3 to, Vector3 progress) {
            return new Vector3(
                (((progress.x < from.x)) ? (0) : ((((progress.x > to.x)) ? (1) : (((progress.x * progress.x) * ((3 - (2 * progress.x)))))))), 
                (((progress.y < from.y)) ? (0) : ((((progress.y > to.y)) ? (1) : (((progress.y * progress.y) * ((3 - (2 * progress.y)))))))), 
                (((progress.z < from.z)) ? (0) : ((((progress.z > to.z)) ? (1) : (((progress.z * progress.z) * ((3 - (2 * progress.z)))))))));
        }
        public static Vector4 smoothstep(Vector4 from, Vector4 to, Vector4 progress) {
            return new Vector4(
                (((progress.x < from.x)) ? (0) : ((((progress.x > to.x)) ? (1) : (((progress.x * progress.x) * ((3 - (2 * progress.x)))))))), 
                (((progress.y < from.y)) ? (0) : ((((progress.y > to.y)) ? (1) : (((progress.y * progress.y) * ((3 - (2 * progress.y)))))))), 
                (((progress.z < from.z)) ? (0) : ((((progress.z > to.z)) ? (1) : (((progress.z * progress.z) * ((3 - (2 * progress.z)))))))), 
                (((progress.w < from.w)) ? (0) : ((((progress.w > to.w)) ? (1) : (((progress.w * progress.w) * ((3 - (2 * progress.w)))))))));
        }
        public static Vector2 smoothstep(float from, float to, Vector2 progress) {
            return new Vector2(
                (((progress.x < from)) ? (0) : ((((progress.x > to)) ? (1) : (((progress.x * progress.x) * ((3 - (2 * progress.x)))))))), 
                (((progress.y < from)) ? (0) : ((((progress.y > to)) ? (1) : (((progress.y * progress.y) * ((3 - (2 * progress.y)))))))));
        }
        public static Vector3 smoothstep(float from, float to, Vector3 progress) {
            return new Vector3(
                (((progress.x < from)) ? (0) : ((((progress.x > to)) ? (1) : (((progress.x * progress.x) * ((3 - (2 * progress.x)))))))), 
                (((progress.y < from)) ? (0) : ((((progress.y > to)) ? (1) : (((progress.y * progress.y) * ((3 - (2 * progress.y)))))))), 
                (((progress.z < from)) ? (0) : ((((progress.z > to)) ? (1) : (((progress.z * progress.z) * ((3 - (2 * progress.z)))))))));
        }
        public static Vector4 smoothstep(float from, float to, Vector4 progress) {
            return new Vector4(
                (((progress.x < from)) ? (0) : ((((progress.x > to)) ? (1) : (((progress.x * progress.x) * ((3 - (2 * progress.x)))))))), 
                (((progress.y < from)) ? (0) : ((((progress.y > to)) ? (1) : (((progress.y * progress.y) * ((3 - (2 * progress.y)))))))), 
                (((progress.z < from)) ? (0) : ((((progress.z > to)) ? (1) : (((progress.z * progress.z) * ((3 - (2 * progress.z)))))))), 
                (((progress.w < from)) ? (0) : ((((progress.w > to)) ? (1) : (((progress.w * progress.w) * ((3 - (2 * progress.w)))))))));
        }

        public static float sqr(float m) {
            return m * m;
        }
    }
}

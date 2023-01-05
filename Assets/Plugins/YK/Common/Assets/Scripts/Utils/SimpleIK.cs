using System;
using UnityEngine;

namespace moveen.utils {
    public class SimpleIK {

        public static bool intersection(float x0, float y0, float r0, float x1, float y1, float r1) {
            float a;
            float dx;
            float dy;
            float d;
            float h;
            float rx;
            float ry;
            float x2;
            float y2;
            dx = x1 - x0;
            dy = y1 - y0;
            d = (float)Math.Sqrt(dy * dy + dx * dx);
            if (d == 0) return false;
            if (d > r0 + r1)  {
                return false;
            }
            if (d < Math.Abs(r0 - r1))  {
                return false;
            }
            a = (r0 * r0 - r1 * r1 + d * d) / (2 * d);
            x2 = x0 + dx * a / d;
            y2 = y0 + dy * a / d;
            float hh = r0 * r0 - a * a;
            if (hh < 0) return false;
            h = (float)Math.Sqrt(hh);
            rx = -dy * (h / d);
            ry = dx * (h / d);
            xi = x2 + rx;
            xi_prime = x2 - rx;
            yi = y2 + ry;
            yi_prime = y2 - ry;

            if (float.IsNaN(xi_prime)) {
                Debug.Log(x1 + " " + d + " " + x2 + " " + rx + " " + dx + " " + h + " " + r0 + " " + a);
            }
            return true;
        }

        public static bool intersection(float r0, float x1, float r1) {
            if (x1 == 0) return false;
            if (x1 > r0 + r1) return false;
            if (x1 < Math.Abs(r0 - r1)) return false;
            
            float a = (r0 * r0 - r1 * r1 + x1 * x1) / (2 * x1);
            float hh = r0 * r0 - a * a;
            if (hh < 0) return false;
            float h = (float)Math.Sqrt(hh);
            xi = a;
            xi_prime = a;
            yi = h;
            yi_prime = -h;

            if (float.IsNaN(xi_prime)) {
                Debug.Log(x1 + " " + x1 + " " + a + " " + (float) 0 + " " + x1 + " " + h + " " + r0 + " " + a);
            }
            return true;
        }

        public static float xi;
        public static float xi_prime;
        public static float yi;
        public static float yi_prime;

    }
}

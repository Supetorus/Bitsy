using System.Collections.Generic;
using moveen.utils;
using UnityEngine;

namespace moveen.editor {
    public class HistoryGizmo1 {
        public static List<HistoryInfoBean> historyBeans = new List<HistoryInfoBean>() {
//            new HistoryInfoBean(HistoryInfoBean.heightRatio, Color.white),
//            new HistoryInfoBean(HistoryInfoBean.bipedEarlyStep, Color.magenta),
            new HistoryInfoBean(HistoryInfoBean.land, Color.green),
            new HistoryInfoBean(HistoryInfoBean.beginStep, Color.blue),
            new HistoryInfoBean(HistoryInfoBean.switchToBottom, Color.cyan),
//            new HistoryInfoBean(HistoryInfoBean.undockProgress, Color.magenta),
            new HistoryInfoBean(HistoryInfoBean.wasTooLong, Color.yellow),
            new HistoryInfoBean(HistoryInfoBean.legAlt, Color.white),

            new HistoryInfoBean(HistoryInfoBean.deviation, Color.red),
            new HistoryInfoBean(HistoryInfoBean.fromAbove, Color.white),
            new HistoryInfoBean(HistoryInfoBean.lt, Color.magenta),
            new HistoryInfoBean(HistoryInfoBean.lt2, Color.magenta),
            new HistoryInfoBean(HistoryInfoBean.progressPare, Color.blue),
            new HistoryInfoBean(HistoryInfoBean.devFrom0, Color.cyan),
            new HistoryInfoBean(HistoryInfoBean.devFrom1, Color.cyan),
            new HistoryInfoBean(HistoryInfoBean.devFrom2, Color.cyan),
            new HistoryInfoBean(HistoryInfoBean.devFrom3, Color.cyan),
            new HistoryInfoBean(HistoryInfoBean.progressFrom0, Color.yellow),
            new HistoryInfoBean(HistoryInfoBean.progressFrom1, Color.yellow),
            new HistoryInfoBean(HistoryInfoBean.progressFrom2, Color.yellow),
            new HistoryInfoBean(HistoryInfoBean.progressFrom3, Color.yellow),
            new HistoryInfoBean(HistoryInfoBean.progress, Color.black),
        };

        public static void drawNeedle(float a2, Vector2 dialCenter, float r1, float r2) {
            Vector2 aa = new Vector2(MyMath.cos(a2), MyMath.sin(a2));
            UnityEditorUtils.line2D(dialCenter + aa * (r1 + 2), dialCenter + aa * (r2 - 2), 3);
        }

        public static void drawHistory(CounterStacksCollection history, List<HistoryInfoBean> historyInfoBeans, int i) {
            float nextTimeAt = -1;
            for (int j = 0; j < history.stacks.Count; j++) {
                Dictionary<string, float> currentMoment = history.getRelative(history.stacks.Count - j);

                float h1 = 60f;


                float w = 2;
                int count = historyInfoBeans.Count;
                float x = j * (count * (w + 1) + 3) + 30;
                float y = Camera.current.pixelHeight - (i + 1) * h1;
                int cur = 0;

                if (x > nextTimeAt) {
                    if (currentMoment.ContainsKey("time")) {
                        //Vector3 p = Camera.current.ScreenToWorldPoint(new Vector3(x, y - 20, 10));
                        //Handles.Label(p, currentMoment["time"].ToString("F3"), new GUIStyle {richText = true});
                    }
                    nextTimeAt = x + 100;
                }

                Gizmos.color = Color.white;
                for (int hbi = 0; hbi < historyInfoBeans.Count; hbi++) {
                    HistoryInfoBean hb = historyInfoBeans[hbi];
                    Gizmos.color = hb.color;
                    drawValue(x, cur++, y, w, 20, hb.name, currentMoment);
                }
            }
        }

        private static void drawValue(float x, int n, float y, float w, float mul, string name, Dictionary<string, float> d) {
            if (!d.ContainsKey(name)) return;
            float value = d[name];
            value = MyMath.clamp(value, -1, 1) * mul;


            drawValue(x, n, y, w, value);
        }

        private static void drawValue(float x, int n, float y, float w, float value) {
            x += (w + 1) * n;
            for (int xx = (int) x; xx <= x + w; xx++) {
                Vector3 p = Camera.current.ScreenToWorldPoint(new Vector3(xx, y, 10));
                Vector3 p2 = Camera.current.ScreenToWorldPoint(new Vector3(xx, y + value, 10));
                Gizmos.DrawLine(p, p2);
            }
        }
    }
}
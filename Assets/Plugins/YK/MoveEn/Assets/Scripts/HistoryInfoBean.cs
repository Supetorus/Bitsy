using UnityEngine;

namespace moveen.utils {
    public class HistoryInfoBean {
        public static string heightRatio = "heightRatio";
        public static string bipedEarlyStep = "bipedEarlyStep";
        public static string land = "land";
        public static string timedProgress = "timedProgress";
        public static string deviation = "deviation";
        public static string fromAbove = "fromAbove";
        public static string lt = "lt";
        public static string lt2 = "lt2";
        public static string progressPare = "progressPare";
        public static string devFrom0 = "devFrom0";
        public static string devFrom1 = "devFrom1";
        public static string devFrom2 = "devFrom2";
        public static string devFrom3 = "devFrom3";
        public static string progressFrom0 = "progressFrom0";
        public static string progressFrom1 = "progressFrom1";
        public static string progressFrom2 = "progressFrom2";
        public static string progressFrom3 = "progressFrom3";
        public static string progress = "progress";
        public static string wasTooLong = "wasTooLong";
        public static string beginStep = "beginStep";
        public static string switchToBottom = "switchToBottom";
        public static string legAlt = "legAlt";
        public static string undockProgress = "undockProgress";
        public static string groundProgress = "groundProgress";
        
        public bool show = true;
        public Color color;
        public string name;

        public HistoryInfoBean(string name, Color color) {
            this.color = color;
            this.name = name;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using moveen.core;

namespace moveen.utils {
    public static class MUtil2 {
        public static List<Step2> tempSort(List<Step2> from) {
            return from.OrderBy(x => -x.fromAbove).ToList();
        }

        
        //TODO remove, with al<Transform>(null), values == null
//        public static List<T> al<T>(params T[] values) {
//            List<T> result = new List<T>();
//            for (int i = 0; i < values.Length; i++) {
//                T t = values[i];
//                result.Add(t);
//            }
//            return result;
//        }

        public static List<T> al<T>(params T[] values) {//WARNING, don't use with (null)
            List<T> result = new List<T>();
            for (int i = 0; i < values.Length; i++) {
                T t = values[i];
                result.Add(t);
            }
            return result;
        }
//        public static List<T> al<T>(T value) {  //can't use this as it is ombiguous when value is null
//            List<T> result = new List<T>();
//            result.Add(value);
//            return result;
//        }
        public static List<T> al<T>() {
            return new List<T>();
        }
    }
}
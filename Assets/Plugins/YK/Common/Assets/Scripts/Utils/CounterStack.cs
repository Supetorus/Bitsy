using System.Collections.Generic;
using moveen.utils;
using UnityEngine;

namespace moveen.utils {
    public class CounterStack {
        private List<int> values = new List<int>();
        private int currentCount;
        private int nextIndex;
        public int sum;

        public CounterStack() {
        }

        public CounterStack(int count) {
            setCount(count);
        }

        public void setCount(int count){
            MUtil.madeCount(values, count, 0);
            nextIndex = 0;
            values[0] = 0;
            sum = 0;
            currentCount = 0;
        }
        
        public void setNextValue(int v) {
            sum = sum - values[nextIndex] + v;
            values[nextIndex] = v;
            nextIndex = (nextIndex + 1) % values.Count;
            currentCount++;
            if (currentCount > values.Count) currentCount = values.Count;
        }

        public float getMid() {
//            Debug.Log(sum + " / " + currentCount);
            if (currentCount == 0) return 0;
            return (float)sum / currentCount;
        }
    }
    
}
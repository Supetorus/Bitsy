using System.Collections.Generic;
using moveen.utils;
using UnityEngine;

namespace moveen.utils {
    public class CounterStacksCollection {
        public List<Dictionary<string, float>> stacks = new List<Dictionary<string, float>>();

        public int current;

        public CounterStacksCollection(int count) {
            MUtil.madeCount(stacks, count);
        }

        public Dictionary<string, float> getCurrent() {
            return stacks[current];
        }

        public void next() {
            current = (current + 1) % stacks.Count;
            getCurrent().Clear();
            setValue("time", Time.time);
        }

        public Dictionary<string, float> getRelative(int i) {
            return stacks[(current + i) % stacks.Count];
        }

        public void setValue(string s, float value) {
            Dictionary<string, float> dict = getCurrent();
            dict[s] = value;
        }
    }
}
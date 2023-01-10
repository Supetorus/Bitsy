using System.Collections.Generic;
using moveen.utils;
using UnityEngine;

namespace moveen.example {
    public delegate bool predicate(CharInfo ci);

    public class CharInfo : MonoBehaviour {
        public string team;
        public static List<CharInfo> allChars = new List<CharInfo>();

        public int command;

        private void OnDisable() {
            allChars.Remove(this);
        }

        private void OnEnable() {
            allChars.Add(this);
        }

        public CharInfo getClosestToMe(predicate p) {
            CharInfo closest = null;
            Vector3 myPos = transform.position;
            float closestDist = 0;
            foreach (var v in allChars) {
                if (v == this) continue;
                float dist = myPos.dist(v.transform.position);
                if (closest == null || dist < closestDist) {
                    if (p(v)) {
                        closest = v;
                        closestDist = dist;
                    }
                }
            }
            return closest;
        }
    }
}
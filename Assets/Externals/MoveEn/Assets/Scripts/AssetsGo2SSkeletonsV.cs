using System.Collections.Generic;
using System.Linq;
using moveen.utils;
using UnityEngine;

namespace moveen.core {
    public static class AssetsGo2SSkeletonsV {
        private static void cleanupNeuro(List<Step2> steps) {
            for (int i = 0; i < steps.Count; i++) {
                Step2 who = steps[i];
                who.affectedByDeviation.Clear();
                who.affectedByDir.Clear();
                who.affectedByProgress.Clear();
            }
        }
                 
        public static void fillNeuro2(List<Step2> steps) {
            cleanupNeuro(steps);
            Step2 s0 = steps[0];
            Step2 s1 = steps[1];
            s0.affectedByProgress = MUtil2.al(new StepNeuro<Step2>(s1, -100, 0.1f));
            s1.affectedByProgress = MUtil2.al(new StepNeuro<Step2>(s0, -100, 0.1f));

            //affect self
            s0.affectedByDeviation = MUtil2.al(new StepNeuro<Step2>(s0, 0, 2f), new StepNeuro<Step2>(s1, 0, 0f));
            s1.affectedByDeviation = MUtil2.al(new StepNeuro<Step2>(s1, 0, 2f), new StepNeuro<Step2>(s0, 0, 0f));

        }

        public static void fillNeuroSimple(List<Step2> steps, float affectSelf, float rewardOthers) {
            cleanupNeuro(steps);
            foreach (Step2 who in steps) {
                who.affectedByDeviation = new List<StepNeuro<Step2>>();
                who.affectedByProgress = new List<StepNeuro<Step2>>();
                foreach (Step2 whom in steps) {
                    if (who == whom) {
                        who.affectedByDeviation.Add(new StepNeuro<Step2>(whom, 0, affectSelf));
                    } else {
                        who.affectedByDeviation.Add(new StepNeuro<Step2>(whom, 0f, rewardOthers));
                        who.affectedByProgress.Add(new StepNeuro<Step2>(whom, -1, 0.1f));//was -2 before 01.02.18
                    }
                }
            }
        }

        public static void fillNeuroNPares(List<Step2> steps) {
            cleanupNeuro(steps);

            for (int i1 = 0; i1 < steps.Count; i1++) {
                Step2 who = steps[i1];
                who.affectedByDeviation = new List<StepNeuro<Step2>>();
                who.affectedByProgress = new List<StepNeuro<Step2>>();

                Step2 pare;
                if (i1 % 2 == 1) {
                    pare = steps[i1 - 1];
                } else {
//                    pare = steps[i1 + 1];
                    pare = i1 + 1 < steps.Count ? steps[i1 + 1] : null;
                }

                for (int i2 = 0; i2 < steps.Count; i2++) {
                    Step2 whom = steps[i2];

                    if (whom == pare) {
//                        who.affectedByDeviation.add(new StepNeuro<StepV>(whom, 0, 2f)); //affect pare
                        who.affectedByProgress.Add(new StepNeuro<Step2>(whom, 1, -1));
                    } else if (who == whom) {
                        who.affectedByDeviation.Add(new StepNeuro<Step2>(whom, 0, 2f));
                    } else {
//                        who.affectedByProgress.add(new StepNeuro<StepV>(whom, -1, 0.2f)); //affect others
                        who.affectedByDeviation.Add(new StepNeuro<Step2>(whom, 0f, 0.5f));
                    }
                }
                if (steps.Count > 3) {//ad-hock fix for some stepping issues 
                    steps[1].affectedByProgress.Add(new StepNeuro<Step2>(steps[3], -2, 0.2f));
                    steps[3].affectedByProgress.Add(new StepNeuro<Step2>(steps[1], -2, 0.2f));
                }
            }
        }
        public static void fillNeuroNPares2(List<Step2> steps, float rewardSelf, float affectPare, float affectCounter, float rewardOthers, float affectOthers) {
            cleanupNeuro(steps);
            float affectPareC = affectPare / steps.Count;
            float affectCounterC = affectCounter / steps.Count;
            float rewardOthersC = rewardOthers / steps.Count;
            float affectOthersC = affectOthers / steps.Count;
            
            foreach (Step2 who in steps) {
                who.affectedByDeviation.Add(new StepNeuro<Step2>(who, 0, rewardSelf, "self"));
            }
            for (int i = 0; i < steps.Count; i++) {
                bool whoFirstBunch = i < steps.Count / 2;
                Step2 who = steps[i];
                for (int j = 0; j < steps.Count; j++) {
                    bool whomFirstBunch = j < steps.Count / 2;
                    Step2 whom = steps[j];
                    if (who == whom) continue;

                    who.affectedByDeviation.Add(new StepNeuro<Step2>(whom, 0f, rewardOthersC, "devFrom" + i));

                    who.affectedByProgress.Add(new StepNeuro<Step2>(whom, -10f/steps.Count, 10f/steps.Count, "progress all" + i));

                    if (whoFirstBunch == whomFirstBunch) {
                        who.affectedByProgress.Add(new StepNeuro<Step2>(whom, affectPareC, -affectPareC * 1, "progressPare"));
                    } else {
                        who.affectedByProgress.Add(new StepNeuro<Step2>(whom, -affectCounterC, 0, "progressFrom" + i));
                        
//                        who.affectedByDeviation.add(new StepNeuro<Step2>(whom, 0f, rewardCounterC, "dev counter")); //reward others (for not to wait for sudden step with both legs)
                    }
                }
            }

        }
    }
}
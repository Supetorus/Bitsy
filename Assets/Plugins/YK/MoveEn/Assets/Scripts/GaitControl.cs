using moveen.descs;
using UnityEngine;
using UnityEngine.UI;

namespace moveen.example {
    public class GaitControl : MonoBehaviour {
        public Transform panelWithButtons;
        public MoveenStepper5 bti;
        public MoveControl1 mc;

        public void Start() {
        }

        public void Update() {
            if (bti == null) {
                mc = GetComponent<MoveControl1>();
                if (mc == null) return;
                bti = mc.moveen;
                if (bti == null) return;
            }
            if (Input.GetKey(KeyCode.Alpha1)) selectButton(0);
            if (Input.GetKey(KeyCode.Alpha2)) selectButton(1);
            if (Input.GetKey(KeyCode.Alpha3)) selectButton(2);
            if (Input.GetKey(KeyCode.Alpha4)) selectButton(3);
            if (Input.GetKey(KeyCode.Alpha5)) selectButton(4);
        }

        public void selectButton(int i) {
            for (int ii = 0; ii < panelWithButtons.childCount; ii++) {
                Button b = panelWithButtons.GetChild(ii).GetComponent<Button>();
                var colors = b.colors;
                if (ii != i) {
                    colors.normalColor = Color.grey;
                } else if (ii == i) {
                    colors.normalColor = Color.green;

                    resetGait(mc);

                    if (i == 0) initBoxer(mc);
                    if (i == 1) initAgile(mc);
                    if (i == 2) initConfident(mc);
                    if (i == 3) initHeavy(mc);
                    if (i == 4) initWounded(mc);
//jumper
//like a boxer
//                    jumpy = 40
//                    maxForce = 40
//                    bti.engine.steps[0].undockPauseMax = 0.5f;//для прыжков
//                    bti.engine.steps[1].undockPauseMax = 0.5f;

                }
                b.colors = colors;
            }
        }

        public static void initWounded(MoveControl1 mc) {
            MoveenStepper5 bti = mc.moveen;
            //TODO edit one-leg-g
            //TODO comfortRadius * 2
//                    bti.engine.stepUndockPauseMax = 0.5f;
            bti.engine.steps[0].undockPause = 0.5f;
            bti.engine.steps[1].undockPause = 0;

            bti.engine.steps[0].stepSpeedMin = 3;
            bti.engine.steps[1].stepSpeedMin = 4;
            mc.speed = 1;
            bti.engine.cogUpDown = -1;
            bti.engine.cogAngle = 0.3f;
            bti.engine.cogAccel = 3;
            bti.engine.hipFlexibility = 0;
            bti.engine.bodyLenHelp = -1;
//            bti.engine.jumpy = 5;
        }

        public static void initHeavy(MoveControl1 mc) {
            MoveenStepper5 bti = mc.moveen;
            //TODO edit one-leg-g
            //TODO comfortRadius * 2
//                    bti.engine.stepUndockPauseMax = 0.5f;
            bti.engine.steps[0].undockPause = 0.5f;
            bti.engine.steps[1].undockPause = 0.5f;
            bti.engine.downOnStep = 0.8f;

            mc.speed = 1;
//                    bti.engine.useDistraction = false;
            bti.engine.cogUpDown = -1;
            bti.engine.cogAngle = 0.15f;
            bti.engine.cogAccel = 2;
            bti.engine.hipFlexibility = 0;
//                    bti.bodyLenHelp = -0.5f; // мы хотим чтобы стрелял прямо... мб во время ходьбы он д помогать, но не во время стрельбы?
            
//            bti.engine.jumpy = 5;
        }

        public static void initHeavyRun(MoveControl1 mc) {
            initHeavy(mc);
            MoveenStepper5 bti = mc.moveen;
            bti.engine.bodyLenHelp = -0.5f;
            mc.speed = 2;
        }

        public static void initConfident(MoveControl1 mc) {
            MoveenStepper5 bti = mc.moveen;
            bti.engine.cogUpDown = -1;
            bti.engine.cogAngle = -0.1f;
            bti.engine.hipFlexibility = 0.4f;
            bti.engine.bodyLenHelp = 0.6f;
//            bti.engine.jumpy = 1;
        }

        public static void initAgile(MoveControl1 mc) {
            MoveenStepper5 bti = mc.moveen;
            bti.engine.cogUpDown = 1;
            bti.engine.cogAngle = 0.3f;
            bti.engine.hipFlexibility = 1;
            bti.engine.bodyLenHelp = 0.6f;
//            bti.engine.jumpy = 1;
        }

        public static void initBoxer(MoveControl1 mc) {
//                    bti.engine.steps[0].undockPauseMax = 0.5f;//для прыжков
//                    bti.engine.steps[1].undockPauseMax = 0.5f;
            MoveenStepper5 bti = mc.moveen;
//            bti.engine.useDistraction = true;
            bti.engine.cogUpDown = 1;
            bti.engine.cogAngle = 0.15f;
            bti.engine.hipFlexibility = 0;
            bti.engine.bodyLenHelp = -0.6f;
//            bti.engine.jumpy = 10;
        }

        public static void resetGait(MoveControl1 mc) {
            mc.speed = 4;
            MoveenStepper5 bti = mc.moveen;

//        bti.engine.steps[0].stepSpeedMin = 4;
//        bti.engine.steps[1].stepSpeedMin = 4;
//        bti.engine.steps[0].stepSpeedBodyMul = 1.2f;
//        bti.engine.steps[1].stepSpeedBodyMul = 1.2f;

            bti.engine.steps[0].undockPause = 0;
            bti.engine.steps[1].undockPause = 0;

            bti.engine.downOnStep = 0.7f;
//            bti.engine.virtualPosMaxDif = 1;
            bti.engine.cogAccel = 10;
            bti.engine.bodyLenHelp = 0;

//            bti.engine.useDistraction = false;
        }
    }
}
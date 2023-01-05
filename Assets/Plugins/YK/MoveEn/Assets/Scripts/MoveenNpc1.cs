using System;
using moveen.example;
using moveen.utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace moveen.descs {
    public class MoveenNpc1 : MonoBehaviour {
        
        private MoveControl3 mc3;
        [NonSerialized]private CharInfo ci;

        private void OnEnable() {
            if (mc3 == null) mc3 = GetComponent<MoveControl3>();
            ci = GetComponent<CharInfo>();
        }

        [ReadOnly]public float thinkPause;
        [ReadOnly]public float idlePause;
        [ReadOnly]public float lookPause;
        public P<Vector3> placeToGo;

        [NonSerialized]private Vector3 lookPos;
        
        private void Update() {
            mc3.aimTarget.position = lookPos;

            if (mc3 == null) return;
            if (mc3.moveen == null) return;
            thinkPause -= Time.deltaTime;
            idlePause -= Time.deltaTime;
            lookPause -= Time.deltaTime;
            if (thinkPause > 0) return;
            thinkPause += 0.1f;

            mc3.moveDir = Vector3.zero;
            //mc3.aimPos = Vector3.zero;

            if (lookPause < 0) {
                lookPause += Random.value * 2 + 1;
                if (mc3.aimTarget != null) {
                    lookPos = mc3.moveen.transform.position + 
                        mc3.moveen.transform.rotation.rotate(Vector3.right) * 10
                        + (Random.insideUnitCircle * 2).withY(Random.value * 2 - 1);
                }

            }


            if (ci != null) {
                CharInfo closestToMe = ci.getClosestToMe(c => true);
//                CharInfo closestToMe = ci.getClosestToMe(c => !c.team.Equals(ci.team));
                if (closestToMe != null) {
                    Vector3 hisPos = closestToMe.transform.position;
                    Vector3 myPos = transform.position;
                    if (hisPos.dist(myPos) < 5) {
                        placeToGo = new P<Vector3>(myPos + myPos.sub(hisPos) * 10 + Random.insideUnitCircle.withY(0) * 5);
                    }
                }
            }

            if (placeToGo != null) {
                Vector3 dir = placeToGo.v.sub(mc3.moveen.engine.imCenter);
                mc3.chassisRot = MUtil.qToAxesYX(Vector3.up, dir);
                Vector3 dir0 = dir.withSetY(0);
                mc3.moveDir = dir0;
                if (dir0.length() < 1) {
                    placeToGo = null;
                    mc3.moveDir = Vector3.zero;
                }
                return;
            }
            
            if (idlePause > 0) return;

            chooseAction();
        }

        private void chooseAction() {
            float value = Random.value;
            if (value < 0.5f) {
                idlePause = 1 + Random.value * 3;
            } else {
                placeToGo = new P<Vector3>(transform.position + Random.insideUnitCircle.withY(0) * 10);
            }

        }

        private void OnDrawGizmos() {
            if (placeToGo != null) {
                Gizmos.DrawLine(transform.position, placeToGo.v.withSetY(transform.position.y));
            }
        }
    }
}
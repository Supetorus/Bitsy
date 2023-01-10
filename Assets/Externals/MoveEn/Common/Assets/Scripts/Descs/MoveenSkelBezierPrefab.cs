using moveen.utils;
using UnityEngine;

namespace moveen.descs {
    public class MoveenSkelBezierPrefab : MoveenSkelBezier {
        //TODO consider composition instead of inheritance
        public GameObject prefab;

        public Vector3 posDisp;
        public Vector3 rotDisp;
        public Vector3 scale = Vector3.one;

        private Quaternion rotDispQ = Quaternion.identity;

        public override void updateData() {
            base.updateData();

            rotDispQ = Quaternion.Euler(rotDisp);
            gameObject.removeAllChildrenImmediate(false);
            for (int i = 0; i < poss.Count; i++) {
                GameObject res = Instantiate(prefab, transform);
                res.transform.parent = transform;
            }
            
        }

        public override void tick(float dt) {
            base.tick(dt);
            
            for (int i = 0; i < poss.Count; i++) {
                Vector3 basisPos = poss[i];
                Quaternion basisRot = rotDispQ * rots[i];

                Transform child = transform.GetChild(i);
                child.SetPositionAndRotation(posChange(basisPos, basisRot, i), rotChange(basisRot, i));
                child.localScale = scaleChange(i);

            }
            
        }

        //override this method for radius modification or smthng
        public virtual Vector3 posChange(Vector3 basisPos, Quaternion basisRot, int iU) {
            return basisPos + basisRot.rotate(posDisp);
        }

        //override this method for radius modification or smthng
        public virtual Quaternion rotChange(Quaternion rot, int iU) {
            return rot;
        }

        //override this method for radius modification or smthng
        public virtual Vector3 scaleChange(int iU) {
            return scale;
        }
    }
}
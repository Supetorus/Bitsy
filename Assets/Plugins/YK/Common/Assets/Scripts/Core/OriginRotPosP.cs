using UnityEngine;
using moveen.utils;

namespace moveen.core {
    public class OriginRotPosP : OriginBase {

        public P<Quaternion> rot = new P<Quaternion>(Quaternion.identity);
        public P<Vector3> pos = new P<Vector3>(new Vector3());
        bool isDirty;
        Matrix4x4 matrix = new Matrix4x4();

        public OriginRotPosP() {
        }

        public OriginRotPosP(P<Quaternion> rot, P<Vector3> pos) {
            this.rot = rot;
            this.pos = pos;
        }
        public override void tick() {
            this.isDirty = true;
        }
        public override Matrix4x4 getMatrix() {
            if (this.isDirty)  {
                this.isDirty = false;
                this.rot.v.fillMatrix4(this.matrix);
                this.matrix.setTranslation(this.pos.v);
            }
            return this.matrix;
        }
        public override Vector3 getPos() {
            return this.pos.v;
        }
        public override Quaternion getRot() {
            return this.rot.v;
        }

    }
}

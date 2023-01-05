using moveen.utils;
using UnityEngine;

namespace moveen.core {
    public class JunctionElbow2 : JunctionBase {

        public float radius1;
        public float radius2;

        public override void tick(float dt) {
            if (SimpleIK.intersection(0, 0, this.radius1, (this.targetAbs.v - this.basisAbs.v).length(), 0, this.radius2))  {
                if ((SimpleIK.yi > 0))  {
                    this.resultAbs.v = this.calc(new Vector2(SimpleIK.xi, SimpleIK.yi));
                } else {
                    this.resultAbs.v = this.calc(new Vector2(SimpleIK.xi_prime, SimpleIK.yi_prime));
                }
            }
        }
        public virtual Vector3 calc(Vector2 res1) {
            Vector3 x = (this.targetAbs.v - this.basisAbs.v).normalized().mul(res1.x);
            Vector3 y = (this.normalAbs.v - this.basisAbs.v).crossProduct((this.targetAbs.v - this.basisAbs.v)).normalized().mul(res1.y);
            return ((x + y) + this.basisAbs.v);
        }

    }
}

using moveen.utils;
using UnityEngine;

namespace moveen.core {
    public abstract class JunctionBase : Tickable {

        public P<Vector3> basisAbs;
        public P<Vector3> resultAbs = new P<Vector3>(new Vector3());
        public P<Vector3> targetAbs;
        
        //normal, not displaced, currently for elbows only
        //  ? use result instead?
        public P<Vector3> normalAbs;

        public abstract void tick(float dt);

        public virtual void calcAbs(float dt, Vector3 pos, Quaternion rot) {
        }

        public virtual void reset() {
            tick(1);
        }

    }
}

using UnityEngine;

namespace moveen.utils {
    public class BiConsumerImplV3V3 : BiConsumer<P<Vector3>, P<Vector3>> {
        public Vector3 m;

        public BiConsumerImplV3V3(Vector3 mul) {
            m = mul;
        }

        public void accept(P<Vector3> a, P<Vector3> b) {
            b.v = a.v.mul(m);
        }
    }
}
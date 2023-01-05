using UnityEngine;

namespace moveen.utils {
    public class BiConsumerImplV3V3Const : BiConsumer<P<Vector3>, P<Vector3>> {
        public Vector3 c;

        public BiConsumerImplV3V3Const(Vector3 c) {
            this.c = c;
        }

        public void accept(P<Vector3> a, P<Vector3> b) {
            b.v = c;
        }
    }
}
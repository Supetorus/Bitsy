using UnityEngine;
using System.Collections.Generic;

namespace moveen.utils {
    public class ManyAxes : BiConsumer<Vector3, Vector3> {

        public List<Tuple<Vector3, Vector3>> axes = MUtil2.al<Tuple<Vector3, Vector3>>();

        public virtual void accept(Vector3 input, Vector3 output) {
            input = input.normalized();
            int axesSize = this.axes.Count;
            for (int i = 0; (i < axesSize); i = (i + 1))  {
                Tuple<Vector3, Vector3> ax = this.axes[i];
                Vector3 a = ax.a;
                Vector3 b = ax.b;
                output = (output + b.mul(((1 - (a.dist(input) / 2)))));
            }
        }
        public ManyAxes(List<Tuple<Vector3, Vector3>> axes) {
            this.axes = axes;
        }

    }
}

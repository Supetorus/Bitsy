namespace moveen.core {
    public class StepNeuro<T> {

        public T leg;
        public float add;
        public float mul;
        public string desc;

        public StepNeuro(T leg, float add, float mul, string desc = "") {
            this.leg = leg;
            this.add = add;
            this.mul = mul;
            this.desc = desc;
        }

    }
}

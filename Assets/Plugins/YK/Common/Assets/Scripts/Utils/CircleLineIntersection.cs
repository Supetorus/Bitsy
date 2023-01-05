namespace moveen.utils {
    public class CircleLineIntersection {

        public static CircleLineIntersection inst = new CircleLineIntersection();
        public float resDisc;
        public float resX1;
        public float resY1;
        public float resX2;
        public float resY2;

        public virtual void calc(float r, float x1, float y1, float x2, float y2) {
            float dx = (x2 - x1);
            float dy = (y2 - y1);
            float dr = MyMath.sqrt((MyMath.sqr(dx) + MyMath.sqr(dy)));
            float D = ((x1 * y2) - (x2 * y1));
            this.resDisc = ((MyMath.sqr(r) * MyMath.sqr(dr)) - MyMath.sqr(D));
            if ((this.resDisc < 0))  {
                return ;
            }
            float sgnDy = (((dy < 0)) ? (-1) : (1));
            float f1 = MyMath.sqrt(((MyMath.sqr(r) * MyMath.sqr(dr)) - MyMath.sqr(D)));
            this.resX1 = (((D * dy) + ((sgnDy * dx) * f1))) / MyMath.sqr(dr);
            this.resY1 = (((-D * dx) + (MyMath.abs(dy) * f1))) / MyMath.sqr(dr);
            this.resX2 = (((D * dy) - ((sgnDy * dx) * f1))) / MyMath.sqr(dr);
            this.resY2 = (((-D * dx) - (MyMath.abs(dy) * f1))) / MyMath.sqr(dr);
        }

    }
}

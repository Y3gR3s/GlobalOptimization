namespace GlobalOptimization
{
    public class Interval
    {
        public double A { get; set; }
        public double B { get; set; }
        public double R { get; set; }

        public Interval(double a, double b)
        {
            A = a;
            B = b;
        }

        public Interval(double a, double b, double r) : this(a, b)
        {
            R = r;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Interval interval = obj as Interval;
            if (obj == null)
            {
                return false;
            }

            return A == interval.A
                && B == interval.B
                && R == interval.R;
        }

        public override int GetHashCode()
        {
            return A.GetHashCode() ^ B.GetHashCode() ^ R.GetHashCode();
        }
    }
}

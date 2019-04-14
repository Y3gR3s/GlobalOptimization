using System.Linq;
using System.Collections.Generic;

namespace GlobalOptimization
{
    public class Interval
    {
        /// <summary>
        /// Начальная точка интервала
        /// </summary>
        public double A { get; set; }

        /// <summary>
        /// Конечная точка интервала
        /// </summary>
        public double B { get; set; }

        /// <summary>
        /// R характеристика интервала
        /// </summary>
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

        /// <summary>
        /// Создает набор интервалов, исходя из заданного массива точек
        /// Пример: points = [1, 2, 3] -> intervals = [1; 2], [2; 3]
        /// </summary>
        /// <param name="points">Массив точек</param>
        /// <returns>Список интервалов</returns>
        public static List<Interval> GetIntervals(double[] points)
        {
            var intervals = new List<Interval>();

            for (int i = 0; i < points.Length - 1; i++)
            {
                intervals.Add(new Interval(points[i], points[i + 1]));
            }

            return intervals;
        }

        /// <summary>
        /// Делит отрезок на равные интервалы заданной длины
        /// </summary>
        /// <param name="a">Левая граница отрезка</param>
        /// <param name="b">Правая граница отрезка</param>
        /// <param name="length">Длина интервала</param>
        /// <returns>Список интервалов</returns>
        public static List<Interval> DevideSegmentByIntervals(double a, double b, double length)
        {
            var xPoints = new List<double>();

            for (double i = a; i < b; i += length)
            {
                xPoints.Add(i);
            }

            xPoints.Add(b);
            return Interval.GetIntervals(xPoints.ToArray());
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

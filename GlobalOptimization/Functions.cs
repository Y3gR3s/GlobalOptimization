using System.Collections.Generic;

namespace GlobalOptimization
{
    public delegate double Function(double x);

    public static class Functions
    {
        public static double Shekel(double x)
        {           
            var coefficients = new List<double[]>()
            {
                new double[] { 0.394344, 1.393876, 0.126179 },
                new double[] { 0.295838, 0.655881, 0.087775 },
                new double[] { 0.635375, 1.040491, 0.075923 },
                new double[] { 0.225777, 1.296672, 0.086019 },
                new double[] { 0.57074,  0.247111, 0.034143 },
                new double[] { 0.448298, 0.509802, 0.156708 },
                new double[] { 0.944544, 0,396264, 0.072744 },
                new double[] { 0.577814, 1.345561, 0.065791 },
                new double[] { 0.927328, 0.385115, 0.176967 },
                new double[] { 0.500884, 2.850398, 0.030718 },
            };

            double sum = 0;
            foreach (var coefficient in coefficients)
            {
                sum += 1 / (coefficient[0] * (x - coefficient[1]) * (x - coefficient[1]) + coefficient[2]);
            }

            return -sum;
        }
    }
}

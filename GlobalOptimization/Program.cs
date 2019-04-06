using System;

namespace GlobalOptimization
{
    class Program
    {
        static void Main(string[] args)
        {
            // Начальные условия
            double a = 0;
            double b = 4;
            double accuarcy = 0.01;
            double r = 1.5;

            Console.WriteLine("Абсолютный минимум: {0}", Solver.GetAbsoluteMinimum(Functions.Shekel, a, b, accuarcy, r));
            Console.ReadLine();
        }
    }
}

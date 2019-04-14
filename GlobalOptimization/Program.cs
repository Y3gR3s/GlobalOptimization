using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GlobalOptimization
{
    class Program
    {
        static void Main(string[] args)
        {
            // Начальные условия (см. метод Solver.GetAbsoluteMinimum)
            double a   = 0,
                   b   = 4,
                   eps = 0.00001,
                   r   = 1.5;

            // Координаты точки глобального минимума
            double x,
                   y;

            using (var timer = new Timer((time) => Console.WriteLine("Продолжительность работы последовательного алгоритма: {0} мс", time)))
            {
                Solver.GetAbsoluteMinimum(Functions.Shekel, a, b, eps, r, out x, out y);
            }
            Console.WriteLine("Точка глобального минимума при последовательном алгоритме: [{0:0.000}, {1:0.000}]\n", x, y);

            #region Parallel Realization

            // Количество логических процессоров
            int processorsCount = 50;

            // Массивы, хранящие локальные минимумы на каждом из интервалов
            var localMinimumsX = new double[processorsCount];
            var localMinimumsY = new double[processorsCount];

            // Делим исходный отрезок на равные интервалы исходя из количества процессоров
            double intervalLength = (b - a) / processorsCount;
            var intervals = Interval.DevideSegmentByIntervals(a, b, intervalLength);

            //Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)0x0055;

            using (var timer = new Timer((time) => Console.WriteLine("Продолжительность работы параллельного алгоритма: {0} мс", time)))
            {
                Parallel.For(0, processorsCount, (i) =>
                {
                    Solver.GetAbsoluteMinimum(Functions.Shekel, intervals[i].A, intervals[i].B, eps, r, out localMinimumsX[i], out localMinimumsY[i]);
                });

                y = localMinimumsY.Min();
                x = localMinimumsX[localMinimumsY.ToList().IndexOf(y)];
            }

            Console.WriteLine("Точка глобального минимума при параллельном алгоритме: [{0:0.000}, {1:0.000}]", x, y);

            #endregion

            Console.ReadLine();
        }

    }
}

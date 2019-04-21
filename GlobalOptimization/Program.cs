using System;
using System.Diagnostics;

namespace GlobalOptimization
{
    class Program
    {
        static void Main(string[] args)
        {
            // Начальные условия (см. метод Solver.GetAbsoluteMinimum)
            double a = 0,
                   b = 4,
                   eps = 0.0005,
                   r = 1.5;

            // Координаты точки глобального минимума
            double x, y;

            // Время работы последовательного и параллельного алгоритма
            double seqTime = 0,
                   parallelTime = 0,
                   quaziParallelTime = 0;

            // Количество процессоров
            int processorsCount = Environment.ProcessorCount;

            using (Timer timer = new Timer((time) => seqTime = time))
            {
                Solver.GetAbsoluteMinimum(Functions.Shekel, a, b, eps, r, out x, out y);
            }
            Console.WriteLine("Точка глобального минимума: [{0:0.00000}, {1:0.00000}]\n", x, y);

            Console.WriteLine("{0, -20} {1, -15} {2, -15} {3, -15} {4, -15} {5, -15}", "Количество потоков", "T пос. (мс)", "T пар. (мс)", "T кв-пар. (мс)", "S", "E (%)");
            for (int i = 1; i <= Environment.ProcessorCount; i++)
            {
                // Параллельное выполнение программы
                Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)0x0055;
                using (Timer timer = new Timer((time) => parallelTime = time))
                {
                    Solver.GetAbsoluteMinimumParallel(Functions.Shekel, a, b, eps, r, i, out x, out y);
                }

                // Квазипаралельное выполнение программы
                Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)0x0001;
                using (Timer timer = new Timer((time) => quaziParallelTime = time))
                {
                    Solver.GetAbsoluteMinimumParallel(Functions.Shekel, a, b, eps, r, 1, out x, out y);
                }

                Console.WriteLine("{0, -20} {1, -15:0.000} {2, -15:0.000} {3, -15:0.000} {4, -15:0.000} {5, -15:0.000}", i, seqTime, parallelTime, quaziParallelTime,
                    seqTime / parallelTime, (seqTime / parallelTime) / (processorsCount / 2) * 100);
            }

            Console.ReadLine();
        }

    }
}

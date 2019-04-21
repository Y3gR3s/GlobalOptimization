using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlobalOptimization
{
    public static class Solver
    {
        /// <summary>
        /// Метод одномерной глобальной оптимизации Р.Стронгина
        /// Находит абсолютный минимум функции на отрезке с заданной точностью
        /// https://pribor.ifmo.ru/file/article/4922.pdf
        /// </summary>
        /// <param name="function">Функция</param>
        /// <param name="a">Левая граница отрезка</param>
        /// <param name="b">Правая граница отрезка</param>
        /// <param name="eps">Точность</param>
        /// <param name="r">Заданный коэффициент, параметр алгоритма, который определяется из предположений о коэффициенте K в условии Липшица (r > 1)</param>
        /// <param name="x">Точка глобального минимума</param>
        /// <param name="y">Глобальный минимум</param>
        public static void GetAbsoluteMinimum(Function function, double a, double b, double eps, double r, out double x, out double y)
        {
            // Список Х координат функции
            List<double> xPoints = new List<double>() { a, b };

            // Список интервалов для данных координат Х 
            List<Interval> intervals;

            do
            {
                xPoints.Sort();

                // Находим интервалы для координат Х
                intervals = Interval.GetIntervals(xPoints.ToArray());

                // Находим максимальное абсолютное значение относительной первой разности
                for (int i = 0; i < intervals.Count; i++)
                {
                    intervals[i].M = Math.Abs(function(intervals[i].B) - function(intervals[i].A)) / (intervals[i].B - intervals[i].A);
                }

                double M = intervals.Max(i => i.M);

                if (M < 0)
                {
                    throw new ArgumentException("M не может быть меньше нуля");
                }

                double m = (M == 0) ? 1 : (r * M);

                // Для каждого интервала вычисляем R характеристику
                // Она определяет вероятность нахождения глобального минимума на этом интервале. Чем она больше, тем больше вероятность
                for (int i = 0; i < intervals.Count; i++)
                {
                    if (i == 0)
                    {
                        intervals[i].R = 2 * (intervals[i].B - intervals[i].A) - 4 * function(intervals[i].B) / m;
                    }
                    else if (i == intervals.Count - 1)
                    {
                        intervals[i].R = 2 * (intervals[i].B - intervals[i].A) - 4 * function(intervals[i].A) / m;
                    }
                    else
                    {
                        double ya = function(intervals[i].A),
                               yb = function(intervals[i].B);

                        intervals[i].R = (intervals[i].B - intervals[i].A) + ((yb - ya) * (yb - ya)) /
                            (m * m * (intervals[i].B - intervals[i].A)) - 2 * (yb + ya) / m;
                    }
                }

                // Сортируем интервалы по убыванию R, для того чтобы найти интервал, которому соответствует максимальная R характеристика
                // Реализовано таким образом для соответствия параллельному алгоритму
                intervals = intervals.OrderByDescending(i => i.R).ToList();

                // Вычисляем новую точку разбиения интервала
                double xk = (intervals[0].A == a ^ intervals[0].B == b) ?
                        0.5 * (intervals[0].A + intervals[0].B) :
                        0.5 * (intervals[0].A + intervals[0].B) - Math.Sign(function(intervals[0].B) - function(intervals[0].A)) *
                            (function(intervals[0].B) - function(intervals[0].A)) / (2 * r * m);

                // Если достигли заданной погрешности, останавливаемся
                if (intervals[0].B - intervals[0].A < eps)
                {
                    x = xk;
                    y = function(x);
                    return;
                }

                xPoints.Add(xk);
            }
            while (true);
        }

        /// <summary>
        /// Метод одномерной глобальной оптимизации Р.Стронгина
        /// Находит абсолютный минимум функции на отрезке с заданной точностью используя вычислительные мощности нескольких процессоров
        /// http://skif.pereslavl.ru/psi-info/rcms-open.ts/open.ts-publications/2005-rus/grishagine.pdf
        /// </summary>
        /// <param name="function">Функция</param>
        /// <param name="a">Левая граница отрезка</param>
        /// <param name="b">Правая граница отрезка</param>
        /// <param name="eps">Точность</param>
        /// <param name="r">Заданный коэффициент, параметр алгоритма, который определяется из предположений о коэффициенте K в условии Липшица (r > 1)</param>
        /// <param name="processorsCount">Количество используемых процессоров</param>
        /// <param name="x">Точка глобального минимума</param>
        /// <param name="y">Глобальный минимум</param>
        public static void GetAbsoluteMinimumParallel(Function function, double a, double b, double eps, double r, int processorsCount, out double x, out double y)
        {
            var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = processorsCount };

            // Список Х координат функции
            List<double> xPoints = new List<double>() { a, b };

            // Список интервалов для данных координат Х 
            List<Interval> intervals;

            do
            {
                xPoints.Sort();

                // Находим интервалы для координат Х
                intervals = Interval.GetIntervals(xPoints.ToArray());

                // Находим максимальное абсолютное значение относительной первой разности
                Parallel.For(0, intervals.Count, parallelOptions, (i) =>
                {
                    intervals[i].M = Math.Abs(function(intervals[i].B) - function(intervals[i].A)) / (intervals[i].B - intervals[i].A);
                });

                double M = intervals.Max(i => i.M);

                if (M < 0)
                {
                    throw new ArgumentException("M не может быть меньше нуля");
                }

                double m = (M == 0) ? 1 : (r * M);

                // Для каждого интервала вычисляем R характеристику
                // Она определяет вероятность нахождения глобального минимума на этом интервале. Чем она больше, тем больше вероятность
                Parallel.For(0, intervals.Count, parallelOptions, (i) =>
                {
                    if (i == 0)
                    {
                        intervals[i].R = 2 * (intervals[i].B - intervals[i].A) - 4 * function(intervals[i].B) / m;
                    }
                    else if (i == intervals.Count - 1)
                    {
                        intervals[i].R = 2 * (intervals[i].B - intervals[i].A) - 4 * function(intervals[i].A) / m;
                    }
                    else
                    {
                        double ya = function(intervals[i].A),
                               yb = function(intervals[i].B);

                        intervals[i].R = (intervals[i].B - intervals[i].A) + ((yb - ya) * (yb - ya)) /
                            (m * m * (intervals[i].B - intervals[i].A)) - 2 * (yb + ya) / m;
                    }
                });

                // Сортируем интервалы по убыванию R, для того чтобы найти интервал, которому соответствует максимальная R характеристика
                intervals = intervals.OrderByDescending(i => i.R).ToList();

                // Вычисляем новую точку разбиения интервала
                double xk = (intervals[0].A == a ^ intervals[0].B == b) ?
                        0.5 * (intervals[0].A + intervals[0].B) :
                        0.5 * (intervals[0].A + intervals[0].B) - Math.Sign(function(intervals[0].B) - function(intervals[0].A)) *
                            (function(intervals[0].B) - function(intervals[0].A)) / (2 * r * m);

                // Если достигли заданной погрешности, останавливаемся
                if (intervals[0].B - intervals[0].A < eps)
                {
                    x = xk;
                    y = function(x);
                    return;
                }

                xPoints.Add(xk);
            }
            while (true);
        }
    }
}

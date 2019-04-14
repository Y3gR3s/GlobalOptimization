using System;
using System.Collections.Generic;
using System.Linq;

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
            var xPoints = new List<double>() { a, b };

            // Интервал, которому соответствует максимальная R характеристика
            Interval intervalMaxR; 

            do
            {
                xPoints.Sort();

                // Список интервалов для данных координат Х 
                var intervals = Interval.GetIntervals(xPoints.ToArray());

                // Находим максимальное абсолютное значение относительной первой разности
                var M = intervals.Max(interval => Math.Abs(function(interval.B) - function(interval.A)) / (interval.B - interval.A));

                if (M < 0)
                {
                    throw new ArgumentException("M не может быть меньше нуля");
                }
                
                double m = (M == 0) ? 1 : (r * M);

                // Для каждого интервала вычисляем R характеристику
                // Она определяет вероятность нахождения глобального минимума на этом интервале. Чем она больше, тем больше вероятность
                foreach (var interval in intervals)
                {
                    interval.R = 2 * (interval.B - interval.A) - 4 * function(interval.B) / (r * m);
                }

                // Находим интервал, которому соответствует максимальная R характеристика
                intervalMaxR = intervals.First(interval => interval.R == intervals.Max(i => i.R));

                // Вычисляем новую точку разбиения интервала                
                double xk = intervalMaxR.A != intervalMaxR.B ?
                    0.5 * (intervalMaxR.A + intervalMaxR.B) :
                    0.5 * (intervalMaxR.A + intervalMaxR.B) - Math.Sign(function(intervalMaxR.B) - 
                    function(intervalMaxR.A)) / (2 * r) * function(intervalMaxR.B) - function(intervalMaxR.A) / m;

                // Если достигли заданной погрешности, останавливаемся
                if (intervalMaxR.B - intervalMaxR.A < eps)
                {
                    x = xk;
                    y = function(xk);
                    return;
                }

                xPoints.Add(xk);
            }
            while (true);
        }
    }
}

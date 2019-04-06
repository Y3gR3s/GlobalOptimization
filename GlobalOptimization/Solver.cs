using System;
using System.Collections.Generic;
using System.Linq;

namespace GlobalOptimization
{
    public static class Solver
    {
        /// <summary>
        /// Алгоритм глобального поиска Стронгина
        /// Находит абсолютный минимум функции на отрезке с заданной точностью
        /// http://crm-en.ics.org.ru/uploads/crmissues/crm_2015_3/15750.pdf
        /// </summary>
        /// <param name="function">Функция</param>
        /// <param name="a">Левая граница отрезка</param>
        /// <param name="b">Правая граница отрезка</param>
        /// <param name="accuracy">Точность</param>
        /// <param name="r">Заданный коэффициент, параметр алгоритма, который определяется из предположений о коэффициенте K в условии Липшица (r > 1)</param>
        /// <returns>Абсолютный минимум функции</returns>
        public static double GetAbsoluteMinimum(Function function, double a, double b, double accuracy, double r)
        {
            // Список Х координат функции
            var xPoints = new List<double>() { a, b };

            // Список интервалов для данных координат Х 
            var intervals = GetIntervals(xPoints);

            // Точка разбиения интервала
            double x;

            do
            {
                // TODO: Уточнить знаменатель
                // Находим максимальное абсолютное значение относительной первой разности
                double M = intervals.Max(interval => Math.Abs(function(interval.B) - function(interval.A)) / (interval.B - interval.A));

                // Если верно, то ошибка в вычислении M
                if (M < 0)
                {
                    throw new ArgumentException("M не может быть меньше нуля");
                }

                double m = (M == 0) ? 1 : (r * M);

                // Для каждого интервала вычисляем R характеристику
                // Она определяет вероятность нахождения глобального минимума на этом интервале. Чем она больше, тем больше вероятность
                foreach (var interval in intervals)
                {
                    interval.R = m * (interval.B - interval.A) + Math.Pow(function(interval.B) - function(interval.A), 2) / (m * (interval.B - interval.A))
                        - 2 * (function(interval.B) + function(interval.A));
                }

                // Находим интервал, которому соответствует максимальная R характеристика
                Interval intervalMaxR = intervals.First(interval => interval.R == intervals.Max(i => i.R));

                // Вычсляем новую точку разбиения интервала 
                x = 0.5 * (intervalMaxR.A + intervalMaxR.B) - 0.5 * (function(intervalMaxR.B) - function(intervalMaxR.A));

                xPoints.Add(x);
                xPoints.Sort();

                intervals = GetIntervals(xPoints);
            }
            // TODO: уточнить условие остановки
            while (intervals.Min(interval => interval.B - x) < accuracy);

            return function(x);
        }

        // Возвращает список интервалов, исходя из заданного списка точек
        // Пример: points = [1, 2, 3] -> inervals = [1; 2], [2; 3] 
        private static List<Interval> GetIntervals(List<double> points)
        {
            var intervals = new List<Interval>();

            for (int i = 0; i < points.Count - 1; i++)
            {
                intervals.Add(new Interval(points[i], points[i + 1]));
            }

            return intervals;
        }
    }
}

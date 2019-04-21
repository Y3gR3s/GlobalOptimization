using System;
using System.Diagnostics;

namespace GlobalOptimization
{
    /// <summary>
    /// Вычисляет продолжительность выполнения кода, заключенного в using и выполняет заданное действие
    /// </summary>
    public class Timer : IDisposable
    {
        readonly Stopwatch watch;
        readonly Action<double> output;

        public Timer(Action<double> Output)
        {
            output = Output;

            watch = new Stopwatch();
            watch.Start();
        }

        public void Dispose()
        {
            watch.Stop();
            output(watch.Elapsed.TotalMilliseconds);
        }
    }
}

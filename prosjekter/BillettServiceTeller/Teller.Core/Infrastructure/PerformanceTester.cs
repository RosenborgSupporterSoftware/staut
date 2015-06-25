using System;
using System.Diagnostics;

namespace Teller.Core.Infrastructure
{
    public class PerformanceTester : IDisposable
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly Action<TimeSpan> _callback;

        public PerformanceTester()
        {
            _stopwatch.Start();
        }

        public PerformanceTester(Action<TimeSpan> callback): this()
        {
            _callback = callback;
        }

        public static PerformanceTester Start(Action<TimeSpan> callback)
        {
            return new PerformanceTester(callback);
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            if (_callback != null)
                _callback(Elapsed);
        }

        public TimeSpan Elapsed
        {
            get { return _stopwatch.Elapsed; }
        }

        public long ElapsedMilliseconds
        {
            get { return _stopwatch.ElapsedMilliseconds; }
        }
    }
}
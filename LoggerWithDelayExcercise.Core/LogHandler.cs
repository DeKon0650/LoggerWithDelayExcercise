using System;
using System.Diagnostics;

namespace LoggerWithDelayExcercise.Core
{
    public class LogHandler
    {
        public string Message { get; }
        public bool WasCanceled { get; private set; }
        public TimeSpan ElapsedTime => _stopwatch.Elapsed;

        private readonly Stopwatch _stopwatch = new Stopwatch();

        public LogHandler(string message)
        {
            Message = message;
        }

        public void StartLogging()
        {
            if (!_stopwatch.IsRunning)
            {
                _stopwatch.Restart();
                WasCanceled = false;
            }
        }

        public void Cancel()
        {
            _stopwatch.Stop();
            WasCanceled = true;
        }
    }
}

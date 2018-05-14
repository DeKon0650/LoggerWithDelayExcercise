using System;
using System.Diagnostics;
using System.Threading;
using LoggerWithDelayExcercise.Core.Implementations.MessageToFile;

namespace LoggerWithDelayExcercise.Core
{
    public class LogHandler
    {
        private readonly object _locker = new object();
        private readonly ILogWriter _logWriter;
        private readonly TimeSpan _delay;

        private Thread _delayedLogTask;
        private CancellationTokenSource _cancellationTokenSource;

        public LogHandler(TimeSpan delay, ILogWriter logWriter)
        {
            _delay = delay;
            _logWriter = logWriter ?? throw new ArgumentNullException(nameof(logWriter));
        }

        public void StartLogCountdown()
        {
            if (_delayedLogTask != null) return;
            _cancellationTokenSource = new CancellationTokenSource();
            _delayedLogTask = new Thread(() =>
            {
                var testMessage = (MessageToFileLogWriter) _logWriter;
                Debug.WriteLine($"(1b) {testMessage._message} (Logger) [{DT(DateTime.Now)}] gone sleep for {_delay}");
                Thread.Sleep(_delay);
                Debug.WriteLine($"(2b) {testMessage._message} (Logger) [{DT(DateTime.Now)}] woke up and started checking cancellation");
                lock (_locker)
                {
                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        Debug.WriteLine($"(3b1) {testMessage._message} (Logger) [{DT(DateTime.Now)}] found cancellation");
                        return;
                    }
                }
                //TODO Exception handling
                Debug.WriteLine($"(3b2) {testMessage._message} (Logger) [{DT(DateTime.Now)}] did not find cancellation and started writing a log");
                _logWriter.WriteLog();
            });
            _delayedLogTask.Start();
        }

        public void Cancel()
        {
            lock (_locker)
            {
                if (_delayedLogTask == null || _cancellationTokenSource == null || _cancellationTokenSource.IsCancellationRequested)
                    return;
                _cancellationTokenSource.Cancel();
            }
        }

        public static string DT(DateTime dateTime)
        {
            return dateTime.ToString("HH:mm:ss FFFFFFF");
        }
    }
}

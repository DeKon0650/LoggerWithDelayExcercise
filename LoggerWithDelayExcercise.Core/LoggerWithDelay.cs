using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerWithDelayExcercise.Core
{
    public class LoggerWithDelay
    {
        public static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(1);

        private readonly object _locker = new object();
        private readonly List<LogHandler> _logs = new List<LogHandler>();
        private readonly ILogWriter _logWriter;
        private readonly TimeSpan _delay;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private Task _checkTask;

        public LoggerWithDelay(TimeSpan delay, ILogWriter logWriter)
        {
            _logWriter = logWriter;
            _delay = delay;
            _cancellationTokenSource = new CancellationTokenSource();
            _checkTask = Task.Factory.StartNew(CheckLogs, TaskCreationOptions.LongRunning);
        }

        public LogHandler LogWithDelay(string message)
        {
            var log = new LogHandler(message);
            lock (_locker)
                _logs.Add(log);
            log.StartLogging();
            return log;
        }

        public void StopLogger()
        {
            _cancellationTokenSource.Cancel();
        }

        private void CheckLogs()
        {
            do
            {
                Thread.Sleep(CheckInterval);
                LogHandler[] logs;
                lock (_locker)
                    logs = _logs.ToArray();
                foreach (var log in logs)
                {
                    if (log.ElapsedTime >= _delay)
                    {
                        DoLog(log);
                        RemoveFromLogList(log);
                    }
                    else if (log.WasCanceled)
                    {
                        RemoveFromLogList(log);
                    }
                }
            } while (!_cancellationTokenSource.IsCancellationRequested);
        }

        public void RemoveFromLogList(LogHandler log)
        {
            lock (_locker)
                _logs.Remove(log);
        }

        private void DoLog(LogHandler log)
        {
            _logWriter.WriteLog(log.Message);
        }
    }
}

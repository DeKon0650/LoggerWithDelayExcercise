using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LoggerWithDelayExcercise.Core.Implementations.MessageToFile;

namespace LoggerWithDelayExcercise.Core.Implementations.ListLogger
{
    public class LoggerWithDelay
    {
        public static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(1);

        private readonly object _locker = new object();
        private readonly List<LogHandler> _logs = new List<LogHandler>();
        private readonly TimeSpan _delay;
        private Task _checkTask;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public LoggerWithDelay(TimeSpan delay)
        {
            _delay = delay;
            _cancellationTokenSource = new CancellationTokenSource();
            _checkTask = Task.Factory.StartNew(CheckLogs, TaskCreationOptions.LongRunning);
        }

        public LogHandler LogWithDelay(string message)
        {
            var log = new LogHandler(message);
            lock (_locker)
                _logs.Add(log);
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
                var checkDate = DateTime.Now;
                foreach (var log in logs)
                {
                    bool needLog = false;
                    bool needRemoveFromLogList = false;
                    lock (log.Locker)
                    {
                        if (log.CancelDate.HasValue)
                        {
                            if (log.CancelDate - log.StartDate > _delay) needLog = true;
                            needRemoveFromLogList = true;
                        }
                        else
                        {
                            if (log.StartDate + _delay < checkDate)
                            {
                                needLog = true;
                                needRemoveFromLogList = true;
                            }
                        }
                    }
                    if (needLog) DoLog(log);
                    if (needRemoveFromLogList) RemoveFromLogList(log);
                }
            } while (!_cancellationTokenSource.Token.IsCancellationRequested);
        }

        public void RemoveFromLogList(LogHandler log)
        {
            lock (_locker)
                _logs.Remove(log);
        }

        private void DoLog(LogHandler log)
        {
            new MessageToFileLogWriterFactory(log.Message).CreateLogWriter().WriteLog();
        }
    }
}

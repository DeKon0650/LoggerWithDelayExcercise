using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerWithDelayExcercise.Core
{
    public class LoggerWithDelay : IDisposable
    {
        public static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(1);

        private readonly ConcurrentDictionary<Guid, LogHandler> _logs = new ConcurrentDictionary<Guid, LogHandler>();
        private readonly ILogWriter _logWriter;
        private readonly TimeSpan _delay;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private bool _disposed = false;

        public LoggerWithDelay(TimeSpan delay, ILogWriter logWriter)
        {
            _logWriter = logWriter;
            _delay = delay;
            _cancellationTokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(CheckLogs, TaskCreationOptions.LongRunning);
        }

        public LogHandler LogWithDelay(string message)
        {
            var log = new LogHandler(message);
            _logs.TryAdd(log.UniqueId, log);
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
                foreach (var logFromDic in _logs)
                {
                    var log = logFromDic.Value;
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
            } while (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested);
        }

        public void RemoveFromLogList(LogHandler log)
        {
            _logs.TryRemove(log.UniqueId, out log);
        }

        private void DoLog(LogHandler log)
        {
            _logWriter.WriteLog(log.Message);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _logs.Clear();
                _cancellationTokenSource?.Dispose();
            }
            _disposed = true;
        }
    }
}

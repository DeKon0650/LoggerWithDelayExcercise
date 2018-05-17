using System;
using System.Collections.Concurrent;
using System.Threading;

namespace LoggerWithDelayExcercise.Core
{
    public class LoggerWithDelay : IDisposable
    {
        public static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(1);

        private readonly ConcurrentDictionary<Guid, LogHandler> _logs = new ConcurrentDictionary<Guid, LogHandler>();
        private readonly ILogWriter _logWriter;
        private readonly TimeSpan _delay;
        private Timer _timer;
        private bool _disposed = false;

        public LoggerWithDelay(TimeSpan delay, ILogWriter logWriter)
        {
            _logWriter = logWriter;
            _delay = delay;
            StartLogger();
        }

        public LogHandler LogWithDelay(string message)
        {
            var log = new LogHandler(message);
            _logs.TryAdd(log.UniqueId, log);
            log.StartLogging();
            return log;
        }

        public void StartLogger()
        {
            if (_timer == null)
                _timer = new Timer(state => CheckLogs(), null, TimeSpan.Zero, CheckInterval);
        }

        public void StopLogger()
        {
            _timer?.Dispose();
        }

        private void CheckLogs()
        {
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
                _timer?.Dispose();
            }
            _disposed = true;
        }
    }
}

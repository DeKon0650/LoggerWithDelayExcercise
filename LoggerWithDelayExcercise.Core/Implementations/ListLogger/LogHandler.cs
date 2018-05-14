using System;

namespace LoggerWithDelayExcercise.Core.Implementations.ListLogger
{
    public class LogHandler
    {
        public readonly object Locker = new object();

        public DateTime StartDate { get; }
        public string Message { get; }
        public DateTime? CancelDate { get; private set; }

        public LogHandler(string message)
        {
            Message = message;
            StartDate = DateTime.Now;
        }

        public void Cancel()
        {
            lock (Locker)
            {
                if (CancelDate == null) CancelDate = DateTime.Now;
            }
        }
    }
}

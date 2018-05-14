using System;
using LoggerWithDelayExcercise.Core.Implementations.MessageToFile;

namespace LoggerWithDelayExcercise.Core
{
    public class LoggerWithDelay
    {
        private readonly TimeSpan _delay;

        public LoggerWithDelay(TimeSpan delay)
        {
            _delay = delay;
        }

        public LogHandler LogWithDelay(string message)
        {
            ILogWriterFactory factory = new MessageToFileLogWriterFactory(message);
            var logHandler = new LogHandler(_delay, factory.CreateLogWriter());
            logHandler.StartLogCountdown();
            return logHandler;
        }
    }
}

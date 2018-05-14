using System;

namespace LoggerWithDelayExcercise.Tests
{
    public class TestLoggingMessage
    {
        public string Message { get; }
        public TimeSpan Delay { get; }

        public TestLoggingMessage(string message, TimeSpan delay)
        {
            Message = message;
            Delay = delay;
        }
    }
}

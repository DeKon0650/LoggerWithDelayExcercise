using System;

namespace LoggerWithDelayExcercise.Core
{
    public interface ILogWriter : IDisposable
    {
        void WriteLog(string message);
    }
}

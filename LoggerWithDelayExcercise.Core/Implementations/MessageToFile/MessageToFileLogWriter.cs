using System;
using System.IO;

namespace LoggerWithDelayExcercise.Core.Implementations.MessageToFile
{
    public class MessageToFileLogWriter : ILogWriter
    {
        private readonly object _fileLocker = new object();
        private readonly string _fileName;

        public MessageToFileLogWriter(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("FileName is empty", nameof(fileName));
            _fileName = fileName;
        }

        public void WriteLog(string message)
        {
            lock (_fileLocker)
            {
                File.AppendAllLines(_fileName, new[] {message});
            }
        }
    }
}

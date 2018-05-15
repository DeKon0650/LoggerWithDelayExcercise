using System;
using System.Diagnostics;
using System.IO;

namespace LoggerWithDelayExcercise.Core.Implementations.MessageToFile
{
    public class MessageToFileLogWriter : ILogWriter
    {
        private static readonly object FileLocker = new object();
        private readonly string _fileName;
        private readonly string _message;

        public MessageToFileLogWriter(string message, string fileName)
        {
            if (string.IsNullOrWhiteSpace(message)) throw new ArgumentException("Message is empty", nameof(message));
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("FileName is empty", nameof(fileName));
            _message = message;
            _fileName = fileName;
        }

        public void WriteLog()
        {
            lock (FileLocker)
            {
                Debug.WriteLine(_message);
                File.AppendAllLines(_fileName, new[] {_message});
            }
        }
    }
}

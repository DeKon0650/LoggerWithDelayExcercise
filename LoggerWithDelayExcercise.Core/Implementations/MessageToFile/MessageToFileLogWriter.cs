using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace LoggerWithDelayExcercise.Core.Implementations.MessageToFile
{
    public class MessageToFileLogWriter : ILogWriter
    {
        private readonly BlockingCollection<string> _messagesCollection = new BlockingCollection<string>(new ConcurrentQueue<string>());
        private bool _disposed = false;

        public MessageToFileLogWriter(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("FileName is empty", nameof(fileName));
            Task.Factory.StartNew(() =>
            {
                try
                {
                    foreach (var message in _messagesCollection.GetConsumingEnumerable())
                    {
                        File.AppendAllLines(fileName, new[] { message });
                    }
                }
                finally 
                {
                    _messagesCollection?.Dispose();
                }
            }, TaskCreationOptions.LongRunning);
        }

        public void WriteLog(string message)
        {
            _messagesCollection.Add(message);
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
                _messagesCollection?.CompleteAdding();
            }
            _disposed = true;
        }
    }
}

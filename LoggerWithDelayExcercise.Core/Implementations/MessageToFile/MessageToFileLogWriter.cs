using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace LoggerWithDelayExcercise.Core.Implementations.MessageToFile
{
    public class MessageToFileLogWriter : ILogWriter
    {
        private readonly BlockingCollection<string> _messagesCollection = new BlockingCollection<string>(new ConcurrentQueue<string>());

        public MessageToFileLogWriter(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("FileName is empty", nameof(fileName));
            Task.Factory.StartNew(() =>
            {
                //TODO Stop condition
                while (true)
                {
                    if (_messagesCollection.TryTake(out var message))
                        File.AppendAllLines(fileName, new[] {message});
                }
            });
        }

        public void WriteLog(string message)
        {
            _messagesCollection.Add(message);
        }
    }
}

using System;
using System.IO;
using System.Threading;
using LoggerWithDelayExcercise.Core;
using LoggerWithDelayExcercise.Core.Implementations.MessageToFile;

namespace LoggerWithDelayExcercise.ConsoleTest
{
    public class Program
    {
        private static LoggerWithDelay _logger = new LoggerWithDelay(TimeSpan.FromSeconds(5));

        public static void Main()
        {
            if (File.Exists(MessageToFileLogWriterFactory.FileName)) File.Delete(MessageToFileLogWriterFactory.FileName);
            ThreadPool.QueueUserWorkItem(_ =>
            {
                var t = _logger.LogWithDelay("message 1"); // this message should not be in the log output
                Thread.Sleep(TimeSpan.FromSeconds(1));
                t.Cancel();
                Console.WriteLine("First have finished");
            });

            ThreadPool.QueueUserWorkItem(_ =>
            {
                var t = _logger.LogWithDelay("message 2"); // this message should be in the log output
                Thread.Sleep(TimeSpan.FromSeconds(6));
                t.Cancel();
                Console.WriteLine("Second have finished");
            });
            Console.ReadLine();
        }
    }
}

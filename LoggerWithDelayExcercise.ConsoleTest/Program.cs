using System;
using System.IO;
using System.Threading;
using LoggerWithDelayExcercise.Core;
using LoggerWithDelayExcercise.Core.Implementations.MessageToFile;

namespace LoggerWithDelayExcercise.ConsoleTest
{
    public class Program
    {
        public static void Main()
        {
            using (var logWriter = new MessageToFileLogWriterFactory().CreateLogWriter())
            using (var logger = new LoggerWithDelay(TimeSpan.FromSeconds(5), logWriter))
            {
                if (File.Exists(MessageToFileLogWriterFactory.FileName)) File.Delete(MessageToFileLogWriterFactory.FileName);
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    var t = logger.LogWithDelay("message 1"); // this message should not be in the log output
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    t.Cancel();
                    Console.WriteLine("First have finished");
                });

                ThreadPool.QueueUserWorkItem(_ =>
                {
                    var t = logger.LogWithDelay("message 2"); // this message should be in the log output
                    Thread.Sleep(TimeSpan.FromSeconds(6));
                    t.Cancel();
                    Console.WriteLine("Second have finished");
                });
                Console.ReadLine();
            }
        }
    }
}

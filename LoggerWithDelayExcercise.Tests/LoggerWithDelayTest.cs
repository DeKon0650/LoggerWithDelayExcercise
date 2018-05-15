using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LoggerWithDelayExcercise.Core;
using LoggerWithDelayExcercise.Core.Implementations.MessageToFile;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LoggerWithDelayExcercise.Tests
{
    [TestClass]
    public class LoggerWithDelayTest
    {
        [TestMethod]
        public void TestMultithreadLog()
        {
            const int threadCount = 100;
            const int maxSecondsDelay = 60;
            var logDelay = TimeSpan.FromSeconds(maxSecondsDelay / 2);
            var logger = new LoggerWithDelay(logDelay, new MessageToFileLogWriterFactory().CreateLogWriter());
            var random = new Random();
            var loggingMessages = new TestLoggingMessage[threadCount];
            var testTasks = new Task[loggingMessages.Length];
            for (int i = 0; i < loggingMessages.Length; i++)
            {
                var testLogginMessage = new TestLoggingMessage((i + 1).ToString(), TimeSpan.FromSeconds(random.Next(1, maxSecondsDelay + 1)));
                loggingMessages[i] = testLogginMessage;
                testTasks[i] = new Task(t =>
                {
                    var testMessage = (TestLoggingMessage) t;
                    var handler = logger.LogWithDelay(testMessage.Message);
                    Thread.Sleep(testMessage.Delay);
                    handler.Cancel();
                }, testLogginMessage);
            }
            if (File.Exists(MessageToFileLogWriterFactory.FileName)) File.Delete(MessageToFileLogWriterFactory.FileName);
            testTasks.AsParallel().ForAll(t => t.Start());
            Task.WaitAll(testTasks);
            var loggedMessages = File.ReadAllLines(MessageToFileLogWriterFactory.FileName);
            foreach (var testLoggingMessage in loggingMessages)
            {
                bool shouldBeLogged = testLoggingMessage.Delay >= logDelay;
                Assert.AreEqual(shouldBeLogged, loggedMessages.Contains(testLoggingMessage.Message));
            }
        }
    }
}

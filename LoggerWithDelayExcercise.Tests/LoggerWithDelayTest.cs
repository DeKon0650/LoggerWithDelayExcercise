using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
//using LoggerWithDelayExcercise.Core;
using LoggerWithDelayExcercise.Core.Implementations.ListLogger;
using LoggerWithDelayExcercise.Core.Implementations.MessageToFile;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static LoggerWithDelayExcercise.Core.LogHandler;

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
            var logger = new LoggerWithDelay(logDelay);
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
                    Debug.WriteLine($"(1a) {testMessage.Message} [{DT(DateTime.Now)}] started logger");
                    var handler = logger.LogWithDelay(testMessage.Message);
                    Debug.WriteLine($"(2a) {testMessage.Message} [{DT(DateTime.Now)}] gone sleep for {testMessage.Delay}");
                    Thread.Sleep(testMessage.Delay);
                    Debug.WriteLine($"(3a) {testMessage.Message} [{DT(DateTime.Now)}] woke up and started to cancel");
                    handler.Cancel();
                }, testLogginMessage);
            }
            if (File.Exists(MessageToFileLogWriterFactory.FileName)) File.Delete(MessageToFileLogWriterFactory.FileName);
            testTasks.AsParallel().ForAll(t => t.Start());
            Task.WaitAll(testTasks);
            Debug.WriteLine("");
            Debug.WriteLine("Test data:");
            foreach (var loggingMessage in loggingMessages)
            {
                Debug.WriteLine($"{loggingMessage.Message}\t{loggingMessage.Delay}");
            }
            var loggedMessages = File.ReadAllLines(MessageToFileLogWriterFactory.FileName);
            Debug.WriteLine("");
            Debug.WriteLine("Logged:");
            foreach (var loggedMessage in loggedMessages)
            {
                Debug.WriteLine($"{loggedMessage}");
            }
            foreach (var testLoggingMessage in loggingMessages)
            {
                bool shouldBeLogged = testLoggingMessage.Delay >= logDelay;
                Assert.AreEqual(shouldBeLogged, loggedMessages.Contains(testLoggingMessage.Message));
            }
        }
    }
}

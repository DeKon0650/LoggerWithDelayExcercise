namespace LoggerWithDelayExcercise.Core.Implementations.MessageToFile
{
    public class MessageToFileLogWriterFactory : ILogWriterFactory
    {
        public const string FileName = "messages.log";

        public ILogWriter CreateLogWriter()
        {
            return new MessageToFileLogWriter(FileName);
        }
    }
}

namespace LoggerWithDelayExcercise.Core.Implementations.MessageToFile
{
    public class MessageToFileLogWriterFactory : ILogWriterFactory
    {
        public const string FileName = "messages.log";

        private readonly string _message;

        public MessageToFileLogWriterFactory(string message)
        {
            _message = message;
        }

        public ILogWriter CreateLogWriter()
        {
            return new MessageToFileLogWriter(_message, FileName);
        }
    }
}

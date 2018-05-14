namespace LoggerWithDelayExcercise.Core
{
    public interface ILogWriterFactory
    {
        ILogWriter CreateLogWriter();
    }
}

using System.Data;

namespace SqlBatchProcess
{
    internal interface ICommandRecorder
    {
        void Record(string commandText, IDataParameterCollection parameters);
        void Run();
        string GetRecordedSql();
    }
}
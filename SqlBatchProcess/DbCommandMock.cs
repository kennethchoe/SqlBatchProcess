using System;
using System.Data;

namespace SqlBatchProcess
{
    internal class DbCommandMock : IDbCommand
    {
        public IDbConnection Connection { get; set; }
        public IDbTransaction Transaction { get; set; }
        public string CommandText { get; set; }
        public int CommandTimeout { get; set; }
        public CommandType CommandType { get; set; }
        public IDataParameterCollection Parameters { get; private set; }
        public UpdateRowSource UpdatedRowSource { get; set; }

        private readonly ICommandRecorder _commandRecorder;

        public DbCommandMock(ICommandRecorder commandRecorder)
        {
            _commandRecorder = commandRecorder;
            Parameters = new ParameterCollectionMock();
        }

        public IDbDataParameter CreateParameter()
        {
            return new DataParameterMock();
        }

        public int ExecuteNonQuery()
        {
            _commandRecorder.Record(CommandText, Parameters);
            return 0;
        }

        public void Dispose()
        {

        }

        public void Prepare()
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public IDataReader ExecuteReader()
        {
            throw new ReaderOperationNotAllowedException();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            throw new ReaderOperationNotAllowedException();
        }

        public object ExecuteScalar()
        {
            throw new ReaderOperationNotAllowedException();
        }
    }

    public class ReaderOperationNotAllowedException: InvalidOperationException
    {
        public ReaderOperationNotAllowedException(): base("Reader operation cannot be recorded.")
        {}
    }
}
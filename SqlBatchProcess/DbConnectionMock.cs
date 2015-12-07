using System;
using System.Data;

namespace SqlBatchProcess
{
    internal class DbConnectionMock : IDbConnection
    {
        public string ConnectionString { get; set; }
        public int ConnectionTimeout { get; set; }
        public string Database { get; set; }
        public ConnectionState State { get; set; }

        private readonly ICommandRecorder _commandRecorder;

        public DbConnectionMock(ICommandRecorder commandRecorder)
        {
            _commandRecorder = commandRecorder;
        }

        public IDbCommand CreateCommand()
        {
            return new DbCommandMock(_commandRecorder) { Connection = this };
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IDbTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            
        }

        public void ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            
        }
    }
}
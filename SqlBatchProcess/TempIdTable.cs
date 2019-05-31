using System;
using System.Collections.Generic;
using System.Data;

namespace SqlBatchProcess
{
    public class TempIdTable : IDisposable
    {
        public string TableName;
        private IDbConnection _conn;

        public TempIdTable(IDbConnection conn, IEnumerable<int> ids, string tableName)
        {
            Construct(conn, ids, tableName, "int");
        }

        public TempIdTable(IDbConnection conn, IEnumerable<long> ids, string tableName)
        {
            Construct(conn, ids, tableName, "bigint");
        }

        public TempIdTable(IDbConnection conn, IEnumerable<Guid> ids, string tableName)
        {
            Construct(conn, ids, tableName, "uniqueidentifier");
        }

        private void Construct<T>(IDbConnection conn, IEnumerable<T> ids, string tableName, string sqlType)
        {
            TableName = tableName;
            _conn = conn;

            ExecuteNonQuery(conn, "create table " + tableName + " (id " + sqlType + " not null)");

            var batchRunner = new SqlBatchRunner(conn);

            foreach (var id in ids)
            {
                ExecuteNonQuery(batchRunner.RecordingConnection, "insert into " + tableName + "(id) values('" + id + "')");
            }

            batchRunner.Run();
        }

        private void ExecuteNonQuery(IDbConnection conn, string sql)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        public void Dispose()
        {
            ExecuteNonQuery(_conn, "drop table " + TableName);
        }
    }
}

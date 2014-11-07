using System;
using System.Collections.Generic;
using System.Data;

namespace SqlBatchProcess
{
    public class TempIdTable : IDisposable
    {
        public readonly string TableName;
        private readonly IDbConnection _conn;

        public TempIdTable(IDbConnection conn, IEnumerable<int> ids, string tableName)
        {
            TableName = tableName;
            _conn = conn;

            ExecuteNonQuery(conn, "create table " + tableName + " (id int not null)");

            var batchRunner = new SqlBatchRunner(conn);

            foreach (var id in ids)
            {
                ExecuteNonQuery(batchRunner.RecordingConnection, "insert into " + tableName + "(id) values(" + id + ")");
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

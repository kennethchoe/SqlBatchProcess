using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using SqlBatchProcess;

namespace Test
{
    public class TempIdTable : IDisposable
    {
        public readonly string TableName;
        private readonly IDbConnection _conn;

        public TempIdTable(IDbConnection conn, IEnumerable<int> ids, string tableName)
        {
            TableName = tableName;
            _conn = conn;

            conn.Execute("create table " + tableName + " (id int not null)");

            var batchRunner = new SqlBatchRunner(conn);

            foreach (var id in ids)
                batchRunner.RecordingConnection.Execute("insert into " + tableName + "(id) values(@id)", new { id });

            batchRunner.Run();
        }

        public void Dispose()
        {
            _conn.Execute("drop table " + TableName);
        }
    }
}

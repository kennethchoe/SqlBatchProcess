using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Dapper;
using NUnit.Framework;
using SqlBatchProcess;

namespace Test
{
    [TestFixture]
    public class SqlBatchRunnerBehavior
    {
        private const int RunCount = 10000;
        private const string TempTable = "#temp_table";

        [Test]
        public void ComparePerformance()
        {
            var ids = Enumerable.Range(1, RunCount);

            WritePerformanceLog(() => InsertOneByOne(ids), "Insert one by one");
            WritePerformanceLog(() => InsertUsingSqlBatchRunner(ids), "Insert using SQLBatchRunner");
        }

        private void WritePerformanceLog(Action action, string title)
        {
            var t = new Stopwatch();
            t.Start();
            TestDb.Connection.Execute("create table " + TempTable + " (id int not null)");
            action.Invoke();
            TestDb.Connection.Execute("drop table " + TempTable);
            t.Stop();
            Debug.Print("{0}: {1}", title, t.Elapsed);
        }

        private void InsertOneByOne(IEnumerable<int> ids)
        {
            foreach (var id in ids)
                InsertOneRecord(TestDb.Connection, id);
        }

        private void InsertUsingSqlBatchRunner(IEnumerable<int> ids)
        {
            var batchRunner = new SqlBatchRunner(TestDb.Connection);
            foreach (var id in ids)
                InsertOneRecord(batchRunner.RecordingConnection, id);

            batchRunner.Run();
        }

        private void InsertOneRecord(IDbConnection conn, int id)
        {
            conn.Execute("insert into " + TempTable + "(id) values(@id)", new { id });
        }

        [Test]
        public void GetSqlStatementTest_IEnumerableHandling()
        {
            var ids = Enumerable.Range(1, 10);

            var batchRunner = new SqlBatchRunner(TestDb.Connection);
            batchRunner.RecordingConnection.Execute("select 1 where 1 in (@ids)", new { ids });

            var sql = batchRunner.GetRecordedSql();
            Assert.That(sql, Is.EqualTo("select 1 where 1 in (('1','2','3','4','5','6','7','8','9','10'))\r\n"));
        }

        [Test]
        public void GetSqlStatementTest_ArrayHandling()
        {
            var ids = Enumerable.Range(1, 3).Select(x => new { id = x });

            var batchRunner = new SqlBatchRunner(TestDb.Connection);
            batchRunner.RecordingConnection.Execute("select @id", ids);

            var sql = batchRunner.GetRecordedSql();
            Assert.That(sql, Is.EqualTo("select 1\r\nselect 2\r\nselect 3\r\n"));
        }
    }
}

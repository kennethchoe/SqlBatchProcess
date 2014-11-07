using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using NUnit.Framework;
using SqlBatchProcess;

namespace Test
{
    [TestFixture]
    class TempIdTableSample
    {
        private const int RunCount = 100;
        private const string TempTable = "#temp_table";
        
        [Test]
        public void TempIdTableTest()
        {
            var ids = new List<int>();
            ids.AddRange(Enumerable.Range(1, RunCount));

            int result;

            using (new TempIdTable(TestDb.Connection, ids, TempTable))
            {
                result = TestDb.Connection.Query<int>("select count(*) from " + TempTable).First();
            }

            Assert.That(result, Is.EqualTo(RunCount));
        }
    }
}

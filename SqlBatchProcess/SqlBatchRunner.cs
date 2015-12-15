using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SqlBatchProcess
{
    public class SqlBatchRunner : ICommandRecorder
    {
        private readonly IDbConnection _conn;
        public IDbConnection RecordingConnection { get; private set; }
        private readonly List<string> _recordedCommands;

        public int BatchExecutionSizeInBytes { get; set; }

        public SqlBatchRunner(IDbConnection conn)
        {
            _conn = conn;
            RecordingConnection = new DbConnectionMock(this);
            _recordedCommands = new List<string>();
            BatchExecutionSizeInBytes = 100000;
        }

        public void Run()
        {
            var sb = new StringBuilder();
            foreach (var recordedCommand in _recordedCommands)
            {
                sb.AppendLine(recordedCommand);

                if (sb.Length > BatchExecutionSizeInBytes)
                {
                    ExecuteCommand(sb);
                    sb.Clear();
                }
            }

            ExecuteCommand(sb);
        }

        private string BuildSql(string commandText, IDataParameterCollection parameters)
        {
            IParameterValueConverter converter = new SqlParameterValueConverter();
            var singleSql = converter.Convert(commandText, parameters);
            return singleSql;
        }

        public string GetRecordedSql()
        {
            var sb = new StringBuilder();
            foreach (var recordedCommand in _recordedCommands)
                sb.AppendLine(recordedCommand);

            return sb.ToString();
        }

        private void ExecuteCommand(StringBuilder sb)
        {
            var command = _conn.CreateCommand();
            command.CommandText = sb.ToString();

            if (command.CommandText == "") return;

            command.ExecuteNonQuery();
        }

        public void Record(string commandText, IDataParameterCollection parameters)
        {
            var sql = BuildSql(commandText, parameters);
            _recordedCommands.Add(sql);
        }
    }
}

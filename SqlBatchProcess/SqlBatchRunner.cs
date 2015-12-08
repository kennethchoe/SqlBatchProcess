using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SqlBatchProcess
{
    public class SqlBatchRunner : ICommandRecorder
    {
        private readonly IDbConnection _conn;
        public IDbConnection RecordingConnection { get; private set; }
        private readonly List<RecordedCommand> _recordedCommands;

        public int BatchExecutionSizeInBytes { get; set; }

        public SqlBatchRunner(IDbConnection conn)
        {
            _conn = conn;
            RecordingConnection = new DbConnectionMock(this);
            _recordedCommands = new List<RecordedCommand>();
            BatchExecutionSizeInBytes = 100000;
        }

        public void Run()
        {
            var sb = new StringBuilder();
            foreach (var recordedCommand in _recordedCommands)
            {
                sb.AppendLine(BuildSql(recordedCommand));

                if (sb.Length > BatchExecutionSizeInBytes)
                {
                    ExecuteCommand(sb);
                    sb.Clear();
                }
            }

            ExecuteCommand(sb);
        }

        private string BuildSql(RecordedCommand recordedCommand)
        {
            IParameterValueConverter converter = new SqlParameterValueConverter();
            var singleSql = converter.Convert(recordedCommand.CommandText, recordedCommand.Parameters);
            return singleSql;
        }

        public string GetRecordedSql()
        {
            var sb = new StringBuilder();
            foreach (var recordedCommand in _recordedCommands)
                sb.AppendLine(BuildSql(recordedCommand));

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
            _recordedCommands.Add(new RecordedCommand(commandText, parameters));
        }
    }
}

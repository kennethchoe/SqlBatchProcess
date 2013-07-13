using System.Collections.Generic;
using System.Data;
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
            IParameterValueConverter converter = new SqlParameterValueConverter();

            var sb = new StringBuilder();
            foreach (var recordedCommand in _recordedCommands)
            {
                var singleSql = recordedCommand.CommandText;

                foreach (IDbDataParameter p in recordedCommand.Parameters)
                    singleSql = converter.Convert(singleSql, p);

                sb.AppendLine(singleSql);

                if (sb.Length > BatchExecutionSizeInBytes)
                {
                    ExecuteCommand(sb);
                    sb.Clear();
                }
            }

            ExecuteCommand(sb);
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

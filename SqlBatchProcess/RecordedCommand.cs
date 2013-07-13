using System.Data;

namespace SqlBatchProcess
{
    internal class RecordedCommand
    {
        public readonly string CommandText;
        public readonly IDataParameterCollection Parameters;

        public RecordedCommand(string commandText, IDataParameterCollection parameters)
        {
            CommandText = commandText;
            Parameters = parameters;
        }
    }
}
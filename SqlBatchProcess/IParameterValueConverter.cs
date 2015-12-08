using System.Data;

namespace SqlBatchProcess
{
    internal interface IParameterValueConverter
    {
        string Convert(string commandText, IDataParameterCollection parameters);
    }
}
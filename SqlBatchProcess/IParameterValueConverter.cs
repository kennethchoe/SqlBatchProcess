using System.Data;

namespace SqlBatchProcess
{
    internal interface IParameterValueConverter
    {
        string Convert(string sql, IDbDataParameter parameter);
    }
}
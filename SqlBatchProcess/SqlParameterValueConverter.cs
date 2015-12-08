using System;
using System.Data;
using System.Linq;

namespace SqlBatchProcess
{
    class SqlParameterValueConverter : IParameterValueConverter
    {
        private string Convert(string sql, IDbDataParameter parameter)
        {
            String retval;
            var p = (DataParameterMock) parameter;

            if (p.Value is DBNull)
                retval = "null";
            else
                switch (p.DbType)
                {
                    case DbType.AnsiString:
                    case DbType.AnsiStringFixedLength:
                    case DbType.Binary:
                    case DbType.Date:
                    case DbType.DateTime:
                    case DbType.DateTime2:
                    case DbType.DateTimeOffset:
                    case DbType.Guid:
                    case DbType.String:
                    case DbType.StringFixedLength:
                    case DbType.Time:

                        // TODO: not sure what these are.
                        //                case DbType.Object:
                        //                case DbType.VarNumeric:


                        retval = "'" + p.Value.ToString().Replace("'", "''") + "'";
                        break;

                    case DbType.Boolean:
                        retval = (p.Value.ToBooleanOrDefault(false)) ? "1" : "0";
                        break;

                    default:
                        retval = p.Value.ToString().Replace("'", "''");
                        break;
                }

            return sql.Replace("@" + p.ParameterName, retval);
        }

        public string Convert(string commandText, IDataParameterCollection parameters)
        {
            var sqlParams = parameters.Cast<DataParameterMock>()
                .OrderByDescending(x => x.ParameterName.Length);

            var sql = commandText;
            foreach (var p in sqlParams)
                sql = Convert(sql, p);

            return sql;
        }
    }
}
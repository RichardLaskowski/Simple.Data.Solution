using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Simple.Data.Common.Extensions;
public static class DbCommandExtensions
{
    public static void AddParameters(this DbCommand dbCommand, IEnumerable<DbParameter> parameters)
    {
        dbCommand.Parameters.AddRange(parameters.ToArray());
    }

    public static void AddParameter(this DbCommand dbCommand, DbParameter parameter)
    {
        dbCommand.Parameters.Add(parameter);
    }
}

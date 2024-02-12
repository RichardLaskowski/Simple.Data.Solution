using System;
using System.Data;
using System.Data.Common;

using Microsoft.Data.SqlClient;

using MySql.Data.MySqlClient;

using Simple.Data.Common.Enums;

namespace Simple.Data.Common.Helpers;

public static class DbParameterHelper
{
    public static DbParameter CreateInputParameter(DataProvider dataProvider, string parameterName, object? value, DbType dbType, bool isNullable = true)
    {
        return CreateDbParameter(dataProvider, parameterName, value, dbType, ParameterDirection.Input, isNullable);
    }

    public static DbParameter CreateOutputParameter(DataProvider dataProvider, string parameterName, DbType dbType, bool isNullable = true)
    {
        return CreateDbParameter(dataProvider, parameterName, default, dbType, ParameterDirection.Output, isNullable);
    }

    public static DbParameter CreateDbParameter(DataProvider dataProvider)
    {
        return dataProvider switch
        {
            DataProvider.MySQL => new MySqlParameter(),
            DataProvider.MSSQL => new SqlParameter(),
            _ => throw new InvalidOperationException($"Invalid DataProvider: [{dataProvider}]"),
        };
    }

    public static DbParameter CreateDbParameter(DataProvider dataProvider, string parameterName, object? value, DbType dbType, ParameterDirection parameterDirection, bool isNullable = true)
    {
        return dataProvider switch
        {
            DataProvider.MySQL => CreateMySqlParameter(parameterName, value, dbType, parameterDirection, isNullable),
            DataProvider.MSSQL => CreateSqlParameter(parameterName, value, dbType, parameterDirection, isNullable),
            _ => throw new InvalidOperationException($"Invalid DataProvider: [{dataProvider}]"),
        };
    }

    private static MySqlParameter CreateMySqlParameter(
        string parameterName,
        object? value,
        DbType dbType,
        ParameterDirection parameterDirection,
        bool isNullable = true)
    {
        return new MySqlParameter()
        {
            ParameterName = parameterName,
            Direction = parameterDirection,
            Value = value,
            DbType = dbType,
            IsNullable = isNullable
        };
    }


    private static SqlParameter CreateSqlParameter(
        string parameterName,
        object? value,
        DbType dbType,
        ParameterDirection parameterDirection,
        bool isNullable = true)
    {
        return new SqlParameter()
        {
            ParameterName = parameterName,
            Direction = parameterDirection,
            Value = value,
            DbType = dbType,
            IsNullable = isNullable
        };
    }
}


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;

using MySql.Data.MySqlClient;

using Simple.Data.Common.Enums;
using Simple.Data.Common.Extensions;

namespace Simple.Data.Common.Helpers;

public static class DbCommandHelper
{
    public static DbCommand CreateDbCommand(
        DataProvider dataProvider,
        string sql,
        DbConnection dbConnection,
        DbTransaction? dbTransaction = default,
        IEnumerable<DbParameter>? dbParameters = default,
        CommandType commandType = CommandType.Text,
        bool isPrepared = false,
        int commandTimeout = 30)
    {
        DbCommand dbCommand = dataProvider switch
        {
            DataProvider.MySQL => new MySqlCommand(sql, (MySqlConnection)dbConnection),
            DataProvider.MSSQL => new SqlCommand(sql, (SqlConnection)dbConnection),
            _ => throw new InvalidOperationException($"Invalid DataProvider: [{dataProvider}]"),
        };

        if (dbParameters is not null)
        {
            dbCommand.AddParameters(dbParameters);
        }

        if (dbTransaction is not null)
        {
            dbCommand.Transaction = dbTransaction;
        }

        if (isPrepared)
        {
            dbCommand.Prepare();
        }

        dbCommand.CommandType = commandType;
        dbCommand.CommandTimeout = commandTimeout;

        return dbCommand;
    }

    public static async Task<DbCommand> CreateDbCommandAsync(DataProvider dataProvider,
        string sql,
        DbConnection dbConnection,
        DbTransaction? dbTransaction = default,
        IEnumerable<DbParameter>? dbParameters = default,
        CommandType commandType = CommandType.Text,
        bool isPrepared = false,
        int commandTimeout = 30)
    {
        DbCommand dbCommand = dataProvider switch
        {
            DataProvider.MySQL => new MySqlCommand(sql, (MySqlConnection)dbConnection),
            DataProvider.MSSQL => new SqlCommand(sql, (SqlConnection)dbConnection),
            _ => throw new InvalidOperationException($"Invalid DataProvider: [{dataProvider}]"),
        };

        if (dbParameters is not null)
        {
            dbCommand.AddParameters(dbParameters);
        }

        if (dbTransaction is not null)
        {
            dbCommand.Transaction = dbTransaction;
        }

        if (isPrepared)
        {
            await dbCommand.PrepareAsync();
        }

        dbCommand.CommandType = commandType;
        dbCommand.CommandTimeout = commandTimeout;

        return dbCommand;
    }
}

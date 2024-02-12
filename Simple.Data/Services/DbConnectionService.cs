using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Diagnostics;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

using MySql.Data.MySqlClient;

using Simple.Data.Common.Enums;
using Simple.Data.Common.Helpers;
using Simple.Data.Common.Options;

namespace Simple.Data.Services;

public class DbConnectionService
{
    #region Fields

    private readonly DbConnection _dbConnection;
    private readonly DbConnectionUnitOfWork _dbConnectionUnitOfWork;

    #endregion Fields


    #region Properties

    public DbConnectionOptions DbConnectionOptions { get; }

    #endregion Properties


    #region Constructors

    public DbConnectionService(DbConnectionOptions dbConnectionOptions)
    {
        this.DbConnectionOptions = dbConnectionOptions;
        _dbConnection = CreateDbConnection();
        _dbConnectionUnitOfWork = new DbConnectionUnitOfWork(_dbConnection);
    }

    public DbConnectionService(IOptions<DbConnectionOptions> dbConnectionOptions)
    {
        this.DbConnectionOptions = dbConnectionOptions.Value;
        _dbConnection = CreateDbConnection();
        _dbConnectionUnitOfWork = new DbConnectionUnitOfWork(_dbConnection);
    }

    #endregion Constructors


    #region Public Methods

    public DbTransaction BeginTransaction() => _dbConnectionUnitOfWork.BeginTransaction();

    public async Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) => await _dbConnectionUnitOfWork.BeginTransactionAsync();

    public void CommitTransaction() => _dbConnectionUnitOfWork.CommitTransaction();

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default) => await _dbConnectionUnitOfWork.CommitTransactionAsync();

    public void RollbackTransaction() => _dbConnectionUnitOfWork.RollbackTransaction();

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default) => await _dbConnectionUnitOfWork.RollbackTransactionAsync();

    public void Close() => _dbConnection.Close();

    public async Task CloseAsync() => await _dbConnection.CloseAsync();

    public DbBatch CreateBatch() => _dbConnection.CreateBatch();

    public DbCommand CreateCommand(string sql, DbTransaction? dbTransaction = default, IEnumerable<DbParameter>? dbParameters = default, CommandType commandType = CommandType.Text, bool isPrepared = false, int commandTimeout = 30)
    {
        return DbCommandHelper.CreateDbCommand(
            DbConnectionOptions.DataProvider,
            sql,
            _dbConnection,
            dbTransaction,
            dbParameters,
            commandType,
            isPrepared,
            commandTimeout
            );
    }

    public async Task<DbCommand> CreateCommandAsync(string sql, DbTransaction? dbTransaction = default, IEnumerable<DbParameter>? dbParameters = default, CommandType commandType = CommandType.Text, bool isPrepared = false, int commandTimeout = 30)
    {
        return await DbCommandHelper.CreateDbCommandAsync(
            DbConnectionOptions.DataProvider,
            sql,
            _dbConnection,
            dbTransaction,
            dbParameters,
            commandType,
            isPrepared,
            commandTimeout
            );
    }

    public DbConnection GetConnection() => _dbConnection;

    public void Open() => _dbConnection.Open();

    public async Task OpenAsync() => await _dbConnection.OpenAsync();

    public DbParameter CreateInputParameter(string parameterName, object? value, DbType dbType, bool isNullable = true)
    {
        return DbParameterHelper.CreateInputParameter(
            DbConnectionOptions.DataProvider,
            parameterName,
            value,
            dbType,
            isNullable);
    }

    public DbParameter CreateOutputParamter(string parameterName, DbType dbType, bool isNullable = true)
    {
        return DbParameterHelper.CreateOutputParameter(
            DbConnectionOptions.DataProvider,
            parameterName,
            dbType,
            isNullable);
    }

    public DbParameter CreateParameter() => DbParameterHelper.CreateDbParameter(DbConnectionOptions.DataProvider);

    public DbParameter CreateParameter(
        string parameterName,
        object? value,
        DbType dbType,
        ParameterDirection parameterDirection,
        bool isNullable = true)
    {
        return DbParameterHelper.CreateDbParameter(DbConnectionOptions.DataProvider, parameterName, value, dbType, parameterDirection, isNullable);
    }

    #endregion Public Methods


    #region Private Methods

    private DbConnection CreateDbConnection()
    {
        Guard.IsNotNull(DbConnectionOptions.DataProvider);

        DbConnection dbConnection = DbConnectionOptions.DataProvider switch
        {
            DataProvider.MySQL => new MySqlConnection(DbConnectionOptions.ConnectionString),
            DataProvider.MSSQL => new SqlConnection(DbConnectionOptions.ConnectionString),
            _ => throw new InvalidOperationException($"Invalid DataProvider: [{DbConnectionOptions.DataProvider}]")
        };

        return dbConnection;
    }

    #endregion Private Methods
}

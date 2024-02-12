using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Data.SqlClient;

using MySql.Data.MySqlClient;

using Simple.Data.Abstractions;
using Simple.Data.Common.Enums;
using Simple.Data.Common.Extensions;
using Simple.Data.Common.Results;

namespace Simple.Data.Services;

public class SqlService : ISqlService
{
    #region Fields

    private DbConnection _dbConnection = null!;
    private readonly IDbConnectionService _dbConnectionService;

    #endregion Fields


    #region Properties

    public DbConnection Connection
    {
        get
        {
            if (_dbConnection is null)
            {
                _dbConnection = _dbConnectionService.GetConnection();
            }

            return _dbConnection;
        }
    }

    #endregion Properties


    #region Constructors

    public SqlService(IDbConnectionService dbConnectionService) => this._dbConnectionService = dbConnectionService;

    #endregion Constructors


    #region Public Methods

    public DbTransaction BeginTransaction()
    {
        return _dbConnectionService.BeginTransaction();
    }

    public async Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return await _dbConnectionService.BeginTransactionAsync(cancellationToken);
    }

    public void CommitTransaction()
    {
        _dbConnectionService.CommitTransaction();
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _dbConnectionService.CommitTransactionAsync(cancellationToken);
    }

    public DbCommand CreateCommand(
        string sql,
        DbTransaction? dbTransaction = default,
        IEnumerable<DbParameter>? dbParameters = default,
        CommandType commandType = CommandType.Text,
        bool isPrepared = false,
        int commandTimeout = 30)
    {
        return _dbConnectionService.CreateCommand(
            sql,
            dbTransaction,
            dbParameters,
            commandType,
            isPrepared,
            commandTimeout);
    }

    public async Task<DbCommand> CreateCommandAsync(
        string sql,
        DbTransaction? dbTransaction = default,
        IEnumerable<DbParameter>? dbParameters = default,
        CommandType commandType = CommandType.Text,
        bool isPrepared = false,
        int commandTimeout = 30)
    {
        return await _dbConnectionService.CreateCommandAsync(
            sql,
            dbTransaction,
            dbParameters,
            commandType,
            isPrepared,
            commandTimeout);
    }

    public DbDataAdapter CreateDataAdapter()
    {
        DbDataAdapter dataAdapter;

        DataProvider DataProvider = _dbConnectionService.DbConnectionOptions.DataProvider;

        switch (DataProvider)
        {
            case DataProvider.MySQL:
                dataAdapter = new MySqlDataAdapter();
                break;
            case DataProvider.MSSQL:
                dataAdapter = new SqlDataAdapter();
                break;
            default:
                throw new InvalidOperationException($"Invalid DataProvider: [{DataProvider}]");
        }

        return dataAdapter;
    }

    public DbParameter CreateInputParameter(
        string parameterName,
        object? value,
        DbType dbType,
        bool isNullable = true)
    {
        return _dbConnectionService.CreateInputParameter(parameterName, value, dbType, isNullable);
    }

    public DbParameter CreateParameter()
    {
        return _dbConnectionService.CreateParameter();
    }

    public DbParameter CreateParameter(
        string parameterName,
        object value,
        DbType dbType,
        ParameterDirection parameterDirection,
        bool isNullable = true)
    {
        return _dbConnectionService.CreateParameter(
            parameterName,
            value,
            dbType,
            parameterDirection,
            isNullable);
    }

    public DbParameter CreateOutputParameter(
        string parameterName,
        DbType dbType,
        bool isNullable = true)
    {
        return _dbConnectionService.CreateOutputParameter(parameterName, dbType, isNullable);
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }

    public ExecuteInsertResult ExecuteTextInsert<T>(
        T item,
        string tableName,
        bool isPrepared = false,
        int commandTimeout = 30)
    {
        List<DbParameter> parameters = new List<DbParameter>();
        PropertyInfo[] propertyInfos = typeof(T).GetProperties();

        StringBuilder sqlStringBuilder = new();

        sqlStringBuilder.Append($"INSERT INTO {tableName} (");

        for (int propertyIndex = 0; propertyIndex < propertyInfos.Length; propertyIndex++)
        {
            PropertyInfo propertyInfo = propertyInfos[propertyIndex];

            sqlStringBuilder.Append($" {propertyInfo.Name}");

            if (propertyIndex != propertyInfos.Length - 1)
            {
                sqlStringBuilder.Append(',');
            }
        }

        sqlStringBuilder.Append(") VALUES (");

        for (int propertyIndex = 0; propertyIndex < propertyInfos.Length; propertyIndex++)
        {
            PropertyInfo propertyInfo = propertyInfos[propertyIndex];

            string parameterName = $" @{propertyInfo.Name}";
            string propertyType = propertyInfo.PropertyType.Name;
            DbType dbType = propertyType switch
            {
                "Bool" => DbType.Boolean,
                "Byte" => DbType.Byte,
                "Char" => DbType.AnsiStringFixedLength,
                "DateTime" => DbType.DateTime,
                "Decimal" => DbType.Decimal,
                "Double" => DbType.Double,
                "Float" => DbType.Decimal,
                "Guild" => DbType.Guid,
                "Int" => DbType.Int32,
                "Long" => DbType.Int64,
                "Short" => DbType.Int16,
                "String" => DbType.String,
                _ => DbType.Object,
            };


            sqlStringBuilder.Append(parameterName);

            parameters.Add(CreateInputParameter(
                parameterName,
                propertyInfo.GetValue(item),
                dbType));

            if (propertyIndex != propertyInfos.Length - 1)
            {
                sqlStringBuilder.Append(',');
            }
        }

        sqlStringBuilder.Append(@");");

        string sql = sqlStringBuilder.ToString();

        return ExecuteInsert(
            sql,
            parameters,
            CommandType.Text,
            isPrepared,
            commandTimeout);
    }

    public ExecuteInsertResult ExecuteTextInsert(
        string tableName,
        List<DbParameter> parameters,
        bool isPrepared = false,
        int commandTimeout = 30)
    {
        string sql = BuildInsertStatement(
            tableName,
            parameters);

        ExecuteInsertResult executeInsertResult = ExecuteInsert(
            sql,
            parameters,
            CommandType.Text,
            isPrepared,
            commandTimeout);

        return executeInsertResult;
    }

    public ExecuteInsertResult ExecuteInsert(
        string sql,
        IEnumerable<DbParameter>? dbParameters = default,
        CommandType commandType = CommandType.Text,
        bool isPrepared = false,
        int commandTimeout = 30)
    {
        DbCommand? dbCommand = null;
        ExecuteInsertResult executeInsertResult = new ExecuteInsertResult();

        try
        {
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }

            switch (_dbConnectionService.DbConnectionOptions.DataProvider)
            {
                case DataProvider.MySQL:
                    break;
                case DataProvider.MSSQL:
                    sql += "SET @RETURNEDID=SCOPE_IDENTITY();";
                    dbCommand?.AddParameter(CreateOutputParameter("@ReturnedId", DbType.Int64));
                    break;
                default: throw new InvalidOperationException($"Invalid DataProvider: [{_dbConnectionService.DbConnectionOptions.DataProvider}]");
            }

            dbCommand = CreateCommand(
                sql,
                default,
                dbParameters,
                commandType,
                isPrepared,
                commandTimeout);

            executeInsertResult.RowsAffected = dbCommand.ExecuteNonQuery();

            executeInsertResult.ID = GetInsertId(dbCommand);
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            dbCommand?.Dispose();
        }

        return executeInsertResult;
    }

    public int ExecuteNonQuery(
        string sql,
        IEnumerable<DbParameter>? dbParameters = default,
        CommandType commandType = CommandType.Text,
        bool isPrepared = false,
        int commandTimeout = 30)
    {
        DbCommand? dbCommand = null;
        int rowsAffected;

        try
        {
            if (Connection.State != ConnectionState.Open)
            {
                _dbConnectionService.Open();
            }

            dbCommand = CreateCommand(
                sql,
                default,
                dbParameters,
                commandType,
                isPrepared,
                commandTimeout);

            rowsAffected = dbCommand.ExecuteNonQuery();
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            dbCommand?.Dispose();
        }

        return rowsAffected;
    }

    public async Task<int> ExecuteNonQueryAsync(
        string sql,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteNonQueryAsync(
            sql,
            default,
            default,
            default,
            default,
            cancellationToken);
    }

    public async Task<int> ExecuteNonQueryAsync(
        string sql,
        IEnumerable<DbParameter>? dbParameters = default,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteNonQueryAsync(
            sql,
            dbParameters,
            default,
            default,
            default,
            cancellationToken);
    }

    public async Task<int> ExecuteNonQueryAsync(
       string sql,
       IEnumerable<DbParameter>? dbParameters = default,
       CommandType commandType = CommandType.Text,
        bool isPrepared = false,
       int commandTimeout = 30,
       CancellationToken cancellationToken = default)
    {
        DbCommand? dbCommand = null;
        int rowsAffected;

        try
        {
            if (Connection.State != ConnectionState.Open)
            {
                _dbConnectionService.Open();
            }

            dbCommand = CreateCommand(
                sql,
                default,
                dbParameters,
                commandType,
                isPrepared,
                commandTimeout);

            rowsAffected = await dbCommand.ExecuteNonQueryAsync(cancellationToken);
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            dbCommand?.Dispose();
        }

        return rowsAffected;
    }

    public object? ExecuteScalar(
        string sql,
        IEnumerable<DbParameter>? dbParameters = default,
        CommandType commandType = CommandType.Text,
        bool isPrepared = false,
        int commandTimeout = 30)
    {
        DbCommand? dbCommand = null;
        object? result;

        try
        {
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }

            dbCommand = CreateCommand(
                sql,
                default,
                dbParameters,
                commandType,
                isPrepared,
                commandTimeout);

            result = dbCommand.ExecuteScalar();
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            dbCommand?.Dispose();
        }

        return result;
    }

    public async Task<object?> ExecuteScalarAsync(
        string sql,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteScalarAsync(
            sql,
            default,
            default,
            default,
            default,
            cancellationToken);
    }

    public async Task<object?> ExecuteScalarAsync(
        string sql,
        IEnumerable<DbParameter>? dbParameters = default,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteScalarAsync(
            sql,
            dbParameters,
            default,
            default,
            default,
            cancellationToken);
    }

    public async Task<object?> ExecuteScalarAsync(
       string sql,
       IEnumerable<DbParameter>? dbParameters = default,
       CommandType commandType = CommandType.Text,
       bool isPrepared = false,
       int commandTimeout = 30,
       CancellationToken cancellationToken = default)
    {
        DbCommand? dbCommand = null;
        object? result;

        try
        {
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }

            dbCommand = CreateCommand(
                sql,
                default,
                dbParameters,
                commandType,
                isPrepared,
                commandTimeout);

            result = await dbCommand.ExecuteScalarAsync(cancellationToken);
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            dbCommand?.Dispose();
        }

        return result;
    }

    public DataTable ExecuteTable(
        string sql,
        IEnumerable<DbParameter>? dbParameters = default,
        CommandType commandType = CommandType.Text,
        bool isPrepared = false,
        int commandTimeout = 30)
    {
        DbCommand? dbCommand = null;
        DataTable dataTable = new DataTable();
        DbDataAdapter dataAdapter = CreateDataAdapter();

        try
        {
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }

            dbCommand = CreateCommand(
                sql,
                default,
                dbParameters,
                commandType,
                isPrepared,
                commandTimeout);

            dataAdapter.SelectCommand = dbCommand;

            dataAdapter.Fill(dataTable);
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            dbCommand?.Dispose();
        }

        return dataTable;
    }

    public async Task<DataTable> ExecuteTableAsync(
        string sql,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteTableAsync(
            sql,
            default,
            default,
            default,
            default,
            cancellationToken);
    }

    public async Task<DataTable> ExecuteTableAsync(
        string sql,
        IEnumerable<DbParameter>? dbParameters = default,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteTableAsync(
            sql,
            dbParameters,
            default,
            default,
            default,
            cancellationToken);
    }

    public async Task<DataTable> ExecuteTableAsync(
                   string sql,
       IEnumerable<DbParameter>? dbParameters = default,
       CommandType commandType = CommandType.Text,
       bool isPrepared = false,
       int commandTimeout = 30,
       CancellationToken cancellationToken = default
        )
    {
        DbCommand? dbCommand = null;
        DataTable dataTable = new DataTable();
        DbDataAdapter dataAdapter = CreateDataAdapter();

        try
        {
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }

            dbCommand = CreateCommand(
                sql,
                default,
                dbParameters,
                commandType,
                isPrepared,
                commandTimeout);

            dataAdapter.SelectCommand = dbCommand;

            await Task.Run(() => dataAdapter.Fill(dataTable));
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            dbCommand?.Dispose();
        }

        return dataTable;
    }

    public DbDataReader ExecuteReader(
        string sql,
        IEnumerable<DbParameter>? dbParameters = default,
        CommandType commandType = CommandType.Text,
        bool isPrepared = false,
        int commandTimeout = 30)
    {
        DbCommand? dbCommand = null;
        DbDataReader? dbDataReader = null;

        try
        {
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }

            dbCommand = CreateCommand(
                sql,
                default,
                dbParameters,
                commandType,
                isPrepared,
                commandTimeout);

            dbDataReader = dbCommand.ExecuteReader();
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            dbCommand?.Dispose();
        }

        return dbDataReader;
    }

    public async Task<DbDataReader> ExecuteReaderAsync(
        string sql,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteReaderAsync(
            sql,
            default,
            default,
            default,
            default,
            cancellationToken);
    }

    public async Task<DbDataReader> ExecuteReaderAsync(
        string sql,
        IEnumerable<DbParameter>? dbParameters = default,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteReaderAsync(
            sql,
            dbParameters,
            default,
            default,
            default,
            cancellationToken);
    }

    public async Task<DbDataReader> ExecuteReaderAsync(
       string sql,
       IEnumerable<DbParameter>? dbParameters = default,
       CommandType commandType = CommandType.Text,
       bool isPrepared = false,
       int commandTimeout = 30,
       CancellationToken cancellationToken = default)
    {
        DbCommand? dbCommand = null;
        DbDataReader? dbDataReader = null;

        try
        {
            if (Connection.State != ConnectionState.Open)
            {
                Connection.Open();
            }

            dbCommand = CreateCommand(
                sql,
                default,
                dbParameters,
                commandType,
                isPrepared,
                commandTimeout);

            dbDataReader = await dbCommand.ExecuteReaderAsync(cancellationToken);
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            dbCommand?.Dispose();
        }

        return dbDataReader;
    }

    public void RollbackTransaction()
    {
        _dbConnectionService.RollbackTransaction();
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _dbConnectionService.RollbackTransactionAsync(cancellationToken);
    }

    public IEnumerable<T> Select<T>(
        string sql,
        IEnumerable<DbParameter>? parameters = default,
        CommandType commandType = CommandType.Text,
        bool isPrepared = false,
        int commandTimeout = 30) where T : new()
    {
        DbDataReader dbDataReader = ExecuteReader(
            sql,
            parameters,
            commandType,
            isPrepared,
            commandTimeout);

        IEnumerable<T> items = dbDataReader.HasRows
            ? dbDataReader.ToList<T>()
            : throw new InvalidOperationException($"Executing {sql} returned no records to cast to type {typeof(T)}.");

        dbDataReader.Close();

        return items;
    }

    public async Task<IEnumerable<T>> SelectAsync<T>(
        string sql,
        CancellationToken cancellationToken = default) where T : new()
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await SelectAsync<T>(
            sql,
            default,
            default,
            default,
            default,
            cancellationToken);
    }

    public async Task<IEnumerable<T>> SelectAsync<T>(
        string sql,
        IEnumerable<DbParameter> dbParameters,
        CancellationToken cancellationToken = default) where T : new()
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await SelectAsync<T>(
            sql,
            dbParameters,
            default,
            default,
            default,
            cancellationToken);
    }

    public async Task<IEnumerable<T>> SelectAsync<T>(
       string sql,
       IEnumerable<DbParameter>? dbParameters = default,
       CommandType commandType = CommandType.Text,
       bool isPrepared = false,
       int commandTimeout = 30,
       CancellationToken cancellationToken = default) where T : new()
    {
        DbDataReader dbDataReader = await ExecuteReaderAsync(
            sql,
            dbParameters,
            commandType,
            isPrepared,
            commandTimeout,
            cancellationToken);

        IEnumerable<T> items = dbDataReader.HasRows
            ? await dbDataReader.ToListAsync<T>(cancellationToken)
            : Enumerable.Empty<T>();

        await dbDataReader.CloseAsync();

        return items;
    }

    public T SelectFirst<T>(
        string sql,
        IEnumerable<DbParameter>? dbParameters = default,
        CommandType commandType = CommandType.Text,
        bool isPrepared = false,
        int commandTimeout = 30) where T : new()
    {
        DbDataReader dbDataReader = ExecuteReader(
            sql,
            dbParameters,
            commandType,
            isPrepared,
            commandTimeout);

        T item = dbDataReader.HasRows
            ? dbDataReader.First<T>()
            : throw new InvalidOperationException($"Executing {sql} returned no records to cast to type {typeof(T)}.");

        dbDataReader.Close();

        return item;
    }

    public async Task<T> SelectFirstAsync<T>(
        string sql,
        CancellationToken cancellationToken = default) where T : new()
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await SelectFirstAsync<T>(
            sql,
            default,
            default,
            default,
            default,
            cancellationToken);
    } 

    public async Task<T> SelectFirstAsync<T>(
        string sql,
        IEnumerable<DbParameter> dbParameters,
        CancellationToken cancellationToken = default) where T : new()
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await SelectFirstAsync<T>(
            sql,
            dbParameters,
            default,
            default,
            default,
            cancellationToken);
    }

    public async Task<T> SelectFirstAsync<T>(
       string sql,
       IEnumerable<DbParameter>? dbParameters = default,
       CommandType commandType = CommandType.Text,
       bool isPrepared = false,
       int commandTimeout = 30,
       CancellationToken cancellationToken = default) where T : new()
    {
        cancellationToken.ThrowIfCancellationRequested();

        DbDataReader dbDataReader = await ExecuteReaderAsync(
            sql,
            dbParameters,
            commandType,
            isPrepared,
            commandTimeout,
            cancellationToken);

        T item = dbDataReader.HasRows
            ? await dbDataReader.FirstAsync<T>(cancellationToken)
            : throw new InvalidOperationException($"Executing {sql} returned no records to cast to type {typeof(T)}.");

        await dbDataReader.CloseAsync();

        return item;
    }

    public T? SelectFirstOrDefault<T>(
        string sql,
        IEnumerable<DbParameter>? dbParameters = default,
        CommandType commandType = CommandType.Text,
        bool isPrepared = false,
        int commandTimeout = 30) where T : new()
    {
        DbDataReader dbDataReader = ExecuteReader(
            sql,
            dbParameters,
            commandType,
            isPrepared,
            commandTimeout);

        T? item = dbDataReader.HasRows
            ? dbDataReader.FirstOrDefault<T>()
            : default;

        dbDataReader.Close();

        return item;
    }

    public async Task<T?> SelectFirstOrDefaultAsync<T>(
        string sql,
        CancellationToken cancellationToken = default) where T : new()
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await SelectFirstOrDefaultAsync<T>(
            sql,
            default,
            default,
            default,
            default,
            cancellationToken);
    }

    public async Task<T?> SelectFirstOrDefaultAsync<T>(
        string sql,
        IEnumerable<DbParameter>? dbParameters = default,
        CancellationToken cancellationToken = default) where T : new()
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await SelectFirstOrDefaultAsync<T>(
            sql,
            dbParameters,
            default,
            default,
            default,
            cancellationToken);
    }

    public async Task<T?> SelectFirstOrDefaultAsync<T>(
       string sql,
       IEnumerable<DbParameter>? dbParameters = default,
       CommandType commandType = CommandType.Text,
       bool isPrepared = false,
       int commandTimeout = 30,
       CancellationToken cancellationToken = default) where T : new()
    {
        cancellationToken.ThrowIfCancellationRequested();

        DbDataReader dbDataReader = await ExecuteReaderAsync(
            sql,
            dbParameters,
            commandType,
            isPrepared,
            commandTimeout,
            cancellationToken);

        T? item = dbDataReader.HasRows
            ? await dbDataReader.FirstOrDefaultAsync<T>(cancellationToken)
            : default;

        await dbDataReader.CloseAsync();

        return item;
    }

    #endregion Public Methods


    #region Private Methods

    private string BuildInsertStatement(string tableName, List<DbParameter> parameters)
    {
        StringBuilder insertStatementStringBuilder = new StringBuilder();

        insertStatementStringBuilder.AppendLine($"INSERT INTO {tableName} (");

        for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
        {
            DbParameter parameter = parameters[parameterIndex];

            if (parameterIndex != parameters.Count - 1)
            {
                insertStatementStringBuilder.AppendLine($"{parameter.ParameterName},");
            }
            else
            {
                insertStatementStringBuilder.AppendLine($"{parameter.ParameterName}");
            }
        }

        insertStatementStringBuilder.AppendLine(") VALUES (");

        for (int parameterIndex = 0; parameterIndex < parameters.Count; parameterIndex++)
        {
            DbParameter parameter = parameters[parameterIndex];

            if (parameterIndex != parameters.Count - 1)
            {
                insertStatementStringBuilder.AppendLine($"@{parameter.ParameterName},");
            }
            else
            {
                insertStatementStringBuilder.AppendLine($"@{parameter.ParameterName}");
            }
        }

        insertStatementStringBuilder.AppendLine(@");");

        string insertStatementSql = insertStatementStringBuilder.ToString();

        return insertStatementSql;
    }
    
    private object GetInsertId(DbCommand dbCommand)
    {
        object id;

        switch (_dbConnectionService.DbConnectionOptions.DataProvider)
        {
            case DataProvider.MySQL:
                id = ((MySqlCommand)dbCommand).LastInsertedId;
                break;
            case DataProvider.MSSQL:
                id = dbCommand.Parameters["@ReturnedId"];
                break;
            default:
                throw new InvalidOperationException($"Invalid DataProvider: [{_dbConnectionService.DbConnectionOptions.DataProvider}]");
        };

        return id;
    }


    #endregion Private Methods
}

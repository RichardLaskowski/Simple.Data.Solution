using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

using Simple.Data.Common.Results;

namespace Simple.Data.Abstractions;

/// <summary>
/// 
/// </summary>
public interface ISqlService : IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Starts a database transaction.
    /// </summary>
    /// <returns>An object representing the new transaction.</returns>
    DbTransaction BeginTransaction();


    /// <summary>
    /// Asynchronously begins a database transaction.
    /// </summary>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/></param>
    /// <returns>A task whose <see cref="Task{TResult}.Result"/> is an object representing the new transaction.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);


    /// <summary>
    /// Commits the database transaction.
    /// </summary>
    void CommitTransaction();


    /// <summary>
    /// Asynchronously commits the database transaction.
    /// </summary>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/></param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    DbDataAdapter CreateDataAdapter();


    /// <summary>
    /// Creates and returns a <see cref="DbCommand"/> object associated with the current connection.
    /// </summary>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="dbTransaction">The <see cref="DbTransaction"/> within which this <see cref="DbCommand"/> object executes.</param>
    /// <param name="dbParameters">The collection of <see cref="DbParameter"/> objects.</param>
    /// <param name="commandType">Determins how the command text is interpreted.</param>
    /// <param name="isPrepared">Indicates whether to create a prepared (or compiled) version of the command on the data source.</param>
    /// <param name="commandTimeout">The wait time (in seconds) before terminating the attrempt to execute the command and generating an error.</param>
    /// <returns>A <see cref="DbCommand"/> object.</returns>
    DbCommand CreateCommand(string commandText, DbTransaction? dbTransaction = default, IEnumerable<DbParameter>? dbParameters = default, CommandType commandType = CommandType.Text, bool isPrepared = false, int commandTimeout = 30);


    /// <summary>
    /// An asynchronous version of <see cref="CreateCommand(string, DbTransaction?, IEnumerable{DbParameter}?, CommandType, bool, int)"/> which creates and returns a <see cref="DbCommand"/> object associated with the current connection.
    /// </summary>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="dbTransaction">The <see cref="DbTransaction"/> within which this <see cref="DbCommand"/> object executes.</param>
    /// <param name="dbParameters">The collection of <see cref="DbParameter"/> objects.</param>
    /// <param name="commandType">Determins how the command text is interpreted.</param>
    /// <param name="isPrepared">Indicates whether to create a prepared (or compiled) version of the command on the data source.</param>
    /// <param name="commandTimeout">The wait time (in seconds) before terminating the attrempt to execute the command and generating an error.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    Task<DbCommand> CreateCommandAsync(string commandText, DbTransaction? dbTransaction = default, IEnumerable<DbParameter>? dbParameters = default, CommandType commandType = CommandType.Text, bool isPrepared = false, int commandTimeout = 30);


    /// <summary>
    /// Creates a new data provider specific instance of an DbParameter object.
    /// </summary>
    /// <param name="dbParameterName"></param>
    /// <param name="value"></param>
    /// <param name="dbType"></param>
    /// <param name="isNullable"></param>
    /// <returns></returns>
    DbParameter CreateInputParameter(
        string dbParameterName,
        object value,
        DbType dbType,
        bool isNullable = true);


    /// <summary>
    /// Creates a new data provider specific instance of a DbParamter object.
    /// </summary>
    /// <returns>A data provider specific DbParameter object.</returns>
    DbParameter CreateParameter();


    /// <summary>
    /// Creates a new data provider specific instance of a DbParamter object.
    /// </summary>
    /// <param name="dbParameterName">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <param name="dbType">The specific data type of the parameter.</param>
    /// <param name="parameterDirection">Indicates weather the parameter is input-only, output-only, bidirectional, or a stored procedure return value parameter.</param>
    /// <param name="isNullable">Indicates whether the parameter should accept null values.</param>
    /// <returns>A data provider specific DbParameter object.</returns>
    DbParameter CreateParameter(
        string dbParameterName,
        object value,
        DbType dbType,
        ParameterDirection parameterDirection,
        bool isNullable = true);


    /// <summary>
    /// Creates a new data provider specific instance of an output DbParamter object.
    /// </summary>
    /// <param name="dbParameterName">The name of the parameter.</param>
    /// <param name="dbType">The specific data type of the parameter.</param>
    /// <param name="isNullable">Indicates whether the parameter should accept null values.</param>
    /// <returns>A data provider specific DbParameter object.</returns>
    DbParameter CreateOutputParameter(
        string dbParameterName,
        DbType dbType,
        bool isNullable = true);


    /// <summary>
    /// Executes the command agains its connection object, returning the number of rows affected.
    /// </summary>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="dbParameters">The collection of DbParameter objects</param>
    /// <param name="commandType">Determines how the command text is interpreted.</param>
    /// <param name="isPrepared">Indicates if the command should be prepared.</param>
    /// <param name="commandTimeout">The wait time (in seconds) before terminating the attempt to execute the command and generating an error.</param>
    /// <returns>The number of rows affected.</returns>
    /// <remarks>
    /// You can use <see cref="ExecuteNonQuery(string, IEnumerable{DbParameter}?, CommandType, bool, int)"/> to perform catalog operations (for example, querying the structure of a 
    /// database or creating database objects such as tables), or to change the data in a database by executing UPDATE, INSERT, or DELETE statements.
    /// 
    /// Although <see cref="ExecuteNonQuery(string, IEnumerable{DbParameter}?, CommandType, bool, int) "/> does not return any rows, any output parameters or return values mapped
    /// to parameters are populated with data.
    /// 
    /// For UPDATE, INSERT, and DELETE statements, the return value is the number of rows affected by the command. For all other types of statements, the return value is -1.
    /// </remarks>
    int ExecuteNonQuery(
        string commandText,
        IEnumerable<DbParameter>? dbParameters = default,
        CommandType commandType = CommandType.Text,
        bool isPrepared = false,
        int commandTimeout = 30);


    /// <summary>
    /// An asynchronous version of <see cref="ExecuteNonQuery(string, IEnumerable{DbParameter}?, CommandType, bool, int)"/> which executes the command text, returning the number of rows affected."/>
    /// </summary>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="DbException">An error occured while executing the command text.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    Task<int> ExecuteNonQueryAsync(
        string commandText,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// An asynchronous version of <see cref="ExecuteNonQuery(string, IEnumerable{DbParameter}?, CommandType, bool, int)"/> which executes the command text, returning the number of rows affected."/>
    /// </summary>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="dbParameters">The collection of DbParameter objects</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="DbException">An error occured while executing the command text.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This expection is stored into the returned task.</exception>
    Task<int> ExecuteNonQueryAsync(
        string commandText,
        IEnumerable<DbParameter>? dbParameters = null,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// This method implements the asynchronous version of <see cref="ExecuteNonQuery(string, IEnumerable{DbParameter}?, CommandType, bool, int)"/> but returns a <see cref="Task"/>
    /// </summary>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="dbParameters">The collection of DbParameter objects</param>
    /// <param name="commandType">Determines how the command text is interpreted.</param>
    /// <param name="isPrepared">Indicates if the command should be prepared.</param>
    /// <param name="commandTimeout">The wait time (in seconds) before terminating the attempt to execute the command and generating an error.</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="DbException">An error occured while executing the command text.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This expection is stored into the returned task.</exception>
    Task<int> ExecuteNonQueryAsync(
        string commandText,
        IEnumerable<DbParameter>? dbParameters = default,
        CommandType commandType = CommandType.Text,
        bool isPrepared = false,
        int commandTimeout = 30,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Executes the command against its connection, returning a DbDataReader which can be used to access the results.
    /// </summary>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="dbParameters">The collection of DbParameter objects</param>
    /// <param name="commandType">Determines how the command text is interpreted.</param>
    /// <param name="isPrepared">Indicates if the command should be prepared.</param>
    /// <param name="commandTimeout">The wait time (in seconds) before terminating the attempt to execute the command and generating an error.</param>
    /// <returns>A DbDataReader object.</returns>
    DbDataReader ExecuteReader(
        string commandText,
        IEnumerable<DbParameter>? dbParameters = default,
        CommandType commandType = CommandType.Text,
        bool isPrepared = false,
        int commandTimeout = 30);


    /// <summary>
    /// An asynchronous version of ExecuteReader, which executes the command against its connection, returning a DbDataReader which can be used to access the results.
    /// </summary>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="DbException">An error occurred while executing the command text.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>"
    Task<DbDataReader> ExecuteReaderAsync(
        string commandText,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// An asynchronous version of ExecuteReader, which executes the command against its connection, returning a DbDataReader which can be used to access the results.
    /// </summary>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="dbParameters">The collection of DbParameter objects</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="DbException">An error occurred while executing the command text.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>"
    Task<DbDataReader> ExecuteReaderAsync(
        string commandText,
        IEnumerable<DbParameter>? dbParameters = null,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// An asynchronous version of ExecuteReader, which executes the command against its connection, returning a DbDataReader which can be used to access the results.
    /// </summary>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="dbParameters">The collection of DbParameter objects</param>
    /// <param name="commandType">Determines how the command text is interpreted.</param>
    /// <param name="isPrepared">Indicates if the command should be prepared.</param>
    /// <param name="commandTimeout">The wait time (in seconds) before terminating the attempt to execute the command and generating an error.</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/></param>>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="DbException">An error occurred while executing the command text.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>"
    Task<DbDataReader> ExecuteReaderAsync(
        string commandText,
        IEnumerable<DbParameter>? dbParameters = default,
        CommandType commandType = CommandType.Text,
        bool isPrepared = false,
        int commandTimeout = 30,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Executes the command against its connection, returning a <see cref="DataTable"/> which can be used to access the results.
    /// </summary>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="dbParameters">The collection of DbParameter objects</param>
    /// <param name="commandType">Determines how the command text is interpreted.</param>
    /// <param name="isPrepared">Indicates if the command should be prepared.</param>
    /// <param name="commandTimeout">The wait time (in seconds) before terminating the attempt to execute the command and generating an error.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    DataTable ExecuteTable(
        string commandText,
        IEnumerable<DbParameter>? dbParameters = default,
        CommandType commandType = CommandType.Text,
        bool isPrepared = false,
        int commandTimeout = 30);


    /// <summary>
    /// An asynchronous version of ExecuteReader, which executes the command against its connection, returning a <see cref="DataTable"/> which can be used to access the results.
    /// </summary>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="DbException">An error occurred while executing the command text.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>"
    Task<DataTable> ExecuteTableAsync(
        string commandText,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// An asynchronous version of ExecuteReader, which executes the command against its connection, returning a <see cref="DataTable"/> which can be used to access the results.
    /// </summary>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="dbParameters">The collection of DbParameter objects</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="DbException">An error occurred while executing the command text.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>"
    Task<DataTable> ExecuteTableAsync(
        string commandText,
        IEnumerable<DbParameter>? dbParameters = null,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// An asynchronous version of ExecuteReader, which executes the command against its connection, returning a <see cref="DataTable"/> which can be used to access the results.
    /// </summary>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="dbParameters">The collection of DbParameter objects</param>
    /// <param name="commandType">Determines how the command text is interpreted.</param>
    /// <param name="isPrepared">Indicates if the command should be prepared.</param>
    /// <param name="commandTimeout">The wait time (in seconds) before terminating the attempt to execute the command and generating an error.</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="DbException">An error occurred while executing the command text.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>"
    Task<DataTable> ExecuteTableAsync(
        string commandText,
        IEnumerable<DbParameter>? dbParameters = default,
        CommandType commandType = CommandType.Text,
        bool isPrepared = false,
        int commandTimeout = 30,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Executes the command and returns the first column of the first row in the first returned result set. All other columns, rows and result sets are ignored.
    /// </summary>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="dbParameters">The collection of DbParameter objects</param>
    /// <param name="commandType">Determines how the command text is interpreted.</param>
    /// <param name="isPrepared">Indicates if the command should be prepared.</param>
    /// <param name="commandTimeout">The wait time (in seconds) before terminating the attempt to execute the command and generating an error.</param>
    /// <returns>The first column of the first row in the first result set.</returns>
    object? ExecuteScalar(
        string commandText,
        IEnumerable<DbParameter>? dbParameters = default,
        CommandType commandType = CommandType.Text,
        bool isPrepared = false,
        int commandTimeout = 30);


    /// <summary>
    /// Implements the asynchronous version of ExecuteScalar(), but returns a Task synchronously, blocking the calling thread.
    /// </summary>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    Task<object?> ExecuteScalarAsync(string commandText, CancellationToken cancellationToken = default);


    /// <summary>
    /// Implements the asynchronous version of ExecuteScalar(), but returns a Task synchronously, blocking the calling thread.
    /// </summary>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="dbParameters">The collection of DbParameter objects</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    Task<object?> ExecuteScalarAsync(
        string commandText,
        IEnumerable<DbParameter>? dbParameters = null,
        CancellationToken cancellationToken = default);


    /// <summary>
    /// Implements the asynchronous version of ExecuteScalar(), but returns a Task synchronously, blocking the calling thread.
    /// </summary>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="dbParameters">The collection of DbParameter objects</param>
    /// <param name="commandType">Determines how the command text is interpreted.</param>
    /// <param name="isPrepared">Indicates if the command should be prepared.</param>
    /// <param name="commandTimeout">The wait time (in seconds) before terminating the attempt to execute the command and generating an error.</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    Task<object?> ExecuteScalarAsync(
        string commandText,
        IEnumerable<DbParameter>? dbParameters = default,
        CommandType commandType = CommandType.Text,
        bool isPrepared = false,
        int commandTimeout = 30,
        CancellationToken cancellationToken = default);


    ExecuteInsertResult ExecuteTextInsert<T>(T item, string tableName, bool isPrepared = false, int commandTimeout = 30);


    ExecuteInsertResult ExecuteTextInsert(string tableName, List<DbParameter> parameters, bool isPrepared = false, int commandTimeout = 30);


    ExecuteInsertResult ExecuteInsert(string commandText, IEnumerable<DbParameter>? dbParameters = default, CommandType commandType = CommandType.Text, bool isPrepared = false, int commandTimeout = 30);


    /// <summary>
    /// Rolls back the transaction from a pending state.
    /// </summary>
    void RollbackTransaction();


    /// <summary>
    /// Asynchronously rolls back a transaction from a pending state.
    /// </summary>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was cancelled. This exception is stored into the returned task.</exception>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);


    /// <summary>
    /// Executes the select command text against its connection, returning the results as Type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of value returned.</typeparam>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="dbParameters">The collection of DbParameter objects</param>
    /// <param name="commandType">Determines how the command text is interpreted.</param>
    /// <param name="isPrepared">Indicates if the command should be prepared.</param>
    /// <param name="commandTimeout">The wait time (in seconds) before terminating the attempt to execute the command and generating an error.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> of the results.</returns>
    IEnumerable<T> Select<T>(string commandText, IEnumerable<DbParameter>? dbParameters = default, CommandType commandType = CommandType.Text, bool isPrepared = false, int commandTimeout = 30) where T : new();


    /// <summary>
    /// An asynchronous version of <see cref="Select{T}(string, IEnumerable{DbParameter}?, CommandType, bool, int)"/> , which executes the select command against its connection, returning the results as Type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of value returned.</typeparam>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was cancelled. This exception is stored into the returned task.</exception>
    Task<IEnumerable<T>> SelectAsync<T>(string commandText, CancellationToken cancellationToken = default) where T : new();


    /// <summary>
    /// An asynchronous version of <see cref="Select{T}(string, IEnumerable{DbParameter}?, CommandType, bool, int)"/> , which executes the select command against its connection, returning the results as Type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of value returned.</typeparam>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="dbParameters">The collection of DbParameter objects</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was cancelled. This exception is stored into the returned task.</exception>
    Task<IEnumerable<T>> SelectAsync<T>(string commandText, IEnumerable<DbParameter> dbParameters, CancellationToken cancellationToken = default) where T : new();


    /// <summary>
    /// An asynchronous version of <see cref="Select{T}(string, IEnumerable{DbParameter}?, CommandType, bool, int)"/> , which executes the select command against its connection, returning the results as Type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of value returned.</typeparam>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="dbParameters">The collection of DbParameter objects</param>
    /// <param name="commandType">Determines how the command text is interpreted.</param>
    /// <param name="isPrepared">Indicates if the command should be prepared.</param>
    /// <param name="commandTimeout">The wait time (in seconds) before terminating the attempt to execute the command and generating an error.</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was cancelled. This exception is stored into the returned task.</exception>
    Task<IEnumerable<T>> SelectAsync<T>(string commandText, IEnumerable<DbParameter>? dbParameters = default, CommandType commandType = CommandType.Text, bool isPrepared = false, int commandTimeout = 30, CancellationToken cancellationToken = default) where T : new();


    /// <summary>
    /// Executes the select command text against its connection, returning the first element of the results as Type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of value returned.</typeparam>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="dbParameters">The collection of DbParameter objects</param>
    /// <param name="commandType">Determines how the command text is interpreted.</param>
    /// <param name="isPrepared">Indicates if the command should be prepared.</param>
    /// <param name="commandTimeout">The wait time (in seconds) before terminating the attempt to execute the command and generating an error.</param>
    /// <returns>The <typeparamref name="T"/> record found.</returns>
    T SelectFirst<T>(string commandText, IEnumerable<DbParameter>? dbParameters = default, CommandType commandType = CommandType.Text, bool isPrepared = false, int commandTimeout = 30) where T : new();


    /// <summary>
    /// An asynchronous version of <see cref="SelectFirst{T}(string, IEnumerable{DbParameter}?, CommandType, bool, int)"/> , which executes the select command text against its connection, returning the first element of the results as Type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of value returned.</typeparam>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was cancelled. This exception is stored into the returned task.</exception>
    Task<T> SelectFirstAsync<T>(string commandText, CancellationToken cancellationToken = default) where T : new();


    /// <summary>
    /// An asynchronous version of <see cref="SelectFirst{T}(string, IEnumerable{DbParameter}?, CommandType, bool, int)"/> , which executes the select command text against its connection, returning the first element of the results as Type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of value returned.</typeparam>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="dbParameters">The collection of DbParameter objects</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was cancelled. This exception is stored into the returned task.</exception>
    Task<T> SelectFirstAsync<T>(string commandText, IEnumerable<DbParameter> dbParameters, CancellationToken cancellationToken = default) where T : new();


    /// <summary>
    /// An asynchronous version of <see cref="SelectFirst{T}(string, IEnumerable{DbParameter}?, CommandType, bool, int)"/> , which executes the select command text against its connection, returning the first element of the results as Type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of value returned.</typeparam>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="dbParameters">The collection of DbParameter objects</param>
    /// <param name="commandType">Determines how the command text is interpreted.</param>
    /// <param name="isPrepared">Indicates if the command should be prepared.</param>
    /// <param name="commandTimeout">The wait time (in seconds) before terminating the attempt to execute the command and generating an error.</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was cancelled. This exception is stored into the returned task.</exception>
    Task<T> SelectFirstAsync<T>(string commandText, IEnumerable<DbParameter>? dbParameters = default, CommandType commandType = CommandType.Text, bool isPrepared = false, int commandTimeout = 30, CancellationToken cancellationToken = default) where T : new();


    /// <summary>
    /// Executes the select command text against its connection, returning the first element of the results as Type <typeparamref name="T"/> or a default value if no element is found.
    /// </summary>
    /// <typeparam name="T">The type of value returned.</typeparam>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="dbParameters">The collection of DbParameter objects</param>
    /// <param name="commandType">Determines how the command text is interpreted.</param>
    /// <param name="isPrepared">Indicates if the command should be prepared.</param>
    /// <param name="commandTimeout">The wait time (in seconds) before terminating the attempt to execute the command and generating an error.</param>
    /// <returns></returns>
    T? SelectFirstOrDefault<T>(string commandText, IEnumerable<DbParameter>? dbParameters = default, CommandType commandType = CommandType.Text, bool isPrepared = false, int commandTimeout = 30) where T : new();


    /// <summary>
    /// An asynchronous version of <see cref="SelectFirstOrDefault{T}(string, IEnumerable{DbParameter}?, CommandType, bool, int)"/> , which executes the select command text against its connection, returning the first element of the results as Type <typeparamref name="T"/> or a default value if no element is found.
    /// </summary>
    /// <typeparam name="T">The type of value returned.</typeparam>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was cancelled. This exception is stored into the returned task.</exception>
    Task<T?> SelectFirstOrDefaultAsync<T>(string commandText, CancellationToken cancellationToken = default) where T : new();


    /// <summary>
    /// An asynchronous version of <see cref="SelectFirstOrDefault{T}(string, IEnumerable{DbParameter}?, CommandType, bool, int)"/> , which executes the select command text against its connection, returning the first element of the results as Type <typeparamref name="T"/> or a default value if no element is found.
    /// </summary>
    /// <typeparam name="T">The type of value returned.</typeparam>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="dbParameters">The collection of DbParameter objects</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was cancelled. This exception is stored into the returned task.</exception>
    Task<T?> SelectFirstOrDefaultAsync<T>(string commandText, IEnumerable<DbParameter>? dbParameters = default, CancellationToken cancellationToken = default) where T : new();


    /// <summary>
    /// An asynchronous version of <see cref="SelectFirstOrDefault{T}(string, IEnumerable{DbParameter}?, CommandType, bool, int)"/> , which executes the select command text against its connection, returning the first element of the results as Type <typeparamref name="T"/> or a default value if no element is found.
    /// </summary>
    /// <typeparam name="T">The type of value returned.</typeparam>
    /// <param name="commandText">The text command to run against the data source.</param>
    /// <param name="dbParameters">The collection of DbParameter objects</param>
    /// <param name="commandType">Determines how the command text is interpreted.</param>
    /// <param name="isPrepared">Indicates if the command should be prepared.</param>
    /// <param name="commandTimeout">The wait time (in seconds) before terminating the attempt to execute the command and generating an error.</param>
    /// <param name="cancellationToken">An optional token to cancel the asynchronous operation. The default value is <see cref="CancellationToken.None"/></param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was cancelled. This exception is stored into the returned task.</exception>
    Task<T?> SelectFirstOrDefaultAsync<T>(string commandText, IEnumerable<DbParameter>? dbParameters = default, CommandType commandType = CommandType.Text, bool isPrepared = false, int commandTimeout = 30, CancellationToken cancellationToken = default) where T : new();
}


using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

using Simple.Data.Common.Options;

namespace Simple.Data.Abstractions;
public interface IDbConnectionService : IUnitOfWork
{
    DbConnectionOptions DbConnectionOptions { get; }

    Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    void Close();
    Task CloseAsync();
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    DbBatch CreateBatch();
    DbCommand CreateCommand(string sql, DbTransaction? dbTransaction = default, IEnumerable<DbParameter>? dbParameters = default, CommandType commandType = CommandType.Text, bool isPrepared = false, int commandTimeout = 30);
    Task<DbCommand> CreateCommandAsync(string sql, DbTransaction? dbTransaction = default, IEnumerable<DbParameter>? dbParameters = default, CommandType commandType = CommandType.Text, bool isPrepared = false, int commandTimeout = 30);
    DbParameter CreateInputParameter(string parameterName, object? value, DbType dbType, bool isNullable = true);
    DbParameter CreateOutputParameter(string parameterName, DbType dbType, bool isNullable = true);
    DbParameter CreateParameter();
    DbParameter CreateParameter(string parameterName, object? value, DbType dbType, ParameterDirection parameterDirection, bool isNullable = true);
    DbConnection GetConnection();
    void Open();
    Task OpenAsync();
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

using Npgsql;
using PgOutbox.Contracts;

namespace PgOutbox.Interfaces;

public interface IOutboxWriter<in T> where T : new()
{
    Task<InsertResult> Insert(T entity, CancellationToken ct);
    Task<InsertResult> Insert(T entity, NpgsqlConnection connection, NpgsqlTransaction transaction);
}
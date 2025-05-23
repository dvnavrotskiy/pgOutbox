using Npgsql;
using PgOutbox.Contracts;
using PgOutbox.Interfaces;

namespace PgOutbox.Services;

internal class OutboxWriter<T> : IOutboxWriter<T> where T : new()
{
    private readonly OutboxRepo<T> repo;

    internal OutboxWriter(OutboxRepo<T> repo)
    {
        this.repo = repo;
    }
    
    public async Task<InsertResult> Insert(T entity, CancellationToken ct)
        => await repo.Insert(entity, ct);
    
    public async Task<InsertResult> Insert(T entity, NpgsqlConnection connection, NpgsqlTransaction transaction)
        => await repo.Insert(entity, connection, transaction);
}
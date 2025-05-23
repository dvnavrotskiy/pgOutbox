using PgOutbox.Contracts;
using PgOutbox.Interfaces;

namespace PgOutbox.Services;

internal class OutboxProcessor<T> : IOutboxProcessor<T> where T : new()
{
    private readonly OutboxRepo<T> repo;

    internal OutboxProcessor(OutboxRepo<T> repo)
    {
        this.repo = repo;
    }

    public async Task<OccupyResult<T>?> Occupy(int id, CancellationToken ct)
        => await repo.Occupy(id, ct);

    public async Task<List<OccupyResult<T>>> OccupyBatch(CancellationToken ct)
        => await repo.OccupyBatch(ct);
    
    public async Task Release(IEnumerable<int> ids, CancellationToken ct)
        => await repo.Release(ids, ct);
}
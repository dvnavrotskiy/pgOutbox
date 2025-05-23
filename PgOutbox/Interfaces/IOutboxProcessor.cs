using PgOutbox.Contracts;

namespace PgOutbox.Interfaces;

public interface IOutboxProcessor<T> where T : new()
{
    Task<OccupyResult<T>?> Occupy(int id, CancellationToken ct);
    Task<List<OccupyResult<T>>> OccupyBatch(CancellationToken ct);
    Task Release(IEnumerable<int> ids, CancellationToken ct);
}
namespace PgOutbox.Contracts;

public sealed record OccupyResult<T> where T: new()
{
    public int Id { get; init; }
    public DateTimeOffset Created { get; init; }
    public T Entity { get; init; }
}
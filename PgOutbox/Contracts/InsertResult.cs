namespace PgOutbox.Contracts;

public sealed record InsertResult
{
    public int Id { get; init; }
    public DateTimeOffset Created { get; init; }
}
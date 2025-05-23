namespace PgOutbox.Contracts;

internal sealed record OccupyResultDto
{
    public int Id { get; init; }
    public DateTimeOffset Created { get; init; }
    public string Data { get; init; }
}
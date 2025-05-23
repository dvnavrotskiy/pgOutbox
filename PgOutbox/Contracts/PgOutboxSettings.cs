namespace PgOutbox.Contracts;

public record PgOutboxSettings
{
    public string ConnectionString { get; init; }
    public int BatchSize { get; init; }
    public int OccupationTimeSeconds { get; init; }
    public bool UseHistoryTable { get; init; }
}
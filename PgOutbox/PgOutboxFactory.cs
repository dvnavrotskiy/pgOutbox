using PgOutbox.Contracts;
using PgOutbox.Interfaces;
using PgOutbox.Services;

namespace PgOutbox;

public static class PgOutboxFactory<T> where T : new()
{
    public static async Task<OutboxFactoryContainer<T>> Create(
        PgOutboxSettings settings
    )
    {
        var repo = new OutboxRepo<T>(settings);
        var writer = new OutboxWriter<T>(repo);
        var processor = new OutboxProcessor<T>(repo);

        await repo.InitMigration();

        return new OutboxFactoryContainer<T>
        {
            Writer = writer,
            Processor = processor
        };
    }
}

public record OutboxFactoryContainer<T> where T : new()
{
    public IOutboxWriter<T> Writer { get; init; }
    public IOutboxProcessor<T> Processor { get; init; } 
}
using PgOutbox.Contracts;
using Xunit;

namespace PgOutbox.Tests;

public class WriteReadRelease
{
    [Fact]
    public async Task SimpleTest()
    {
        var settings = new PgOutboxSettings
        {
            BatchSize = 10,
            OccupationTimeSeconds = 60,
            UseHistoryTable = true,
            ConnectionString = "User ID=postgres;Password=admin;Host=localhost;Port=5432;Database=myData;Pooling=true;"
        };
        
        var container = await PgOutboxFactory<MessageRecord>.Create(settings);

        var msg = new MessageRecord
        {
            Message = "test",
            Number = 10
        };

        var insertResult = await container.Writer.Insert(msg, CancellationToken.None);
        var item = await container.Processor.Occupy(insertResult.Id, CancellationToken.None);
        
        Assert.NotNull(item);
        Assert.Equal(item.Id, insertResult.Id);
        Assert.Equal(item.Created, insertResult.Created);
        Assert.Equal(item.Entity, msg);
        
        await container.Processor.Release([insertResult.Id], CancellationToken.None);
        item = await container.Processor.Occupy(insertResult.Id, CancellationToken.None);
        
        Assert.Null(item);
    }
    
    [Fact]
    public async Task BatchTest()
    {
        var settings = new PgOutboxSettings
        {
            BatchSize = 10,
            OccupationTimeSeconds = 60,
            UseHistoryTable = false,
            ConnectionString = "User ID=postgres;Password=admin;Host=localhost;Port=5432;Database=myData;Pooling=true;"
        };
        
        var container = await PgOutboxFactory<BatchMessageRecord>.Create(settings);

        var messages = new List<(BatchMessageRecord rec, InsertResult res)>(10);
        
        for (var i = 1; i < 11; ++i)
        {
            var msg = new BatchMessageRecord
            {
                Message = $"test {i} ",
                Number = i
            };

            var insertResult = await container.Writer.Insert(msg, CancellationToken.None);
            
            messages.Add((msg, insertResult));
        }

        var items = await container.Processor.OccupyBatch(CancellationToken.None);
        Assert.NotNull(items);

        foreach (var item in items)
        {
            var pair = messages.FirstOrDefault(x => x.res.Id == item.Id);
            var insertResult = pair.res;
            Assert.NotNull(insertResult);
            Assert.Equal(item.Id, insertResult.Id);
            Assert.Equal(item.Created, insertResult.Created);
            Assert.Equal(item.Entity, pair.rec);         
        }
        
        await container.Processor.Release(items.Select(i => i.Id), CancellationToken.None);
        items = await container.Processor.OccupyBatch(CancellationToken.None);
        
        Assert.False(items.Any());
    }
}

public record MessageRecord
{
    public string Message { get; init; }
    public int Number { get; init; }
}

public record BatchMessageRecord
{
    public string Message { get; init; }
    public int Number { get; init; }
}

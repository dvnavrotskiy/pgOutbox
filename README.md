# pgOutbox
Библиотека для упрощения реализации паттерна Outbox на PostgreSQL

## Пошаговое руководство

1. Подлкючаем билиотеку к проекту.  
   
2. Создаем объект класса PgOutboxSettings:  

```
var settings = new PgOutboxSettings
{
    BatchSize = 10,
    OccupationTimeSeconds = 60,
    UseHistoryTable = false,
    ConnectionString = "User ID=postgres;Password=password;Host=posgreshost;Port=5432;Database=myData;Pooling=true;"
};
```

BatchSize - количество записей, которые будут оккупироваться при чтении,  
OccupationTimeSeconds - время оккупации записей,  
UseHistoryTable - true, если после обработки записей их нужно сложить в отдельную таблицу, а не удалить,  
ConnectionString - строка подключения к Postgre. **У пользователя PostgreSQL должны быть права на создание таблиц.**  

3. Вызываем фабрику Outbox  

```
var container = await PgOutboxFactory<YourMessageType>.Create(settings);
```

В контейнере вернутся два интерфейса  
- IOutboxWriter - для внесения записи в таблицу  
- IOutboxProcessor - для обработки очереди
Их можно добавить DI-конетейнер и получать в нужных сервисах.  

4. В транзакции записи бизнес-данных пишем событие в Outbox:  

```
var insertResult = await container.Writer.Insert(msg, connection, transaction, CancellationToken.None);
```

Впрочем, можно и независимо от транзакции создать событие:  

```
var insertResult = await container.Writer.Insert(msg, CancellationToken.None);
```

Возвращаеммый объект класса InsertResult содержит Id и время сохранения события в Outbox.

5. Обрабатываем записи, оккупируя их либо по одной:

```
var item = await container.Processor.Occupy(insertResult.Id, CancellationToken.None);

// Process your item

await container.Processor.Release([insertResult.Id], CancellationToken.None);
```

либо пакетом:  

```
var items = await container.Processor.OccupyBatch(CancellationToken.None);

foreach (var item in items)
{
  // Process your item
}

await container.Processor.Release(items.Select(i => i.Id), CancellationToken.None);
```

Удобно создать класс-наследник BackgroundService, зарегистрировать его как HostedService и реализовать цикл обработки пакетов в методе Execute

Подробнее о паттерне Outbox: https://t.me/tsifrovoy_zodchiy/27

namespace PgOutbox.Services;

internal static class TableHelper
{
    public static string GetOutboxName<T>()
        => typeof(T).FullName.Replace(".", "_") + "_Outbox";
    
    public static string GetHistoryName<T>()
        => typeof(T).FullName.Replace(".", "_") + "_History";
}
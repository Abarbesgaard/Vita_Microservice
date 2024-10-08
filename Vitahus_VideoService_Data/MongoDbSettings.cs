namespace Vitahus_VideoService_Data;

public class MongoDbSettings
{
    public string? Host { get; set; }
    /// <summary>
    /// The port of the database.
    /// </summary>
    public int Port { get; set; }
    /// <summary>
    /// The name of the database.
    /// </summary>
    public string? DatabaseName { get; set; }
    /// <summary>
    /// The connection string for the database.
    /// </summary>
    public string ConnectionString => $"mongodb://{Host}:{Port}";
}
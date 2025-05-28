namespace Metrics;

public interface IMigrationMetadata
{
    string TableName { get; }
    void SetDatabaseName(string databaseName);
    IEnumerable<string> GetScripts();
}

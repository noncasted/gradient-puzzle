namespace Metrics;

public abstract class MigrationBase {

    private string? _databaseName;

    protected string DatabaseName => !string.IsNullOrEmpty(_databaseName) ? _databaseName :
        throw new ArgumentNullException(nameof(DatabaseName));

    public void SetDatabaseName(string databaseName) {
        _databaseName = databaseName;
    }
}
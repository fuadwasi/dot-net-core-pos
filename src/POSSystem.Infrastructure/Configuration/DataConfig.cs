namespace POSSystem.Infrastructure.Configuration;

/// <summary>
/// Represents data configuration parameters
/// </summary>
public partial class DataConfig
{
    /// <summary>
    /// Gets or sets a connection string
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a data provider type
    /// </summary>
    public string DataProvider { get; set; } = "SQLite";

    /// <summary>
    /// Gets or sets the wait time (in seconds) before terminating the attempt to execute a command and generating an error.
    /// By default, timeout isn't set and a default value for the current provider used.
    /// Set 0 to use infinite timeout.
    /// </summary>
    public int? SQLCommandTimeout { get; set; } = null;
}

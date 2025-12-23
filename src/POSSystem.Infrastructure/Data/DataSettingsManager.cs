using POSSystem.Infrastructure.Configuration;

namespace POSSystem.Infrastructure.Data;

/// <summary>
/// Represents the data settings manager
/// </summary>
public partial class DataSettingsManager
{
    private static DataConfig? _dataConfig;

    /// <summary>
    /// Load data settings from configuration
    /// </summary>
    /// <param name="dataConfig">Data configuration</param>
    public static void LoadSettings(DataConfig dataConfig)
    {
        _dataConfig = dataConfig;
    }

    /// <summary>
    /// Gets the current data settings
    /// </summary>
    /// <returns>Data settings</returns>
    public static DataConfig GetSettings()
    {
        if (_dataConfig == null)
            throw new InvalidOperationException("Data settings have not been loaded. Call LoadSettings first.");

        return _dataConfig;
    }

    /// <summary>
    /// Gets a value indicating whether database is configured
    /// </summary>
    public static bool IsDatabaseConfigured()
    {
        return _dataConfig != null && !string.IsNullOrEmpty(_dataConfig.ConnectionString);
    }

    /// <summary>
    /// Gets the command execution timeout
    /// </summary>
    /// <returns>Number of seconds. Negative timeout value means that a default timeout will be used. 0 timeout value corresponds to infinite timeout.</returns>
    public static int GetSqlCommandTimeout()
    {
        return _dataConfig?.SQLCommandTimeout ?? -1;
    }
}

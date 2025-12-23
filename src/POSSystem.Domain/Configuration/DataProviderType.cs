namespace POSSystem.Domain.Configuration;

/// <summary>
/// Represents data provider type enumeration
/// </summary>
public enum DataProviderType
{
    /// <summary>
    /// Unknown provider
    /// </summary>
    Unknown,

    /// <summary>
    /// SQL Server
    /// </summary>
    SqlServer,

    /// <summary>
    /// SQLite
    /// </summary>
    SQLite,

    /// <summary>
    /// PostgreSQL
    /// </summary>
    PostgreSQL
}

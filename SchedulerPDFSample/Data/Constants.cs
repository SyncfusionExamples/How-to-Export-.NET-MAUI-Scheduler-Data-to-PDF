namespace SchedulerPDFSample
{
    /// <summary>
    /// Application-wide constants for database file naming and connection string.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// SQLite database file name.
        /// </summary>
        public const string DatabaseFilename = "AppSQLite.db3";

        /// <summary>
        /// Full SQLite connection string using the app's local storage path.
        /// </summary>
        public static string DatabasePath =>
            $"Data Source={Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename)}";
    }
}
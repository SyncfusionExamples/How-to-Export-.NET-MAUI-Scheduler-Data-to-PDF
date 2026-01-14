using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using SchedulerPDFSample;

namespace SchedulerPDFSample
{
    /// <summary>
    /// Repository class for managing tasks in the database.
    /// </summary>
    public class TaskRepository
    {
        private bool _hasBeenInitialized = false;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        public TaskRepository(ILogger<TaskRepository> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Initializes the database connection and creates the Task table if it does not exist.
        /// </summary>
        private async Task Init()
        {
            if (_hasBeenInitialized)
                return;

            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            try
            {
                var createTableCmd = connection.CreateCommand();
                createTableCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Task (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                IsCompleted INTEGER NOT NULL,
                ProjectID INTEGER NOT NULL,
                StartTime TEXT NULL,
                EndTime TEXT NULL,
                Category TEXT NULL
            );";
                await createTableCmd.ExecuteNonQueryAsync();

                // Lightweight migration: ensure new columns exist for older DBs
                var pragmaCmd = connection.CreateCommand();
                pragmaCmd.CommandText = "PRAGMA table_info(Task)";
                var existing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                await using (var reader = await pragmaCmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        existing.Add(reader.GetString(1)); // name column
                    }
                }

                async Task EnsureColumnAsync(string name, string ddl)
                {
                    if (!existing.Contains(name))
                    {
                        var alter = connection.CreateCommand();
                        alter.CommandText = $"ALTER TABLE Task ADD COLUMN {ddl}";
                        await alter.ExecuteNonQueryAsync();
                    }
                }

                await EnsureColumnAsync("StartTime", "StartTime TEXT NULL");
                await EnsureColumnAsync("EndTime", "EndTime TEXT NULL");
                await EnsureColumnAsync("Category", "Category TEXT NULL");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating/migrating Task table");
                throw;
            }

            _hasBeenInitialized = true;
        }

        /// <summary>
        /// Retrieves a list of all tasks from the database.
        /// </summary>
        /// <returns>A list of <see cref="ProjectTask"/> objects.</returns>
        public async Task<List<ProjectTask>> ListAsync()
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT ID, Title, IsCompleted, ProjectID, StartTime, EndTime, Category FROM Task";
            var tasks = new List<ProjectTask>();

            await using var reader = await selectCmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                tasks.Add(new ProjectTask
                {
                    ID = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    IsCompleted = reader.GetBoolean(2),
                    ProjectID = reader.GetInt32(3),
                    StartTime = reader.IsDBNull(4) ? default : DateTime.Parse(reader.GetString(4)),
                    EndTime = reader.IsDBNull(5) ? default : DateTime.Parse(reader.GetString(5)),
                    Category = reader.IsDBNull(6) ? string.Empty : reader.GetString(6)
                });
            }

            return tasks;
        }

        /// <summary>
        /// Retrieves a list of tasks associated with a specific project.
        /// </summary>
        /// <param name="projectId">The ID of the project.</param>
        /// <returns>A list of <see cref="ProjectTask"/> objects.</returns>
        public async Task<List<ProjectTask>> ListAsync(int projectId)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT ID, Title, IsCompleted, ProjectID, StartTime, EndTime, Category FROM Task WHERE ProjectID = @projectId";
            selectCmd.Parameters.AddWithValue("@projectId", projectId);
            var tasks = new List<ProjectTask>();

            await using var reader = await selectCmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                tasks.Add(new ProjectTask
                {
                    ID = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    IsCompleted = reader.GetBoolean(2),
                    ProjectID = reader.GetInt32(3),
                    StartTime = reader.IsDBNull(4) ? default : DateTime.Parse(reader.GetString(4)),
                    EndTime = reader.IsDBNull(5) ? default : DateTime.Parse(reader.GetString(5)),
                    Category = reader.IsDBNull(6) ? string.Empty : reader.GetString(6)
                });
            }

            return tasks;
        }

        /// <summary>
        /// Retrieves a specific task by its ID.
        /// </summary>
        /// <param name="id">The ID of the task.</param>
        /// <returns>A <see cref="ProjectTask"/> object if found; otherwise, null.</returns>
        public async Task<ProjectTask?> GetAsync(int id)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var selectCmd = connection.CreateCommand();
            selectCmd.CommandText = "SELECT ID, Title, IsCompleted, ProjectID, StartTime, EndTime, Category FROM Task WHERE ID = @id";
            selectCmd.Parameters.AddWithValue("@id", id);

            await using var reader = await selectCmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new ProjectTask
                {
                    ID = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    IsCompleted = reader.GetBoolean(2),
                    ProjectID = reader.GetInt32(3),
                    StartTime = reader.IsDBNull(4) ? default : DateTime.Parse(reader.GetString(4)),
                    EndTime = reader.IsDBNull(5) ? default : DateTime.Parse(reader.GetString(5)),
                    Category = reader.IsDBNull(6) ? string.Empty : reader.GetString(6)
                };
            }

            return null;
        }

        /// <summary>
        /// Saves a task to the database. If the task ID is 0, a new task is created; otherwise, the existing task is updated.
        /// </summary>
        /// <param name="item">The task to save.</param>
        /// <returns>The ID of the saved task.</returns>
        public async Task<int> SaveItemAsync(ProjectTask item)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var saveCmd = connection.CreateCommand();
            if (item.ID == 0)
            {
                saveCmd.CommandText = @"
            INSERT INTO Task (Title, IsCompleted, ProjectID, StartTime, EndTime, Category)
            VALUES (@title, @isCompleted, @projectId, @startTime, @endTime, @category);
            SELECT last_insert_rowid();";
            }
            else
            {
                saveCmd.CommandText = @"
            UPDATE Task
            SET Title = @title,
                IsCompleted = @isCompleted,
                ProjectID = @projectId,
                StartTime = @startTime,
                EndTime = @endTime,
                Category = @category
            WHERE ID = @id";
                saveCmd.Parameters.AddWithValue("@id", item.ID);
            }

            saveCmd.Parameters.AddWithValue("@title", item.Title);
            saveCmd.Parameters.AddWithValue("@isCompleted", item.IsCompleted);
            saveCmd.Parameters.AddWithValue("@projectId", item.ProjectID);
            saveCmd.Parameters.AddWithValue("@startTime", item.StartTime == default ? (object)DBNull.Value : item.StartTime.ToString("o"));
            saveCmd.Parameters.AddWithValue("@endTime", item.EndTime == default ? (object)DBNull.Value : item.EndTime.ToString("o"));
            saveCmd.Parameters.AddWithValue("@category", string.IsNullOrWhiteSpace(item.Category) ? (object)DBNull.Value : item.Category);

            var result = await saveCmd.ExecuteScalarAsync();
            if (item.ID == 0)
            {
                item.ID = Convert.ToInt32(result);
            }

            return item.ID;
        }

        /// <summary>
        /// Deletes a task from the database.
        /// </summary>
        /// <param name="item">The task to delete.</param>
        /// <returns>The number of rows affected.</returns>
        public async Task<int> DeleteItemAsync(ProjectTask item)
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var deleteCmd = connection.CreateCommand();
            deleteCmd.CommandText = "DELETE FROM Task WHERE ID = @id";
            deleteCmd.Parameters.AddWithValue("@id", item.ID);

            return await deleteCmd.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Drops the Task table from the database.
        /// </summary>
        public async Task DropTableAsync()
        {
            await Init();
            await using var connection = new SqliteConnection(Constants.DatabasePath);
            await connection.OpenAsync();

            var dropTableCmd = connection.CreateCommand();
            dropTableCmd.CommandText = "DROP TABLE IF EXISTS Task";
            await dropTableCmd.ExecuteNonQueryAsync();
            _hasBeenInitialized = false;
        }
    }
}
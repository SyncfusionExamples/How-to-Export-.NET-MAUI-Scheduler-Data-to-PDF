using System.Text.Json.Serialization;

namespace SchedulerPDFSample
{
    /// <summary>
    /// Represents a single task belonging to a project.
    /// </summary>
    public class ProjectTask
    {
        #region Properties
        public int ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }

        // Start and end times for scheduling
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // Category for color-coding in scheduler (e.g., Work, Personal)
        public string? Category { get; set; }

        /// <summary>
        /// Foreign key for the owning project.
        /// </summary>
        [JsonIgnore]
        public int ProjectID { get; set; }
        #endregion
    }
}

using System.Text.Json.Serialization;

namespace SchedulerPDFSample
{
    /// <summary>
    /// Represents a project, including its tasks, category, and tags.
    /// </summary>
    public class Project
    {
        #region Properties
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;

        /// <summary>
        /// Foreign key for the assigned category.
        /// </summary>
        [JsonIgnore]
        public int CategoryID { get; set; }

        public Category? Category { get; set; }

        public List<ProjectTask> Tasks { get; set; } = [];

        public List<Tag> Tags { get; set; } = [];

        /// <summary>
        /// Short description for accessibility readers.
        /// </summary>
        public string AccessibilityDescription => $"{Name} Project. {Description}";
        #endregion

        public override string ToString() => $"{Name}";
    }

    /// <summary>
    /// JSON wrapper for a list of projects.
    /// </summary>
    public class ProjectsJson
    {
        public List<Project> Projects { get; set; } = [];
    }
}


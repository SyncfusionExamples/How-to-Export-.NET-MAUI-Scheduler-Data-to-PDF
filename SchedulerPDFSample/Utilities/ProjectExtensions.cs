using System.Diagnostics.CodeAnalysis;

namespace SchedulerPDFSample
{
    /// <summary>
    /// Project Model Extensions
    /// </summary>
    public static class ProjectExtensions
    {
        /// <summary>
        /// Returns true when the project instance is null or has not been persisted (ID == 0).
        /// </summary>
        /// <param name="project">Project instance to check.</param>
        /// <returns>True if null or new; otherwise false.</returns>
        public static bool IsNullOrNew([NotNullWhen(false)] this Project? project)
        {
            return project is null || project.ID == 0;
        }
    }
}

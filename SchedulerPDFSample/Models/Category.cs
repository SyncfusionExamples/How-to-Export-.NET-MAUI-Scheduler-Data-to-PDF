using System.Text.Json.Serialization;

namespace SchedulerPDFSample
{
    /// <summary>
    /// Represents a category used to organize projects.
    /// </summary>
    public class Category
    {
        #region Properties
        public int ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Color { get; set; } = "#FF0000";

        /// <summary>
        /// Brush generated from the hex color value for UI bindings.
        /// </summary>
        [JsonIgnore]
        public Brush ColorBrush
        {
            get => new SolidColorBrush(Microsoft.Maui.Graphics.Color.FromArgb(Color));
        }
        #endregion

        public override string ToString() => $"{Title}";
    }
}


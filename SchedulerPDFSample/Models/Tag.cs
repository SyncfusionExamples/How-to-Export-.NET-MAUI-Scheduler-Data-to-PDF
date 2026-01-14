using System.Text.Json.Serialization;
using CommunityToolkit.Maui.Core.Extensions;
using Color = Microsoft.Maui.Graphics.Color;

namespace SchedulerPDFSample
{
    /// <summary>
    /// Represents a tag that can be assigned to projects.
    /// </summary>
    public class Tag
    {
        #region Properties
        public int ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Color { get; set; } = "#FF0000";

        /// <summary>
        /// Brush generated from the hex color value for UI bindings.
        /// </summary>
        [JsonIgnore]
        public Brush ColorBrush => new SolidColorBrush(Microsoft.Maui.Graphics.Color.FromArgb(Color));

        /// <summary>
        /// Actual Color object from hex string.
        /// </summary>
        [JsonIgnore]
        public Color DisplayColor => Microsoft.Maui.Graphics.Color.FromArgb(Color);

        /// <summary>
        /// Darkened variation of DisplayColor for UI accents.
        /// </summary>
        [JsonIgnore]
        public Color DisplayDarkColor => DisplayColor.WithBlackKey(0.8);

        /// <summary>
        /// Lightened variation of DisplayColor for UI accents.
        /// </summary>
        [JsonIgnore]
        public Color DisplayLightColor => DisplayColor.WithBlackKey(0.2);

        /// <summary>
        /// Gets or sets whether the tag is selected in UI.
        /// </summary>
        [JsonIgnore]
        public bool IsSelected { get; set; }
        #endregion
    }
}


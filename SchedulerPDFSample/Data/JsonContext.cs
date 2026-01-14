using System.Text.Json.Serialization;
using SchedulerPDFSample;

[JsonSerializable(typeof(Project))]
[JsonSerializable(typeof(ProjectTask))]
[JsonSerializable(typeof(ProjectsJson))]
[JsonSerializable(typeof(Category))]
[JsonSerializable(typeof(Tag))]
/// <summary>
/// Source-generation context for System.Text.Json serialization of core models.
/// </summary>
public partial class JsonContext : JsonSerializerContext
{
}

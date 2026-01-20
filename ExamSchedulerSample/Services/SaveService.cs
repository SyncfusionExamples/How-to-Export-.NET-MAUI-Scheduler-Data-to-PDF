
using System.IO;
using System.Threading.Tasks;

namespace ExamSchedulerSample.Services
{
    public static partial class SaveService
    {
        public static partial Task SaveAndViewAsync(string filename, string contentType, MemoryStream stream);
    }
}

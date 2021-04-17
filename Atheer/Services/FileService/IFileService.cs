using System.IO;
using System.Threading.Tasks;

namespace Atheer.Services.FileService
{
    public interface IFileService
    {
        Task<string> Add(FileUse fileUse, string fileId, string contentType, Stream stream);
        Task Remove(FileUse fileUse, string fileId);
    }
}
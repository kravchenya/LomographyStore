using System.IO;
using System.Threading.Tasks;

namespace LomographyStoreApi.Services.Interfaces
{
    public interface IBlobService
    {
        Task<string> UploadBlobAsync(string blobName, Stream imageStream);   
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using LomographyStoreApi.Models;

namespace LomographyStoreApi.Services.Interfaces
{
    public interface IDocumentDBService
    {
        Task<PhotoItem> AddProductAsync(PhotoItem product);

        Task<List<PhotoItem>> GetProductsAsync();

        Task<PhotoItem> GetProductAsync(string id, string camera);

        Task AddImageToProductAsync(string id, string camera, string imageUri);
    }
}
using System.Net.Http;
using System.Threading.Tasks;
using LomographyStoreWeb.Models;

namespace LomographyStoreWeb.Services
{
    public interface IHttCustomClient
    {
        Task<string> GetAllProducts();

        Task<string> GetProductById(string id, string camera);

        Task<string> GetOderHistory();

        Task<HttpResponseMessage> PlaceOder(Order order);

        Task<HttpResponseMessage> AddNewProduct(PhotoItem product);

        Task<HttpResponseMessage> AddNewImage(string id, string camera, MultipartFormDataContent dataContent);
    }
}
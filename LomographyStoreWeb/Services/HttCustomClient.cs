using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using LomographyStoreWeb.Models;

namespace LomographyStoreWeb.Services
{
    public class HttCustomClient : IHttCustomClient
    {
        private HttpClient _httpClient;
        private string _productRoute;
        private string _imageRoute;
        private string _orderRoute;
        private string _photoItemRoute;
        private string _cameraRoute;

        public HttCustomClient(HttpClient httClient, IOptions<WebApiRouteOptions> options)
        {
            _httpClient = httClient;
            _productRoute = options.Value.ProductRoute;
            _imageRoute = options.Value.ImageRoute;
            _orderRoute = options.Value.OrderRoute;
            _photoItemRoute = options.Value.PhotoItemRoute;
            _cameraRoute = options.Value.CameraRoute;
        }

        public async Task<HttpResponseMessage> AddNewImage(string id, string camera, MultipartFormDataContent dataContent)
        {
           var result = await _httpClient.PostAsync($"{_productRoute}/{_imageRoute}/{id}/{_cameraRoute}/{camera}", dataContent);
           return result;
        }

        public async Task<HttpResponseMessage> AddNewProduct(PhotoItem product)
        {
            var httpContent = CreateStringContent(product);

            var response = await _httpClient.PostAsync($"{_productRoute}/{_photoItemRoute}", httpContent);
            return response;
        }

        public async Task<string> GetOderHistory()
        {
            var orderHistory = await _httpClient.GetStringAsync($"{_orderRoute}");
            return orderHistory;
        }

        public async Task<HttpResponseMessage> PlaceOder(Order order)
        {
            var httpContent = CreateStringContent(order);
            var responce = await _httpClient.PostAsync($"{_orderRoute}", httpContent);
            return responce;
        }

        public async Task<string> GetProductById(string id, string camera)
        {
            var response = await _httpClient.GetStringAsync($"{_productRoute}/{id}/{_cameraRoute}/{camera}" );
            return response;
        }

        public async Task<string> GetAllProducts()
        {
            var responce = await _httpClient.GetStringAsync($"{_productRoute}");
            return responce;
        }

        private StringContent CreateStringContent(object objToSerialise)
        {
            var jsonInString = JsonConvert.SerializeObject(objToSerialise);
            var httpContent = new StringContent(jsonInString, Encoding.UTF8, "application/json");
            return httpContent;
        }
    }
}
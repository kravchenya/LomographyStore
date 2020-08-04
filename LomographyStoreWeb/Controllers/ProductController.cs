using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using LomographyStoreWeb.Services;

namespace LomographyStoreWeb.Controllers
{
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IHttCustomClient _client;

        public ProductController(ILogger<ProductController> logger, IHttCustomClient webApiClient)
        {
            _logger = logger;
            _client = webApiClient;        
        }

        public async Task<IActionResult> Index()    
        {
            _logger.LogInformation("Get all products is called");
            var response = await _client.GetAllProducts();
            var products = JArray.Parse(response);
            return View(products);
        }

        public async Task<IActionResult> Detail(string id, string camera)
        {
            _logger.LogInformation($"Get details to product with id {id} is called");
            var response = await _client.GetProductById(id, camera);
            var product = JObject.Parse(response);
            return View(product);
        } 
    }
}
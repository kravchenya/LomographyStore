using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using LomographyStoreWeb.Models;
using Newtonsoft.Json;
using LomographyStoreWeb.Services;
using Microsoft.Extensions.Logging;

namespace LomographyStoreWeb.Controllers
{
    public class AdminController : Controller
    {
        private IHttCustomClient _client;
        private ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger, IHttCustomClient httpCustomClient)
        {
            _client = httpCustomClient;
            _logger = logger;
        }
        //Add Product
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AddPhoto(PhotoItem newProduct)
        {
            _logger.LogInformation($"Add new photo item is called");
           
            //Call API to add new product 
            var response = await _client.AddNewProduct(newProduct);
            
            //check response for errors
            if(response.IsSuccessStatusCode)
            {
                var modelJson = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<PhotoItem>(modelJson);
                ViewData["PhotoModel"] = model;
                ViewData["NewPhotoId"] = model.Id;
                ViewData["Camera"] = model.Camera;
                ViewData["ProgressMessage"] = "New photo description added";
                return View("Index");
            }
            else
            {
                _logger.LogError("Error happened during adding new photo", response.ReasonPhrase.ToString());
                ViewData["ErrorMessage"] = "Error happened while adding photo description";
                return View("Index");
            }
        }       

        /// <summary>
        /// Add a new image and associate it with a product
        /// </summary>
        /// <param name="imageFile">The posted image file</param>
        /// <returns></returns>
        public async Task<IActionResult> NewImage(IFormFile imageFile)
        {
            _logger.LogInformation($"Add new photo is called");

            //product ID from query string
            string id = Request.Form["imageId"];
            string camera = Request.Form["camera"];
            if(imageFile.Length > 0)
            {
                //create form payload to pass to web API
                var imageContent = new StreamContent(imageFile.OpenReadStream());
                imageContent.Headers.ContentDisposition =
                    new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                    {
                        Name = "imageFile",
                        FileName = imageFile.FileName
                    };

                var postContent = new MultipartFormDataContent();
                postContent.Add(imageContent,"imageFile");

                //call web api passing multipart form data
                var result = await _client.AddNewImage(id, camera, postContent);

                if(result.IsSuccessStatusCode)
                {
                    ViewData["ProgressMessage"] = "Photo added";
                    ModelState.Clear();
                    return View("Index");
                }
                else
                {
                    _logger.LogError("Error happened adding new photo", result.ReasonPhrase.ToString());
                    ViewData["ErrorMessage"] = "Error happened while adding new image";
                    return View("Index");
                }
            }
            else
            {
                ViewData["ErrorMessage"] = "image can not be null";
                return View("Index");
            }
        }
    }
}
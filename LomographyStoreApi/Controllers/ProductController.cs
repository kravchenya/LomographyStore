using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LomographyStoreApi.Models;
using LomographyStoreApi.Services.Interfaces;

namespace LomographyStoreApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IBlobService _blobStorageService;
        private readonly IDocumentDBService _docService;

        public ProductController(ILogger<ProductController> logger, IDocumentDBService docService, IBlobService blobStorageService)
        {
            _logger = logger;
            _blobStorageService = blobStorageService;
            _docService = docService;      
        }

        /// <summary>
        /// Gets all photo items from Azure CosmosDB
        /// </summary>
        /// <returns>photo items as JSON</returns>
        // Get api/product/
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                _logger.LogInformation("GetAllProducts is called");
                var products = await _docService.GetProductsAsync();
                return new JsonResult(products);
            } 
            catch(Exception exp)
            {
                _logger.LogError("Error happened while calling GetAllProducts", exp.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Gets photo item by id and partion key from Azure CosmosDB
        /// </summary>
        /// <param name="id">Photo item to add</param>
        /// <param name="cameraName">Partition key</param>
        /// <returns>Created photo item</returns>
        // GET api/product/5/camera/kodak
        [HttpGet("{id}/camera/{cameraName}")]
        public async Task<IActionResult> GetProductById(string id, string cameraName)
        {
            try
            {
                _logger.LogInformation($"GetProductById is called with id {id} and partionkey {cameraName}");
                var product = await _docService.GetProductAsync(id, cameraName);
                return new JsonResult(product);
            }
            catch (Exception exp)
            {
                _logger.LogError("Error happened while calling GetProductById", exp.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Creates a new photo item in Azure CosmosDB
        /// </summary>
        /// <param name="product">Photo item to add</param>
        /// <returns>Created photo item</returns>
        // Post api/product/photoitem
        [HttpPost]
        [Route("photoitem")]
        public async Task<IActionResult> AddNewPhotoItem(PhotoItem product)
        {
            try
            {
                _logger.LogInformation("AddNewPhotoItem is called");
                var newProduct = await _docService.AddProductAsync(product);
                return new JsonResult(newProduct);
            } 
            catch (Exception exp)
            {
                _logger.LogError("Error happened while calling AddNewPhotoItem", exp.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        
        /// <summary>
        /// Adds an image to a photo item to Azure Blob storage and then adds a link to it to Azure CosmosDB
        /// </summary>
        /// <param name="imageFile">image to add to photo</param>
        /// <returns>Execution status code</returns>
        // Post api/product/image/1/camera/diana
        [HttpPost]
        [Route("image/{id}/camera/{camera}")]
        public async Task<IActionResult> AddProductImage(IFormFile imageFile)
        {   
            try
            {
                _logger.LogInformation("AddProductImage is called");

                string id = (string)RouteData.Values["id"];
                string camera = (string)RouteData.Values["camera"];

                if (!Request.HasFormContentType){
                    return new UnsupportedMediaTypeResult();
                }

                //BLOB Service: Get blob to write
                string blobFileRef;
                using(var imageStream = imageFile.OpenReadStream())
                {
                    blobFileRef = await _blobStorageService.UploadBlobAsync($"{id}.jpg", imageStream);
                }

                //update cosmos db with image link
                await _docService.AddImageToProductAsync(id, camera, blobFileRef);
                return Ok();
            }
            catch(Exception ex)
            {
                _logger.LogError("Error happened while calling AddProductImage", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
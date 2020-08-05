using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using LomographyStoreWeb.Services;
using LomographyStoreWeb.Models;
using System.Net;
using System;

namespace LomographyStoreWeb.Controllers
{
    public class CartController : Controller
    {
        private IHttCustomClient _client;
        private ILogger<CartController> _logger;

        public CartController(ILogger<CartController> logger, IHttCustomClient client)
        {
            _logger = logger;
            _client = client;
        }

        [HttpGet]
        public IActionResult Index()
        {
            //cart items are shown in the index page using localstorage
            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> History()
        {
            _logger.LogInformation($"Get order history is called");
            var orderHistory = await _client.GetOderHistory();
            var historyArray = JArray.Parse(orderHistory);

            return View(historyArray);
        }

         /// <summary>
        /// Handle a cart being submitted as an order
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Index(Order order)
        {
            _logger.LogInformation($"Place order is called");
            var respone = await _client.PlaceOder(order);
            if(Response.StatusCode == (int)HttpStatusCode.OK)
            {
                return Ok();
            }
            else
            {
                _logger.LogError("Error happened during placing new order", Response.StatusCode);
                throw new ApplicationException($"Order failed with status code: {Response.StatusCode}");
            }
            
        }

        /// <summary>
        /// Placholder page for completion success
        /// </summary>
        /// <returns></returns>
        public IActionResult OrderComplete()
        {
            return View();
        } 
    }
}
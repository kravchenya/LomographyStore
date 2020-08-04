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
    public class OrderController : ControllerBase
    {
        private IQueueService _qService;
        private ITableService _tService;
        private ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger, IQueueService queueService, ITableService tableService)
        {
            _qService = queueService;
            _tService = tableService;
            _logger = logger;
        }

        /// <summary>
        /// Creates an order on an Azure Queue for processing.
        /// </summary>
        /// <param name="order">the order to write</param>
        /// <returns>Execution status code</returns>
        // Post api/order
        [HttpPost]
        public async Task<IActionResult> CreateOrder(Order order)
        {
            try
            {
                _logger.LogInformation($"CreateOrder is called with {order.Items.Count} items");
                await _qService.SendMessageAsync(order);
                return Ok();
            } 
            catch(Exception exp)
            {
                _logger.LogError("Error happened, while calling CreateOrder", exp.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Retrieves order history from an Azure table.
        /// </summary>
        /// <returns>Execution status code</returns>
        // GET api/order
       [HttpGet]
       public async Task<IActionResult> GetOrderHistory()
        {
            try
            {
                _logger.LogInformation("GetOrderHistory is called");
                var items = await _tService.GetOrderHistoryAsync();
                return new JsonResult(items);
            } 
            catch (Exception exp)
            {
                _logger.LogError("Error happened while calling GetOrderHistory", exp.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
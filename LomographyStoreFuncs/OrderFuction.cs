using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using LomographyStoreFuncs.DataObjects;

namespace LomographyStoreFuncs.Function
{
    public static class OrderFunction
    {
        [FunctionName(nameof(OrderFunction))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequestMessage req,
            [Table("orderhistory")] ICollector<TableOrderItem> items,
            ILogger log)
        {
            try
            {
                log.LogInformation("OrderFunction is called");
                         
                var order = await req.Content.ReadAsAsync<Order>();

                foreach(var item in order.Items)
                {
                    var orderId = Guid.NewGuid().ToString();
                    TableOrderItem toi = new TableOrderItem(item);
                    toi.PartitionKey = "history";
                    toi.RowKey = $"{orderId} - {item.Id}";
                    items.Add(toi);
                }
            
                return new OkResult();
            }
            catch(Exception exp)
            {
                log.LogError("Error happened, while calling OrderFunction", exp.Message);
                return new BadRequestObjectResult(exp.Message);
            }
        }
    }
}

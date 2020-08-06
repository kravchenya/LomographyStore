using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LomographyStoreApi.Controllers;
using LomographyStoreApi.Models;
using LomographyStoreApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace LomographyStoreApi.Unittests
{
    [TestFixture]
    public class OrderControllerTests
    {
        private Mock<ILogger<OrderController>> _loggerMock;
        private Mock<IQueueService> _queueService;
        private Mock<ITableService> _tableService;
        private Order _order;

        [SetUp]
        public void Init()
        {
            _loggerMock = new Mock<ILogger<OrderController>>();
            _tableService = new Mock<ITableService>();
            _queueService = new Mock<IQueueService>();
   

            _order = new Order();
            var orderItem = new OrderItem {
                Id = Guid.NewGuid().ToString(),
                Camera = "Diana",
                Description = "Photo taken with Diana camera",
                Name = "My favorite photo"
            };
            _order.Items = new List<OrderItem>();
            _order.Items.Add(orderItem);
        }

        [Test]
        public async Task CreateOrder_InitializedSuccessfully_ReturnsOkay()
        {
            // Arrange

            _queueService.Setup(x => x.SendMessageAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);

            var controller = new OrderController(_loggerMock.Object, _queueService.Object, _tableService.Object);
            
            // Act
            var result = await controller.CreateOrder(_order);
            
            // Assert
            var okResult = result as OkResult;
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [Test]
        public async Task CreateOrder_InitializedNegatively_InternalServerError()
        {
            // Arrange     
            _queueService.Setup(x => x.SendMessageAsync(It.IsAny<Order>())).ThrowsAsync(new InvalidOperationException());

            var controller = new OrderController(_loggerMock.Object, _queueService.Object, _tableService.Object);

            // Act
            var result = await controller.CreateOrder(_order);
           
            // Assert
            var codeRsult = result as StatusCodeResult;
            Assert.AreEqual(500, codeRsult.StatusCode);
        }
        
        
        [Test]
        public async Task GetOrderHistory_InitializedSuccessfully_ReturnsOkay()
        {
            // Arrange
            var orderHistoryItem1 = new OrderHistoryItem{
                Camera = "Diana",
                Description = "Photo description",
                Name = "Night sky",
                Id = Guid.NewGuid().ToString()
            };

            var orderHistoryItem2 = new OrderHistoryItem{
                Camera = "Lomo",
                Description = "Photo description",
                Name = "Daily sky",
                Id = Guid.NewGuid().ToString()
            };

            var orderHistory = new List<OrderHistoryItem>();
            orderHistory.Add(orderHistoryItem1);
            orderHistory.Add(orderHistoryItem2);
            _tableService.Setup(x => x.GetOrderHistoryAsync()).ReturnsAsync(orderHistory);
            
            var controller = new OrderController(_loggerMock.Object, _queueService.Object, _tableService.Object);

            // Act
            var result = await controller.GetOrderHistory();
           
            // Assert
            var jsonResult = result as JsonResult;
            var orderHistoryList =  jsonResult.Value as List<OrderHistoryItem>;
            Assert.AreEqual(orderHistoryItem1.Id, orderHistoryList[0].Id);
            Assert.AreEqual(orderHistoryItem2.Id, orderHistoryList[1].Id);
        }

        [Test]
        public async Task GetOrderHistory_InitializedNegatively_ReturnsInternalServerError()
        {
            // Arrange                  
            _tableService.Setup(x => x.GetOrderHistoryAsync()).ThrowsAsync(new InvalidOperationException());
         
            var controller = new OrderController(_loggerMock.Object, _queueService.Object, _tableService.Object);

            // Act
            var result = await controller.GetOrderHistory();

            // Assert
            var codeRsult = result as StatusCodeResult;
            Assert.AreEqual(500, codeRsult.StatusCode);
        }
    }
}
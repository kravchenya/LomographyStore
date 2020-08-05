using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using LomographyStoreWeb.Controllers;
using LomographyStoreWeb.Models;
using LomographyStoreWeb.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace LomographyStoreWeb.Unittests
{
    [TestClass]
    public class CartControllerTests
    {
        private Mock<ILogger<CartController>> _loggerMock;
        private Mock<IHttCustomClient> _httpClientMock;
        private Mock<HttpResponse> _httpResponceMock;
        private string _id1;
        private string _id2;
        private Order _order;
        private ControllerContext _controllerContext;

        [TestInitialize]
        public void Initialize()
        {
            _loggerMock = new Mock<ILogger<CartController>>();
            _httpClientMock = new Mock<IHttCustomClient>();

            _id1 = Guid.NewGuid().ToString();
            _id2 = Guid.NewGuid().ToString();

            var historyDict1 = new Dictionary<string, string>
                {
                    {"Id", _id1},
                    {"name", "Selfie"},
                    {"camera", "Diana"},
                    {"description", "Selfie in nature"},
                };
            var historyDict2 = new Dictionary<string, string>
                {
                    {"Id", _id2},
                    {"name", "Portrait"},
                    {"camera", "Lomo"},
                    {"description", "Portrait of woman"},
                };
            
            var list = new List<Dictionary<string, string>>();
            list.Add(historyDict1);
            list.Add(historyDict2);

            var serializedString = JsonConvert.SerializeObject(list);
            _httpClientMock.Setup(x => x.GetOderHistory()).ReturnsAsync(serializedString);
            
            _order = new Order();
            _order.Items = new List<OrderItem>();
            var orderItem = new OrderItem{Id = Guid.NewGuid().ToString(), Camera = "Diana", Name = "Nature", Description = "View from my balcony"};
            _order.Items.Add(orderItem);

            var orderSerializedString = JsonConvert.SerializeObject(_order);

            var responseResult = new HttpResponseMessage(HttpStatusCode.OK) 
            {
                Content =  new StringContent(orderSerializedString, System.Text.Encoding.UTF8, "application/json" ) 
            };
            _httpClientMock.Setup(x => x.PlaceOder(It.IsAny<Order>())).ReturnsAsync(responseResult);

            _httpResponceMock = new Mock<HttpResponse>();

            Mock<HttpContext> httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.Response).Returns(_httpResponceMock.Object);
            _controllerContext = new ControllerContext(){ HttpContext = httpContextMock.Object };
        }

        [TestMethod]
        public void History_InitializedSuccessfully_ReturnsOkay()
        {
             // Arrange
            var productController = new CartController(_loggerMock.Object, _httpClientMock.Object);
            
            // Act
            var result = productController.History();

            // Assert
            var viewResult = result.Result as ViewResult;
            var history = viewResult.Model as Newtonsoft.Json.Linq.JArray; 
            Assert.AreEqual(2, history.Count);
            Assert.AreEqual(_id1, history[0].First.First.ToString());
            Assert.AreEqual(_id2, history[1].First.First.ToString());
        }
        
        [TestMethod]
        public void Index_InitializedSuccessfully_ReturnsOkay()
        {
            // Arrange
            _httpResponceMock.Setup(x => x.StatusCode).Returns((int)HttpStatusCode.OK);
            var cartController = new CartController(_loggerMock.Object, _httpClientMock.Object){
                ControllerContext = _controllerContext
            };
            
            // Act
            var result = cartController.Index(_order);

            // Assert
            var okResult = result.Result as OkResult;
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public void Index_BadResponse_ThrowsException()
        {
            // Arrange
            _httpResponceMock.Setup(x => x.StatusCode).Returns((int)HttpStatusCode.BadRequest);
            var cartController = new CartController(_loggerMock.Object, _httpClientMock.Object){
                ControllerContext = _controllerContext
            };
            
            var result = cartController.Index(_order);

            Assert.AreEqual("Order failed with status code: 400", result.Exception.InnerException.Message);
        }
    }
}
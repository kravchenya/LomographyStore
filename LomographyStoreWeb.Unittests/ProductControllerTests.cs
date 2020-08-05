using System;
using System.Collections.Generic;
using LomographyStoreWeb.Controllers;
using LomographyStoreWeb.Models;
using LomographyStoreWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace LomographyStoreWeb.Unittests
{
    [TestClass]
    public class ProductControllerTests
    {
        private Mock<ILogger<ProductController>> _loggerMock;
        private Mock<IHttCustomClient> _httpClientMock;
        private List<PhotoItem> _photos;

        [TestInitialize]
        public void Initialize()
        {
            var photoItem1 = new PhotoItem
            {
                Id = Guid.NewGuid().ToString(), Name = "Selfie", Description = "Selfie in the night", Camera = "Lomo", Film = "Kodak"
            };

            var photoItem2 = new PhotoItem
            {
                Id = Guid.NewGuid().ToString(), Name = "Double exposure", Description = "Same frame exposed twice", Camera = "Diana", Film = "Kodak"
            };

            _photos = new List<PhotoItem>();
            _photos.Add(photoItem1);
            _photos.Add(photoItem2);

            var serializedStrings = JsonConvert.SerializeObject(_photos);
            var serializedString = JsonConvert.SerializeObject(photoItem1);
        
            _loggerMock = new Mock<ILogger<ProductController>>();

            _httpClientMock = new Mock<IHttCustomClient>();
            _httpClientMock.Setup(x => x.GetAllProducts()).ReturnsAsync(serializedStrings);
            _httpClientMock.Setup(x => x.GetProductById(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(serializedString);
        }

        [TestMethod]
        public void Index_InitializedSuccessfully_ReturnsOkay()
        {
            // Arrange
            var productController = new ProductController(_loggerMock.Object, _httpClientMock.Object);
            
            // Act
            var result = productController.Index();

            // Assert
            var viewResult = result.Result as ViewResult; 
            var photosAsJson = viewResult.Model as Newtonsoft.Json.Linq.JArray;

            Assert.AreEqual(2, photosAsJson.Count);
            Assert.AreEqual(_photos[0].Id, photosAsJson[0].First.First.ToString());
            Assert.AreEqual(_photos[1].Id, photosAsJson[1].First.First.ToString());
        }

        [TestMethod]
        public void Detail_InitializedSuccessfully_ReturnsOkay()
        {
             // Arrange
            var productController = new ProductController(_loggerMock.Object, _httpClientMock.Object);
            
            // Act
            var result = productController.Detail(_photos[0].Id, _photos[0].Camera);

            // Assert
            var viewResult = result.Result as ViewResult; 
            var photosAsJson = viewResult.Model as Newtonsoft.Json.Linq.JObject;

            Assert.AreEqual(_photos[0].Id, photosAsJson.First.First.ToString());
        }
    }
}
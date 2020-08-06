using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using LomographyStoreApi.Controllers;
using LomographyStoreApi.Models;
using LomographyStoreApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace LomographyStoreApi.Unittests
{
    [TestFixture]
    public class ProductControllerTests
    {
        private Mock<ILogger<ProductController>> _loggerMock;
        private Mock<IDocumentDBService> _docService;
        private Mock<IBlobService> _blobService;

        [SetUp]
        public void Init()
        {
            _loggerMock = new Mock<ILogger<ProductController>>();
            _docService = new Mock<IDocumentDBService>();
            _blobService = new Mock<IBlobService>();
        }

        [Test]
        public async Task GetAllProducts_InitializedSuccessfully_ReturnsOkay()
        {
            // Arrange
            var photoItems = new List<PhotoItem>();
            var photoItem = new PhotoItem
            {
                Id = Guid.NewGuid().ToString(),
                Camera = "Diana",
                Description = "Photo taken with Diana camera",
                Name = "My favorite photo",
                Film = "Kodak"
            };
            photoItems.Add(photoItem);


            var productController = new ProductController(_loggerMock.Object, _docService.Object, _blobService.Object);
            
            _docService.Setup(x => x.GetProductsAsync()).ReturnsAsync(photoItems);
            
            // Act
            var result = await productController.GetAllProducts();

            // Assert
            var jsonResult = result as JsonResult;
            var photoItemsResult =  jsonResult.Value as List<PhotoItem>;
            Assert.AreEqual(1, photoItems.Count);
            Assert.AreEqual(photoItem.Id, photoItemsResult[0].Id);
        }

        [Test]
        public async Task GetAllProducts_InitializedNegatively_ReturnsInternalServerError()
        {
            // Arrange
            var productController = new ProductController(_loggerMock.Object, _docService.Object, _blobService.Object);
            
            _docService.Setup(x => x.GetProductsAsync()).ThrowsAsync(new InvalidOperationException());
            
            // Act
            var result = await productController.GetAllProducts();

            // Assert
            var codeRsult = result as StatusCodeResult;
            Assert.AreEqual(500, codeRsult.StatusCode);
        }

        [Test]
        public async Task GetProductById_InitializedSuccessfully_ReturnsOkay()
        {
            // Arrange
            var photoItem = new PhotoItem
            {
                Id = Guid.NewGuid().ToString(),
                Camera = "Diana",
                Description = "Photo taken with Diana camera",
                Name = "My favorite photo",
                Film = "Kodak"
            };
           
            var productController = new ProductController(_loggerMock.Object, _docService.Object, _blobService.Object);
            
            _docService.Setup(x => x.GetProductAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(photoItem);
            
            // Act
            var result = await productController.GetProductById("123", "Diana");

            // Assert
            var jsonResult = result as JsonResult;
            var photoItemsResult =  jsonResult.Value as PhotoItem;
            Assert.AreEqual(photoItem.Id, photoItemsResult.Id);
        }

        [Test]
        public async Task GetProductById_InitializedNegatively_ReturnsInternalServerError()
        {
            // Arrange
            var productController = new ProductController(_loggerMock.Object, _docService.Object, _blobService.Object);
            
            _docService.Setup(x => x.GetProductAsync(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new InvalidOperationException());
            
            // Act
            var result = await productController.GetProductById("123", "Diana");

            // Assert
            var codeRsult = result as StatusCodeResult;
            Assert.AreEqual(500, codeRsult.StatusCode);
        }


        [Test]
        public async Task AddNewPhotoItem_InitializedSuccessfully_ReturnsOkay()
        {
            // Arrange
            var productController = new ProductController(_loggerMock.Object, _docService.Object, _blobService.Object);

            var photoItem = new PhotoItem
            {
                Id = Guid.NewGuid().ToString(),
                Camera = "Diana",
                Description = "Photo taken with Diana camera",
                Name = "My favorite photo",
                Film = "Kodak"
            };
            
            _docService.Setup(x => x.AddProductAsync(It.IsAny<PhotoItem>())).ReturnsAsync(photoItem);
            

            // Act
            var result = await productController.AddNewPhotoItem(photoItem);

            // Assert
            var jsonResult = result as JsonResult;
            var photoItemsResult =  jsonResult.Value as PhotoItem;
            Assert.AreEqual(photoItem.Id, photoItemsResult.Id);
        }

        [Test]
        public async Task AddNewPhotoItem_InitializedNegatively_ReturnsInternalServerError()
        {
            // Arrange
            var productController = new ProductController(_loggerMock.Object, _docService.Object, _blobService.Object);
            
            _docService.Setup(x => x.AddProductAsync(It.IsAny<PhotoItem>())).ThrowsAsync(new InvalidOperationException());

            // Act
            var result = await productController.AddNewPhotoItem(new PhotoItem());

            // Assert
            var codeRsult = result as StatusCodeResult;
            Assert.AreEqual(500, codeRsult.StatusCode);
        }

        [Test]
        public async Task AddProductImage_InitializedSuccessfully_ReturnsOkay()
        {
            // Arrange         
            _blobService.Setup(x => x.UploadBlobAsync(It.IsAny<string>(), It.IsAny<Stream>())).ReturnsAsync("some string");
            _docService.Setup(x => x.AddImageToProductAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var formFileMock = new Mock<IFormFile>();
            var stream = new MemoryStream();
            formFileMock.Setup(x => x.OpenReadStream()).Returns(stream);

            var routeData = new RouteData();
            routeData.Values.Add( "id", "123" );
            routeData.Values.Add( "camera", "Diana" );

            var requestMock = new Mock<HttpRequest>();
            requestMock.Setup(x => x.HasFormContentType).Returns(true);
            Mock<HttpContext> httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(x => x.Request).Returns(requestMock.Object);
            
            var controllerContext = new ControllerContext(){ HttpContext = httpContextMock.Object, RouteData = routeData };

            var productController = new ProductController(_loggerMock.Object, _docService.Object, _blobService.Object)
            {
                ControllerContext = controllerContext
            };

            
            // Act
            var result = await productController.AddProductImage(formFileMock.Object);

            // Assert
            var okResult = result as OkResult;
            Assert.AreEqual(200, okResult.StatusCode);
        }

         [Test]
        public async Task AddProductImage_InitializedNegatively_ReturnsInternalServerError()
        {
            // Arrange         
            var formFileMock = new Mock<IFormFile>();

            var productController = new ProductController(_loggerMock.Object, _docService.Object, _blobService.Object);
            
            // Act
            var result = await productController.AddProductImage(formFileMock.Object);

            // Assert
           var codeRsult = result as StatusCodeResult;
            Assert.AreEqual(500, codeRsult.StatusCode);
        } 
    }
}